using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AILib.Entities;

namespace AILib
{
    [AI(Name = "Publisher",
        Description = "AI for Publishing Developing Record.",
        IsAvailable = true,
        PriorityLevel = 11)]
    public class Publisher : AIBase
    {
        public Publisher(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
        }

        public override void Work()
        {
        }

        [EnterCommand(Command = "发布记录", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void PublishRec(PrivateMsgDTO MsgDTO)
        {
            PublishRec(MsgDTO.msg);
        }

        [EnterCommand(Command = "发布", SourceType = MsgType.Private, IsDeveloperOnly = true)]
        public void PublishTo(PrivateMsgDTO MsgDTO)
        {
            PublishTo(MsgDTO.msg);
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

        private void PublishTo(string msg)
        {
            string[] strs = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (strs == null || strs.Length != 2)
            {
                return;
            }

            string Rec = GetRecByIndex(strs[1]);
            long groupNum;
            if (!long.TryParse(strs[0], out groupNum))
            {
                return;
            }
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

        public string GetRecByIndex(string Index)
        {
            try
            {
                var query = DbMgr.Query<PublishRecordEntity>(p => p.Index == Index);
                if (query == null || query.Count() == 0)
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