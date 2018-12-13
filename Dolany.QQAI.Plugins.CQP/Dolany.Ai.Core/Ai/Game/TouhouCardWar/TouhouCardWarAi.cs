namespace Dolany.Ai.Core.Ai.Game.TouhouCardWar
{
    using System.Collections.Generic;
    using System.Linq;

    using Dolany.Ai.Core.Base;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Db;

    [AI(
        Name = nameof(TouhouCardWarAi),
        Description = "AI for Touhou Card War Game.",
        IsAvailable = false,
        PriorityLevel = 10)]
    public class TouhouCardWarAi : AIBase
    {
        private readonly object _lockObj = new object();
        private List<WarGameMgr> Mgrs = new List<WarGameMgr>();

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "GameStart",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "Start Touhou Card War!",
            Syntax = "",
            Tag = "游戏功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailabe = false)]
        public void GameStart(MsgInformationEx MsgDTO, object[] param)
        {
            lock (_lockObj)
            {
                if (Mgrs.Any(m => m.GroupNum == MsgDTO.FromGroup))
                {
                    MsgSender.Instance.PushMsg(MsgDTO, "游戏正在进行中，请稍后再试！");
                    return;
                }
            }

            if (!CreateCharactor(MsgDTO))
            {
                return;
            }

            var wgMgr = new WarGameMgr(MsgDTO.FromGroup);
            lock (_lockObj)
            {
                Mgrs.Add(wgMgr);
            }
            wgMgr.GameStart();
        }

        private bool CreateCharactor(MsgInformationEx MsgDTO)
        {
            // todo
            return false;
        }

        private void GameOver(long GroupNum)
        {
            lock (_lockObj)
            {
                Mgrs.Remove(Mgrs.First(m => m.GroupNum == GroupNum));
            }
        }
    }
}
