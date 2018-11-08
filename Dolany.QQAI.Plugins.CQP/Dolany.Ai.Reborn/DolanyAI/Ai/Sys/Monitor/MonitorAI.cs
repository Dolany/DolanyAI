using System;
using System.Linq;
using Dolany.Ai.Reborn.DolanyAI.Base;
using Dolany.Ai.Reborn.DolanyAI.Cache;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.Db;
using Dolany.Ai.Reborn.DolanyAI.DTO;
using Dolany.Ai.Reborn.DolanyAI.Entities;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.Sys.Monitor
{
    [AI(
        Name = nameof(MonitorAI),
        Description = "AI for Monitor Ais status and emitting heart beat.",
        IsAvailable = true,
        PriorityLevel = 100
        )]
    public class MonitorAI : AIBase
    {
        public override void Work()
        {
        }

        [EnterCommand(
            Command = "功能封印",
            Description = "封印一个群的某个ai功能",
            Syntax = "[群组号] [需要封印的ai名]",
            Tag = "系统命令",
            SyntaxChecker = "Long Word",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailabe = true
            )]
        public void SealAi(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var groupNum = (long)param[0];
            var aiName = GetAiRealName(param[1] as string);
            if (string.IsNullOrEmpty(aiName))
            {
                SendMsgToDeveloper("查找ai失败！");
                return;
            }

            using (var db = new AIDatabase())
            {
                var query = db.AISeal.Where(a => a.GroupNum == groupNum &&
                                                 a.AiName == aiName);
                if (!query.IsNullOrEmpty())
                {
                    SendMsgToDeveloper("ai功能已经在封印中！");
                    return;
                }

                var aiseal = new AISeal
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = groupNum,
                    AiName = aiName
                };
                db.AISeal.Add(aiseal);
                db.SaveChanges();
            }
            SendMsgToDeveloper("ai封印成功！");
        }

        private static string GetAiRealName(string aiName)
        {
            var list = AIMgr.Instance.AIList;
            foreach (var ai in list)
            {
                var t = ai.GetType();
                var attributes = t.GetCustomAttributes(typeof(AIAttribute), false);
                if (attributes.Length <= 0 ||
                    !(attributes[0] is AIAttribute))
                {
                    continue;
                }
                var attr = (AIAttribute)attributes[0];
                if (attr.Name == aiName)
                {
                    return t.Name;
                }
            }

            return string.Empty;
        }

        public override bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            FiltPicMsg(MsgDTO);

            using (var db = new AIDatabase())
            {
                var selfNum = SelfQQNum;
                var query = db.ActiveOffGroups.Where(p => p.AINum == selfNum &&
                                                          p.GroupNum == MsgDTO.FromGroup);
                return !query.IsNullOrEmpty();
            }
        }

        private static void FiltPicMsg(ReceivedMsgDTO MsgDTO)
        {
            var guid = ParsePicGuid(MsgDTO.FullMsg);
            var cacheInfo = ReadImageCacheInfo(guid);
            if (cacheInfo == null)
            {
                return;
            }
            DbMgr.Insert(new PicCacheEntity
            {
                Id = Guid.NewGuid().ToString(),
                FromGroup = MsgDTO.FromGroup,
                FromQQ = MsgDTO.FromQQ,
                Content = cacheInfo.url,
                SendTime = DateTime.Now
            });

            var MaxPicCacheCount = int.Parse(GetConfig("MaxPicCacheCount"));
            var pics = DbMgr.Query<PicCacheEntity>();
            var count = pics.Count();
            if (count <= MaxPicCacheCount)
            {
                return;
            }

            var redundantPics = pics.OrderBy(p => p.SendTime).Take(count - MaxPicCacheCount);
            foreach (var pic in redundantPics)
            {
                DbMgr.Delete<PicCacheEntity>(pic.Id);
            }
        }

        [EnterCommand(
            Command = "关机 PowerOff",
            Description = "让机器人休眠",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailabe = false
        )]
        public void PowerOff(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var selfNum = SelfQQNum;
                var query = db.ActiveOffGroups.Where(p => p.AINum == selfNum &&
                                                          p.GroupNum == MsgDTO.FromGroup);
                if (!query.IsNullOrEmpty())
                {
                    return;
                }

                db.ActiveOffGroups.Add(new ActiveOffGroups
                {
                    Id = Guid.NewGuid().ToString(),
                    AINum = selfNum,
                    GroupNum = MsgDTO.FromGroup,
                    UpdateTime = DateTime.Now
                });

                db.SaveChanges();
                MsgSender.Instance.PushMsg(MsgDTO, "关机成功！");
            }

            AIMgr.Instance.OnActiveStateChange(false, MsgDTO.FromGroup);
        }

        [EnterCommand(
            Command = "开机 PowerOn",
            Description = "唤醒机器人",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.管理员,
            IsPrivateAvailabe = false
        )]
        public void PowerOn(ReceivedMsgDTO MsgDTO, object[] param)
        {
            using (var db = new AIDatabase())
            {
                var selfNum = SelfQQNum;
                var query = db.ActiveOffGroups.Where(p => p.AINum == selfNum &&
                                                          p.GroupNum == MsgDTO.FromGroup);
                if (query.IsNullOrEmpty())
                {
                    return;
                }

                db.ActiveOffGroups.RemoveRange(query);

                db.SaveChanges();
                MsgSender.Instance.PushMsg(MsgDTO, "开机成功！");
            }

            AIMgr.Instance.OnActiveStateChange(true, MsgDTO.FromGroup);
        }

        [EnterCommand(
            Command = "系统状态 .State",
            Description = "获取机器人当前状态",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.开发者,
            IsPrivateAvailabe = false
        )]
        public void Status(ReceivedMsgDTO MsgDTO, object[] param)
        {
            var timeStatus = DbMgr.Query<SysStatusEntity>(p => p.Key == "StartTime");
            var startTime = DateTime.Parse(timeStatus.First().Content);
            var span = DateTime.Now - startTime;
            var timeStr = span.ToString(@"dd天HH小时mm分ss秒");

            var countStatus = DbMgr.Query<SysStatusEntity>(p => p.Key == "Count");
            var count = int.Parse(countStatus.First().Content);

            MsgSender.Instance.PushMsg(MsgDTO, $@"系统已成功运行{timeStr}, 共处理{count}条指令");
        }
    }
}
