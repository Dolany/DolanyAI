using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.ChessAgainst
{
    public partial class ChessEngine
    {
        public long SelfQQNum { get; set; }

        public long AimQQNum { get; set; }

        public long GroupNum { get; }

        public string BindAi { get; set; }

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

        public ChessEngine(long GroupNum, long SelfQQNum, long AimQQNum, Func<long, long, string, Predicate<string>, string> WaitCallBack, string BindAi)
        {
            this.SelfQQNum = SelfQQNum;
            this.AimQQNum = AimQQNum;
            this.GroupNum = GroupNum;
            this.WaitCallBack = WaitCallBack;
            this.BindAi = BindAi;

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
                MsgSender.PushMsg(GroupNum, 0, "对决即将开始，请双方做好准备！", BindAi);
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

                MsgSender.PushMsg(GroupNum, 0, "对决结束！", BindAi);
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                MsgSender.PushMsg(GroupNum, 0, "系统异常，游戏结束！", BindAi);
            }

            ChessMgr.Instance.GameOver(this);
        }

        private void ProceedTurn()
        {
            var response = WaitCallBack(GroupNum, SelfQQNum, $"回合开始！请输入合适的数字来获取随机效果！\r{ChessBoardStr()}",
                msg => int.TryParse(msg, out var num) && AvailableNums.Contains(num));

            if (string.IsNullOrEmpty(response) || !int.TryParse(response, out var selectedNum) || !AvailableNums.Contains(selectedNum))
            {
                MsgSender.PushMsg(GroupNum, 0, "回合结束！", BindAi);
                return;
            }

            var model = Chessborad[selectedNum - 1];

            MsgSender.PushMsg(GroupNum, 0, $"随机效果已生效：{model.Name}！\r{model.Description}", BindAi);
            Thread.Sleep(1000);

            model.Method();
            model.IsChecked = true;

            Thread.Sleep(1000);
            MsgSender.PushMsg(GroupNum, 0, "回合结束！", BindAi);
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
