using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dolany.Ai.Common;

namespace Dolany.Game.Chess
{
    public partial class ChessEngine
    {
        public long SelfQQNum { get; set; }

        public long AimQQNum { get; set; }

        public long GroupNum { get; }

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

        public ChessEngine(long GroupNum, long SelfQQNum, long AimQQNum, Func<long, long, string, Predicate<string>, string> WaitCallBack)
        {
            this.SelfQQNum = SelfQQNum;
            this.AimQQNum = AimQQNum;
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
            try
            {
                CommonUtil.MsgSendBack(GroupNum, 0, "对决即将开始，请双方做好准备！");
                Thread.Sleep(2000);
                InitChessBoard();

                for (var i = 0; i < 6; i++)
                {
                    ProceedTurn();
                    Thread.Sleep(1000);

                    var temp = SelfQQNum;
                    SelfQQNum = AimQQNum;
                    AimQQNum = temp;
                }

                CommonUtil.MsgSendBack(GroupNum, 0, "对决结束！");
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                CommonUtil.MsgSendBack(GroupNum, 0, "系统异常，游戏结束！");
            }

            ChessMgr.Instance.GameOver(this);
        }

        private void ProceedTurn()
        {
            var response = WaitCallBack(GroupNum, SelfQQNum, $"回合开始！请输入合适的数字来获取随机效果！\r{ChessBoardStr()}",
                msg => int.TryParse(msg, out var num) && AvailableNums.Contains(num));

            if (string.IsNullOrEmpty(response) || !int.TryParse(response, out var selectedNum) || !AvailableNums.Contains(selectedNum))
            {
                CommonUtil.MsgSendBack(GroupNum, 0, "回合结束！");
                return;
            }

            var model = Chessborad[selectedNum - 1];

            CommonUtil.MsgSendBack(GroupNum, 0, $"随机效果已生效：{model.Name}！\r{model.Description}");
            Thread.Sleep(1000);

            model.Method();
            model.IsChecked = true;

            Thread.Sleep(1000);
            CommonUtil.MsgSendBack(GroupNum, 0, "回合结束！");
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
                str += model.IsChecked ? model.Name : $"{(i + 1).ToString()}  ";
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
