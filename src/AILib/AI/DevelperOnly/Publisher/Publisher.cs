using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AILib.Entities;

namespace AILib
{
    [AI(
        Name = "Publisher",
        Description = "AI for Publishing Developing Record.",
        IsAvailable = true,
        PriorityLevel = 11
        )]
    public class Publisher : AIBase
    {
        public Publisher(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "发布记录",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "发布新版本内容，并留下记录，将返回一个记录编号",
            Syntax = "[内容]",
            Tag = "发布功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void PublishRec(PrivateMsgDTO MsgDTO, object[] param)
        {
            PublishRec(MsgDTO.msg);
        }

        [EnterCommand(
            Command = "发布",
            SourceType = MsgType.Private,
            IsDeveloperOnly = true,
            Description = "将指定编号的内容发布到指定群组，群组号0时为全部群组",
            Syntax = "[群组号] [记录编号]",
            Tag = "发布功能",
            SyntaxChecker = "LongAndAny"
            )]
        public void PublishTo(PrivateMsgDTO MsgDTO, object[] param)
        {
            string Rec = GetRecByIndex(param[1] as string);
            long groupNum = (long)param[0];
            if (groupNum == 0)
            {
                foreach (var group in ConfigDTO.AimGroups)
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO()
                    {
                        Aim = group,
                        Type = MsgType.Group,
                        Msg = Rec
                    });
                }
            }
            else
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO()
                {
                    Aim = groupNum,
                    Type = MsgType.Group,
                    Msg = Rec
                });
            }
        }

        private void PublishRec(string msg)
        {
            string Index = DateTime.Now.ToString("yyyyMMddHHmmss");

            try
            {
                DbMgr.Insert(new PublishRecordEntity()
                {
                    Id = Guid.NewGuid().ToString(),
                    Content = msg,
                    CreateTime = DateTime.Now,
                    Index = Index
                });

                Common.SendMsgToDeveloper($@"记录成功！序号为：{Index}");
            }
            catch (Exception ex)
            {
                Common.SendMsgToDeveloper(ex);
            }
        }

        public string GetRecByIndex(string Index)
        {
            try
            {
                var query = DbMgr.Query<PublishRecordEntity>(p => p.Index == Index);
                if (query.IsNullOrEmpty())
                {
                    return string.Empty;
                }

                return query.FirstOrDefault().Content;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}