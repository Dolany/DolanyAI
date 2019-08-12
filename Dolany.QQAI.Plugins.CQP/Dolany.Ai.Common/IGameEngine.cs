using System;
using System.Linq;

namespace Dolany.Ai.Common
{
    public abstract class IGameEngine<GamerModel> where GamerModel : IQQNumEntity
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Name { protected get; set; }

        public abstract GamerModel[] Gamers { get; set; }

        public abstract long GroupNum { get; set; }

        public abstract string BindAi { get; set; }

        protected int CurIdx { get; set; }

        protected GamerModel CurGamer => Gamers[CurIdx];

        protected abstract int MaxTurn { get; set; }

        public void StartGame()
        {
            RuntimeLogger.Log($"{Name}已开始;Id:{Id};Group:{GroupNum};Gamers:{string.Join(",", Gamers.Select(g => g.QQNum.ToString()))}");

            try
            {
                BeforeGameStart();

                for (var i = 0; i < MaxTurn; i++)
                {
                    BeforeTurnStart();

                    if (JudgeWinner())
                    {
                        break;
                    }

                    ProcessTurn();
                    if (JudgeWinner())
                    {
                        break;
                    }

                    AfterTurnEnd();
                    if (JudgeWinner())
                    {
                        break;
                    }

                    SwithGamer();
                }

                ShowResult();
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                SendMessage($"发现异常，游戏中止！(ID:{Id})");
            }
        }

        protected abstract void SendMessage(string msg);

        protected virtual void BeforeGameStart()
        {

        }

        protected virtual void BeforeTurnStart()
        {

        }

        protected virtual void AfterTurnEnd()
        {

        }

        protected void SwithGamer()
        {
            CurIdx = (CurIdx + 1) % Gamers.Length;
        }

        protected abstract bool JudgeWinner();

        protected abstract void ProcessTurn();

        protected abstract void ShowResult();
    }
}
