﻿using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.Game.SignIn
{
    public abstract class SignInBaseAI : AIBase
    {
        public override string AIName { get; set; } = "签到";

        public override string Description { get; set; } = "AI for Everyday Signing In";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.签到功能;

        public CrossWorldAiSvc CrossWorldAiSvc { get; set; }

        private Dictionary<long, SignInGroupRecord> GroupSignInDic = new Dictionary<long, SignInGroupRecord>();

        public override void Initialization()
        {
            base.Initialization();

            var records = MongoService<SignInGroupRecord>.Get();
            GroupSignInDic = records.ToDictionary(p => p.GroupNum, p => p);
        }

        [EnterCommand(ID = "SignInAI_SetSignContent",
            Command = "签到",
            AuthorityLevel = AuthorityLevel.管理员,
            Description = "设置签到内容(不能与系统自带命令重复)",
            SyntaxHint = "[签到内容]",
            SyntaxChecker = "Word")]
        public bool SetSignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            if (content != "签到" && CrossWorldAiSvc[MsgDTO.FromGroup].AllAvailableGroupCommands.Any(comm => comm.Command == content))
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

            if (MsgDTO.Type == MsgType.Private)
            {
                return false;
            }

            // 群组签到验证
            var groupInfo = GroupSignInDic.ContainsKey(MsgDTO.FromGroup) ? GroupSignInDic[MsgDTO.FromGroup] : null;
            var signInContent = groupInfo == null ? "签到" : groupInfo.Content;

            if (MsgDTO.FullMsg != signInContent)
            {
                if (MsgDTO.FullMsg == "签到")
                {
                    MsgSender.PushMsg(MsgDTO, "请使用 今日签到内容 命令获取今天的签到口令！");
                }
                return false;
            }

            AIAnalyzer.AddCommandCount(new CmdRec()
            {
                FunctionalAi = AIName,
                Command = "SignInOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });

            // 个人签到验证
            if (SignInSuccessiveRecord.IsTodaySigned(MsgDTO.FromGroup, MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你今天已经签过到啦！");
                return true;
            }

            Sign(MsgDTO);
            return true;
        }

        protected abstract void Sign(MsgInformationEx MsgDTO);

        [EnterCommand(ID = "SignInAI_TodaySignContent",
            Command = "今日签到内容",
            Description = "获取今日签到内容")]
        public bool TodaySignContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = GroupSignInDic.ContainsKey(MsgDTO.FromGroup) ? GroupSignInDic[MsgDTO.FromGroup].Content : "签到";
            MsgSender.PushMsg(MsgDTO, $"今日签到内容是：{content}");
            return true;
        }
    }
}
