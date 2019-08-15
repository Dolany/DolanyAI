using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetCardGameEngine : IGameEngine<PetCardRecord>
    {
        public override PetCardRecord[] Gamers { get; set; }
        public override long GroupNum { get; set; }
        public override string BindAi { get; set; }
        protected override int MaxTurn { get; set; } = 9;

        protected override void SendMessage(string msg)
        {
        }

        protected override bool JudgeWinner()
        {
            return false;
        }

        protected override void ProcessTurn()
        {
        }

        protected override void ShowResult()
        {
        }
    }
}
