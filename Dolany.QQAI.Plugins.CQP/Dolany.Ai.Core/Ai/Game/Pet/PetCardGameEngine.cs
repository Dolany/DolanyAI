using System;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetCardGameEngine
    {
        public readonly PetCardRecord[] GamingPets;

        private readonly string BindAi;

        public readonly long GroupNum;

        private int CurIdx;

        private PetCardRecord SelfPet => GamingPets[CurIdx];
        private PetCardRecord AimPet => GamingPets[(CurIdx + 1) % GamingPets.Length];

        private long LoserQQ = -1;

        private PetCardRecord Winner => GamingPets.FirstOrDefault(p => p.QQNum != LoserQQ);
        private PetCardRecord Loser => GamingPets.FirstOrDefault(p => p.QQNum == LoserQQ);

        public PetCardGameEngine(PetCardRecord[] GamingPets, string BindAi, long GroupNum)
        {
            this.GamingPets = GamingPets;
            this.BindAi = BindAi;
            this.GroupNum = GroupNum;
        }

        public void StartGame()
        {
            BeforeGameStart();

            try
            {
                for (var i = 0; i < 9; i++)
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

                    SwitchTurn();
                }

                DoResult();
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                SendBack("系统EA60模块发现异常，对决中止！");
            }
        }

        private void BeforeGameStart()
        {
            // todo
        }

        private void BeforeTurnStart()
        {
            // todo
        }

        private void ProcessTurn()
        {
            // todo
        }

        private bool JudgeWinner()
        {
            var losers = GamingPets.Where(p => p.HP > p.MaxHP || p.HP <= 0).ToList();
            if (!losers.Any())
            {
                return false;
            }

            if (losers.Count == 2)
            {
                LoserQQ = -1;
            }
            else
            {
                LoserQQ = losers.First().QQNum;
            }

            return true;
        }

        private void DoResult()
        {
            // todo
        }

        private void SwitchTurn()
        {
            CurIdx = (CurIdx + 1) % GamingPets.Length;
        }

        private void SendBack(string msg, long atQQ = 0)
        {
            Thread.Sleep(1500);
            MsgSender.PushMsg(GroupNum, atQQ, msg, BindAi);
        }
    }
}
