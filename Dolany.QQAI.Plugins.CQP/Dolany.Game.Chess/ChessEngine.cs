﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Dolany.Ai.Common;

namespace Dolany.Game.Chess
{
    public partial class ChessEngine
    {
        public long SelfQQNum { get; set; }

        public long AimQQNum { get; set; }

        public long GroupNum { get; }

        private readonly Action<string, long, long> MsgCallBack;

        private readonly Func<long, long, string, Predicate<string>, string> WaitCallBack;

        private readonly List<ChessEffectModel> EffectsList = new List<ChessEffectModel>();

        private ChessEffectModel[] Chessborad;

        private IEnumerable<int> AvailableNums
        {
            get
            {
                var list = new List<int>();
                for (var i = 0; i < Chessborad.Length; i++)
                {
                    if (!Chessborad[i].IsChecked)
                    {
                        list.Add(i + 1);
                    }
                }

                return list;
            }
        }

        public ChessEngine(long GroupNum, long SelfQQNum, long AimQQNum, Action<string, long, long> MsgCallBack, Func<long, long, string, Predicate<string>, string> WaitCallBack)
        {
            this.SelfQQNum = SelfQQNum;
            this.AimQQNum = AimQQNum;
            this.MsgCallBack = MsgCallBack;
            this.GroupNum = GroupNum;
            this.WaitCallBack = WaitCallBack;

            var type = GetType();
            var methods = type.GetMethods();
            foreach (var method in methods)
            {
                if (!(method.GetCustomAttribute(typeof(ChessEffectAttribute)) is ChessEffectAttribute attr))
                {
                    continue;
                }

                EffectsList.Add(new ChessEffectModel
                {
                    Name = attr.Name,
                    Description = attr.Description,
                    Method = method.CreateDelegate(typeof(Action), this) as Action
                });
            }
        }

        public void GameStart()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    InitChessBoard();

                    for (var i = 0; i < 6; i++)
                    {
                        ProceedTurn();
                        Thread.Sleep(1000);

                        var temp = SelfQQNum;
                        SelfQQNum = AimQQNum;
                        AimQQNum = temp;
                    }

                    MsgCallBack("对决结束！", GroupNum, 0);
                }
                catch (Exception)
                {
                    MsgCallBack("系统异常！", GroupNum, 0);
                }
            });
        }

        private void ProceedTurn()
        {
            var response = WaitCallBack(GroupNum, SelfQQNum, $"回合开始！请输入合适的数字来获取随机效果！\r{ChessBoardStr()}",
                msg => int.TryParse(msg, out var num) && AvailableNums.Contains(num));

            if (string.IsNullOrEmpty(response) || !int.TryParse(response, out var selectedNum) || !AvailableNums.Contains(selectedNum))
            {
                MsgCallBack("异常输入，回合结束！", GroupNum, 0);
                return;
            }

            var model = Chessborad[selectedNum];

            model.Method();
            model.IsChecked = true;

            MsgCallBack($"随机效果已生效：{model.Name}！\r{model.Description}", GroupNum, SelfQQNum);
            MsgCallBack("回合结束！", GroupNum, 0);
        }

        private void InitChessBoard()
        {
            Chessborad = CommonUtil.RandSort(EffectsList.ToArray()).Take(9).ToArray();
        }

        private string ChessBoardStr()
        {
            var str = string.Empty;
            for (var i = 0; i < Chessborad.Length; i++)
            {
                var model = Chessborad[i];
                str += model.IsChecked ? model.Name : (i + 1).ToString();
                str += (i + 1) % 3 == 0 ? "\r" : "  ";
            }

            return str;
        }

        public bool DeEffect(string name)
        {
            var effect = EffectsList.FirstOrDefault(e => e.Name == name);
            if (effect == null)
            {
                return false;
            }

            effect.Method();
            return true;
        }
    }
}