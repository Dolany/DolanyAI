using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    public class SignInAI : AIBase
    {
        public override string AIName { get; set; } = "签到";

        public override string Description { get; set; } = "AI for Everyday Signing In";

        public override int PriorityLevel { get; set; } = 10;

        public override bool NeedManualOpeon { get; set; } = true;

        private Dictionary<long, SignInGroupRecord> GroupSignInDic = new Dictionary<long, SignInGroupRecord>();

        public override void Initialization()
        {
            var records = MongoService<SignInGroupRecord>.Get();
            GroupSignInDic = records.ToDictionary(p => p.GroupNum, p => p);
        }

        [EnterCommand(ID = "SignInAI_SetSignContent",
            Command = "签到",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置签到内容(不能与系统自带命令重复)",
            Syntax = "[签到内容]",
            Tag = "商店功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool SetSignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            if (AIMgr.Instance.AllAvailableGroupCommands.Any(comm => comm.Command == content))
            {
                MsgSender.PushMsg(MsgDTO, "不能与系统自带命令重复！");
                return false;
            }

            if (GroupSignInDic.ContainsKey(MsgDTO.FromGroup))
            {
                var groupSignIn = GroupSignInDic[MsgDTO.FromGroup];
                groupSignIn.Content = content;
                groupSignIn.Update();
            }
            else
            {
                var groupSignIn = new SignInGroupRecord(){GroupNum = MsgDTO.FromGroup, Content = content};
                MongoService<SignInGroupRecord>.Insert(groupSignIn);
                GroupSignInDic.Add(MsgDTO.FromGroup, groupSignIn);
            }
            MsgSender.PushMsg(MsgDTO, "设置成功！");
            return true;
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Private || !GroupSettingMgr.Instance[MsgDTO.FromGroup].HasFunction(AIName))
            {
                return false;
            }

            // 群组签到验证
            var groupInfo = GroupSignInDic.ContainsKey(MsgDTO.FromGroup) ? GroupSignInDic[MsgDTO.FromGroup] : null;
            if (groupInfo == null && MsgDTO.FullMsg != "签到")
            {
                return false;
            }

            if (groupInfo != null && MsgDTO.FullMsg != groupInfo.Content)
            {
                return false;
            }

            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = AIName,
                Command = "SignInOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });

            // 个人签到验证
            var personRecord = SignInPersonRecord.Get(MsgDTO.FromQQ);
            SignInPersonGroupInfo ginfo;
            if (personRecord.GroupInfos.ContainsKey(MsgDTO.FromGroup.ToString()))
            {
                ginfo = personRecord.GroupInfos[MsgDTO.FromGroup.ToString()];
            }
            else
            {
                ginfo = new SignInPersonGroupInfo();
                personRecord.GroupInfos.Add(MsgDTO.FromGroup.ToString(), ginfo);
            }

            if (ginfo.LastSignInDate.HasValue && ginfo.LastSignInDate.Value.ToLocalTime() > DateTime.Now.Date)
            {
                MsgSender.PushMsg(MsgDTO, "你今天已经签过到啦！");
                return true;
            }

            Sign(ginfo, MsgDTO);
            personRecord.Update();
            return true;
        }

        private static void Sign(SignInPersonGroupInfo ginfo, MsgInformationEx MsgDTO)
        {
            if (ginfo.LastSignInDate == null || ginfo.LastSignInDate.Value.ToLocalTime().Date < DateTime.Today.AddDays(-1))
            {
                ginfo.SuccessiveDays = 0;
            }

            if (string.IsNullOrEmpty(GlobalVarRecord.Get("SignInAcc").Value))
            {
                ginfo.SuccessiveDays++;
            }
            else
            {
                ginfo.SuccessiveDays += 2;
            }

            ginfo.LastSignInDate = DateTime.Now;
            var goldsGen = ginfo.SuccessiveDays > 5 ? 25 : ginfo.SuccessiveDays * 5;

            OSPerson.GoldIncome(MsgDTO.FromQQ, goldsGen);

            var msg = $"签到成功！你已连续签到 {ginfo.SuccessiveDays}天，获得 {goldsGen}金币！";
            if (ginfo.SuccessiveDays % 10 == 0)
            {
                var cache = PersonCacheRecord.Get(MsgDTO.FromQQ, "抽奖");
                cache.Value = !string.IsNullOrEmpty(cache.Value) && int.TryParse(cache.Value, out var times) ? (times + 1).ToString() : 1.ToString();
                cache.Update();

                msg += "\r恭喜你获得一次抽奖机会，快去试试吧（当日有效！）";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(ID = "SignInAI_TodaySignContent",
            Command = "今日签到内容",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "获取今日签到内容",
            Syntax = "",
            Tag = "商店功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool TodaySignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = GroupSignInDic.ContainsKey(MsgDTO.FromGroup) ? GroupSignInDic[MsgDTO.FromGroup].Content : "签到";
            MsgSender.PushMsg(MsgDTO, $"今日签到内容是：{content}");
            return true;
        }
    }
}
