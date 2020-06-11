using System;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Doremi.Ai.Record.Hello
{
    public class HelloAI : AIBase
    {
        public override string AIName { get; set; } = "打招呼";
        public override string Description { get; set; } = "AI for Saying Hello to you at everyday you say at the first time in one group.";
        public override AIPriority PriorityLevel { get; } = AIPriority.High;
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.打招呼功能;

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

            var result = ProcessHello(MsgDTO);
            result |= ProcessMultiMediaHello(MsgDTO);

            if (result)
            {
                AIAnalyzer.AddCommandCount(new CmdRec()
                {
                    FunctionalAi = AIName,
                    Command = "HelloOverride",
                    GroupNum = MsgDTO.FromGroup,
                    BindAi = MsgDTO.BindAi
                });
            }

            return false;
        }

        private static bool ProcessHello(MsgInformationEx MsgDTO)
        {
            var key = $"Hello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var cache = RapidCacher.GetCache(key, CommonUtil.UntilTommorow(), () => HelloRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ));
            if (cache == null)
            {
                return false;
            }

            MsgSender.PushMsg(MsgDTO, $"{CodeApi.Code_At(MsgDTO.FromQQ)} {cache.Content}");
            return true;
        }

        private static bool ProcessMultiMediaHello(MsgInformationEx MsgDTO)
        {
            var key = $"MultiMediaHello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var cache = RapidCacher.GetCache(key, CommonUtil.UntilTommorow(),
                () => CommonUtil.ReadJsonData_NamedList<MultiMediaHelloRecord>("Doremi/MultiMediaHelloData").FirstOrDefault(h => h.QQNum == MsgDTO.FromQQ));
            if (cache == null)
            {
                return false;
            }

            SendMultiMediaHello(MsgDTO, cache);
            return true;
        }

        private static void SendMultiMediaHello(MsgInformationEx MsgDTO, MultiMediaHelloRecord hello)
        {
            var path = hello.Location switch
            {
                ResourceLocationType.LocalAbsolute => hello.ContentPath,
                ResourceLocationType.LocalRelative => new FileInfo(hello.ContentPath).FullName,
                ResourceLocationType.Network => hello.ContentPath,
                _ => ""
            };

            var msg = hello.MediaType switch
            {
                MultiMediaResourceType.Image => CodeApi.Code_Image(path),
                MultiMediaResourceType.Voice => CodeApi.Code_Voice(path),
                _ => ""
            };

            MsgSender.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(ID = "HelloAI_SaveHelloContent",
            Command = "打招呼设定",
            Description = "设定每天打招呼的内容",
            SyntaxHint = "[设定内容]",
            SyntaxChecker = "Any")]
        public bool SaveHelloContent(MsgInformationEx MsgDTO, object[] param)
        {
            var content = param[0] as string;

            var helloRecord = HelloRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (helloRecord == null)
            {
                helloRecord = new HelloRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = MsgDTO.FromQQ,
                    Content = content
                };
                helloRecord.Insert();
            }
            else
            {
                helloRecord.Content = content;
                helloRecord.Update();
            }

            MsgSender.PushMsg(MsgDTO, "招呼内容设定成功");
            return true;
        }

        [EnterCommand(ID = "HelloAI_SayHello",
            Command = "打招呼",
            Description = "发送打招呼的内容")]
        public bool SayHello(MsgInformationEx MsgDTO, object[] param)
        {
            var query = HelloRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (query == null)
            {
                MsgSender.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, $"{CodeApi.Code_At(MsgDTO.FromQQ)} {query.Content}");
            return true;
        }

        [EnterCommand(ID = "HelloAI_DeleteHello",
            Command = "打招呼删除",
            Description = "删除打招呼的内容")]
        public bool DeleteHello(MsgInformationEx MsgDTO, object[] param)
        {
            var hello = HelloRecord.Get(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (hello == null)
            {
                MsgSender.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            hello.Remove();

            MsgSender.PushMsg(MsgDTO, "删除成功！");
            return true;
        }

        [EnterCommand(ID = "HelloAI_OnStage",
            Command = "登场",
            Description = "显示登场特效")]
        public bool OnStage(MsgInformationEx MsgDTO, object[] param)
        {
            if (ProcessMultiMediaHello(MsgDTO))
            {
                return true;
            }

            MsgSender.PushMsg(MsgDTO, "你还没有任何登场特效！");
            return false;
        }
    }
}
