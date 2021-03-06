﻿using System;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.Standard.Ai.Vip.VipArmers
{
    public class 耐力护符 : IVipArmer
    {
        public string Name { get; set; } = "耐力护符";
        public string Description { get; set; } = "使宠物的耐力上限增加10，持续10天(同时只能持有一个)";
        public int DiamondsNeed { get; set; } = 20;
        public VipArmerLimitInterval LimitInterval { get; set; }
        public int LimitCount { get; set; }

        public bool Purchase(MsgInformationEx MsgDTO)
        {
            var armerRec = VipArmerRecord.Get(MsgDTO.FromQQ);
            var armer = new ArmerModel() {Name = "耐力护符", Description = "使宠物的耐力上限增加10，持续10天。", ExpiryTime = DateTime.Now.AddDays(10)};
            armerRec.Armers.Add(armer);
            armerRec.Update();

            MsgSender.PushMsg(MsgDTO, $"购买成功！有效期至：{armer.ExpiryTime:yyyy-MM-dd HH:mm:ss}");
            return true;
        }

        public int MaxContains { get; set; } = 1;
    }
}
