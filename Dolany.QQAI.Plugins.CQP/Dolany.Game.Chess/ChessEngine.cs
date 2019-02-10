using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Common;

namespace Dolany.Game.Chess
{
    public partial class ChessEngine
    {
        public long SelfQQNum { get; set; }

        public long AimQQNum { get; set; }

        public long GroupNum { get; set; }

        private readonly Action<string, long, long> MsgCallBack;

        private readonly List<ChessEffectModel> EffectsList = new List<ChessEffectModel>();

        private ChessEffectModel[] Chessborad;

        public ChessEngine(long GroupNum, long SelfQQNum, long AimQQNum, Action<string, long, long> MsgCallBack)
        {
            this.SelfQQNum = SelfQQNum;
            this.AimQQNum = AimQQNum;
            this.MsgCallBack = MsgCallBack;
            this.GroupNum = GroupNum;

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
            InitChessBoard();

            for (var i = 0; i < 6; i++)
            {
                ProceedTurn();

                var temp = SelfQQNum;
                SelfQQNum = AimQQNum;
                AimQQNum = temp;
            }

            MsgCallBack("对决结束！", GroupNum, 0);
        }

        private void ProceedTurn()
        {

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
