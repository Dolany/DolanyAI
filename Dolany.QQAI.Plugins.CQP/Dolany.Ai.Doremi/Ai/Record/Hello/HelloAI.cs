using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database;
using Dolany.Database.Ai;
using Dolany.Database.Sqlite;
using Dolany.Database.Sqlite.Model;

namespace Dolany.Ai.Doremi.Ai.Record.Hello
{
    public class HelloAI : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "打招呼";
        public override string Description { get; set; } = "AI for Saying Hello to you at everyday you say at the first time in one group.";
        public override AIPriority PriorityLevel { get; } = AIPriority.High;
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.打招呼功能;

        private List<HelloRecord> HelloList = new List<HelloRecord>();
        private List<MultiMediaHelloRecord> MultiMediaHelloList = new List<MultiMediaHelloRecord>();

        public void RefreshData()
        {
            var Groups = Global.AllGroupsDic.Keys.ToArray();
            HelloList = MongoService<HelloRecord>.Get(p => Groups.Contains(p.GroupNum));
            MultiMediaHelloList = CommonUtil.ReadJsonData_NamedList<MultiMediaHelloRecord>("MultiMediaHelloData");
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

        private bool ProcessHello(MsgInformationEx MsgDTO)
        {
            var key = $"Hello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var response = SCacheService.Get<HelloCache>(key);
            if (response != null)
            {
                return false;
            }

            var hello = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (hello == null)
            {
                return false;
            }

            MsgSender.PushMsg(MsgDTO, $"{CodeApi.Code_At(MsgDTO.FromQQ)} {hello.Content}");
            var model = new HelloCache
            {
                GroupNum = MsgDTO.FromGroup,
                LastUpdateTime = DateTime.Now,
                QQNum = MsgDTO.FromQQ
            };
            SCacheService.Cache(key, model);
            return true;
        }

        private bool ProcessMultiMediaHello(MsgInformationEx MsgDTO)
        {
            var key = $"MultiMediaHello-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}";
            var response = SCacheService.Get<MultiMediaCache>(key);
            if (response != null)
            {
                return false;
            }

            var hello = MultiMediaHelloList.FirstOrDefault(p => p.QQNum == MsgDTO.FromQQ);
            if (hello == null)
            {
                return false;
            }

            SendMultiMediaHello(MsgDTO, hello);
            var model = new MultiMediaCache
            {
                QQNum = MsgDTO.FromQQ,
                RecordID = hello.Name
            };
            SCacheService.Cache(key, model);
            return true;
        }

        private void SendMultiMediaHello(MsgInformationEx MsgDTO, MultiMediaHelloRecord hello)
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

            var query = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (query == null)
            {
                var hello = new HelloRecord
                {
                    Id = Guid.NewGuid().ToString(),
                    GroupNum = MsgDTO.FromGroup,
                    QQNum = MsgDTO.FromQQ,
                    Content = content
                };
                MongoService<HelloRecord>.Insert(hello);

                HelloList.Add(hello);
            }
            else
            {
                query.Content = content;
                MongoService<HelloRecord>.Update(query);
            }

            MsgSender.PushMsg(MsgDTO, "招呼内容设定成功");
            return true;
        }

        [EnterCommand(ID = "HelloAI_SayHello",
            Command = "打招呼",
            Description = "发送打招呼的内容")]
        public bool SayHello(MsgInformationEx MsgDTO, object[] param)
        {
            var query = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
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
            var query = HelloList.FirstOrDefault(h => h.GroupNum == MsgDTO.FromGroup && h.QQNum == MsgDTO.FromQQ);
            if (query == null)
            {
                MsgSender.PushMsg(MsgDTO, "你还没有设定过招呼内容哦~");
                return false;
            }

            MongoService<HelloRecord>.Delete(query);
            this.HelloList.Remove(query);

            MsgSender.PushMsg(MsgDTO, "删除成功！");
            return true;
        }

        [EnterCommand(ID = "HelloAI_OnStage",
            Command = "登场",
            Description = "显示登场特效")]
        public bool OnStage(MsgInformationEx MsgDTO, object[] param)
        {
            var hello = MultiMediaHelloList.FirstOrDefault(p => p.QQNum == MsgDTO.FromQQ);
            if (hello == null)
            {
                MsgSender.PushMsg(MsgDTO, "你还没有任何登场特效！");
                return false;
            }
            SendMultiMediaHello(MsgDTO, hello);

            return true;
        }
    }
}
