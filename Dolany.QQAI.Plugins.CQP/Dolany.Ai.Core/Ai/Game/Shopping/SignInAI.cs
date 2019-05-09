using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.OnlineStore;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.Game.Shopping
{
    [AI(
        Name = "签到",
        Description = "AI for Everyday Signing In.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class SignInAI : AIBase
    {
        private Dictionary<long, SignInGroupRecord> GroupSignInDic = new Dictionary<long, SignInGroupRecord>();

        public override void Initialization()
        {
            base.Initialization();

            var records = MongoService<SignInGroupRecord>.Get();
            GroupSignInDic = records.ToDictionary(p => p.GroupNum, p => p);
        }

        [EnterCommand(Command = "签到",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置签到内容，有效期1个月(不能与系统自带命令重复)",
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

            if (MsgDTO.Type == MsgType.Private || !GroupSettingMgr.Instance[MsgDTO.FromGroup].HasFunction(AIAttr.Name))
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
                Ai = AIAttr.Name,
                Command = "SignInOverride",
                GroupNum = MsgDTO.FromGroup
            });

            // 个人签到验证
            var personRecord = SignInPersonRecord.Get(MsgDTO.FromQQ);
            SignInGroupInfo ginfo;
            if (personRecord.GroupInfos.ContainsKey(MsgDTO.FromGroup.ToString()))
            {
                ginfo = personRecord.GroupInfos[MsgDTO.FromGroup.ToString()];
            }
            else
            {
                ginfo = new SignInGroupInfo();
                personRecord.GroupInfos.Add(MsgDTO.FromGroup.ToString(), ginfo);
            }

            Sign(ginfo, MsgDTO);
            personRecord.Update();
            return true;
        }

        private void Sign(SignInGroupInfo ginfo, MsgInformationEx MsgDTO)
        {
            if (ginfo.LastSignInDate == null || ginfo.LastSignInDate.Value.Date < DateTime.Today.AddDays(-1))
            {
                ginfo.SuccessiveDays = 0;
            }

            if (string.IsNullOrEmpty(SCacheService.Get<string>("SignInAcc")))
            {
                ginfo.SuccessiveDays++;
            }
            else
            {
                ginfo.SuccessiveDays += 2;
            }

            ginfo.LastSignInDate = DateTime.Today;
            var goldsGen = ginfo.SuccessiveDays > 5 ? 25 : ginfo.SuccessiveDays * 5;

            OSPerson.GoldIncome(MsgDTO.FromQQ, goldsGen);

            var msg = $"签到成功！你已连续签到 {ginfo.SuccessiveDays}天，获得 {goldsGen}金币！";
            if (ginfo.SuccessiveDays % 10 == 0)
            {
                var key = $"LimitBonus-{MsgDTO.FromQQ}";
                SCacheService.Cache(key, "nothing");

                msg += "\r恭喜你获得一次抽奖机会，快去试试吧（当日有效！）";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
        }

        [EnterCommand(Command = "今日签到内容",
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

    public class DailySignInCache
    {
        public long GroupNum { get; set; }

        public long QQNum { get; set; }

        public int SuccessiveSignDays { get; set; }

        public DateTime LastSignDate { get; set; }
    }
}
