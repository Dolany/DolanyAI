using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchColleAI : AIBase
    {
        public override string AIName { get; set; } = "考古收藏";
        public override string Description { get; set; } = "Ai for everything about ArchCollections.";

        [EnterCommand(ID = "ArchColleAI_MyStatus", Command = "我的收藏品碎片", Description = "查看自己的收藏品碎片")]
        public bool ItemColles(MsgInformationEx MsgDTO, object[] param)
        {
            var collection = ArchCollection.Get(MsgDTO.FromQQ);
            if (collection.ItemColles.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何收藏品碎片！", true);
                return false;
            }

            var msgs = collection.ItemColles.Select(p => $"{p.Name}:{string.Join(", ", p.Segments.OrderBy(s => s.Key).Select(s => $"{s.Key}*{s.Value}"))}");
            MsgSender.PushMsg(MsgDTO, string.Join("\r\n", msgs));
            return true;
        }

        [EnterCommand(ID = "ArchColleAI_SpecialColles", Command = "我的特殊收藏品", Description = "查看自己的特殊收藏品")]
        public bool SpecialColles(MsgInformationEx MsgDTO, object[] param)
        {
            var collection = ArchCollection.Get(MsgDTO.FromQQ);
            if (collection.SpecialColles.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何特殊收藏品！", true);
                return false;
            }

            var msg = $"你当前拥有的特殊收藏品有：\r\n{string.Join(",", collection.SpecialColles)}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "ArchColleAI_MyColles", Command = "我的收藏品", Description = "查看自己的考古收藏品")]
        public bool MyColles(MsgInformationEx MsgDTO, object[] param)
        {
            var collection = ArchCollection.Get(MsgDTO.FromQQ);
            if (collection.Collectables.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何考古收藏品！", true);
                return false;
            }

            var msg = $"你当前拥有的考古收藏品有：\r\n{string.Join(",", collection.Collectables.Select(p => $"{p.Key}*{p.Value}"))}";
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "ArchColleAI_CombineSegments", Command = "合成考古碎片", Description = "将所有可以合成的碎片合成为考古收藏")]
        public bool CombineSegments(MsgInformationEx MsgDTO, object[] param)
        {
            var collection = ArchCollection.Get(MsgDTO.FromQQ);
            if (collection.ItemColles.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何收藏品碎片！", true);
                return false;
            }

            if (collection.ItemColles.All(p => p.Segments.IsNullOrEmpty() || p.Segments.Count < 10))
            {
                MsgSender.PushMsg(MsgDTO, "你没有任何可以合成的收藏品碎片！", true);
                return false;
            }

            var combineDic = new SafeDictionary<string, int>(new Dictionary<string, int>());
            while ()
            {
                
            }
        }
    }
}
