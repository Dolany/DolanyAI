﻿using System.Collections.Generic;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Ai.Doremi.Xiuxian;

namespace Dolany.Ai.Doremi.Ai.Game.Xiuxian
{
    [AI(Name = "修仙计数器",
        Description = "AI for Msg Count for Xiuxian.",
        Enable = true,
        PriorityLevel = 15,
        NeedManulOpen = true,
        BindAi = "DoreFun")]
    public class MsgCounterAI : AIBase
    {
        private List<long> EnablePersons = new List<long>();

        public override void Initialization()
        {
            EnablePersons = MsgCounterSvc.GetAllEnabledPersons();
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private || !EnablePersons.Contains(MsgDTO.FromQQ))
            {
                return false;
            }

            MsgCounterSvc.Cache(MsgDTO.FromQQ);
            return false;
        }

        [EnterCommand(ID = "MsgCounterAI_Enable",
            Command = "开启修仙模式",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "开启修仙模式，每日发言将会获取经验值，经验值可用于升级",
            Syntax = "",
            Tag = "修仙功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool Enable(MsgInformationEx MsgDTO, object[] param)
        {
            if (EnablePersons.Contains(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你已经开启了修仙模式！", true);
                return false;
            }

            EnablePersons.Add(MsgDTO.FromQQ);
            MsgCounterSvc.PersonEnable(MsgDTO.FromQQ);

            MsgSender.PushMsg(MsgDTO, "开启成功！");
            return true;
        }

        [EnterCommand(ID = "MsgCounterAI_Disable",
            Command = "关闭修仙模式",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "关闭修仙模式，将不会获得新的经验值，原有的数据还将保留",
            Syntax = "",
            Tag = "修仙功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool Disable(MsgInformationEx MsgDTO, object[] param)
        {
            if (!EnablePersons.Contains(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你尚未开启修仙模式！", true);
                return false;
            }

            EnablePersons.Remove(MsgDTO.FromQQ);
            MsgCounterSvc.PersonDisable(MsgDTO.FromQQ);

            MsgSender.PushMsg(MsgDTO, "关闭成功！");
            return true;
        }

        [EnterCommand(ID = "MsgCounterAI_Upgrade",
            Command = "渡劫",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "消耗经验值升级",
            Syntax = "",
            Tag = "修仙功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool Upgrade(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var level = LevelMgr.Instance.GetByLevel(osPerson.Level);
            var exp = MsgCounterSvc.Get(MsgDTO.FromQQ);

            if (exp < level.Exp)
            {
                MsgSender.PushMsg(MsgDTO, "你没有足够的经验值升级！", true);
                return false;
            }

            osPerson.Level++;
            MsgCounterSvc.Consume(MsgDTO.FromQQ, exp -= level.Exp);

            MsgSender.PushMsg(MsgDTO, "升级成功！");
            return true;
        }
    }
}
