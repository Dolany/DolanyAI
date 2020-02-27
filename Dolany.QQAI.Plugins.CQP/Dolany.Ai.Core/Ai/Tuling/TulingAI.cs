using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Model;
using Dolany.Ai.Core.Model.Tuling;
using Dolany.Ai.Core.Net;
using Dolany.Database.Ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Dolany.Ai.Core.Ai.SingleCommand.Tuling
{
    public class TulingAI : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "图灵";

        public override string Description { get; set; } = "AI for Tuling Robot.";

        public override AIPriority PriorityLevel { get;} = AIPriority.SuperLow;

        private readonly string RequestUrl = Global.DefaultConfig.TulingRequestUrl;
        private List<TulingConfigModel> ApiKeys = new List<TulingConfigModel>();
        private const int TulingDailyLimit = 20;

        private int CurTulingIndex;

        private readonly int[] ErroCodes =
            {
                5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008
            };

        public BindAiSvc BindAiSvc { get; set; }

        public void RefreshData()
        {
            ApiKeys = CommonUtil.ReadJsonData_NamedList<TulingConfigModel>("TulingData");
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Group && BindAiSvc.AllAiNums.All(p => !MsgDTO.FullMsg.Contains(CodeApi.Code_At(p))))
            {
                return false;
            }

            var stateCache = AliveStateSvc.GetState(MsgDTO.FromGroup, MsgDTO.FromQQ);
            if (stateCache != null)
            {
                MsgSender.PushMsg(MsgDTO, $"你已经死了({stateCache.Name})！复活时间：{stateCache.RebornTime:yyyy-MM-dd HH:mm:ss}", true);
                return false;
            }

            var limitRecord = DailyLimitRecord.Get(MsgDTO.FromQQ, "Tuling");
            if (!limitRecord.Check(TulingDailyLimit))
            {
                MsgSender.PushMsg(MsgDTO, "今天太累了，明天再找我说话吧~", MsgDTO.Type == MsgType.Group);
                return false;
            }
            limitRecord.Cache();
            limitRecord.Update();

            foreach (var aiNum in BindAiSvc.AllAiNums)
            {
                MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(CodeApi.Code_At(aiNum), string.Empty);
            }

            string response = null;
            foreach (var tuling in ApiKeys.Select(_ => ApiKeys[CurTulingIndex]))
            {
                response = RequestMsg(MsgDTO, tuling.ApiKey);
                if (!string.IsNullOrEmpty(response))
                {
                    break;
                }

                CurTulingIndex = (CurTulingIndex + 1) % ApiKeys.Count;
            }

            if (string.IsNullOrEmpty(response))
            {
                MsgSender.PushMsg(MsgDTO, "今天太累了，明天再找我说话吧~", MsgDTO.Type == MsgType.Group);
                return false;
            }

            AIAnalyzer.AddCommandCount(new CmdRec()
            {
                FunctionalAi = AIName,
                Command = "TulingOverride",
                GroupNum = MsgDTO.FromGroup,
                BindAi = MsgDTO.BindAi
            });
            MsgSender.PushMsg(MsgDTO, response, MsgDTO.Type == MsgType.Group);
            return true;
        }

        private string RequestMsg(MsgInformationEx MsgDTO, string ApiKey)
        {
            var post = GetPostReq(MsgDTO, ApiKey);
            if (post == null)
            {
                return string.Empty;
            }

            var response = RequestHelper.PostData<TulingResponseData>(post);
            if (response == null || ErroCodes.Contains(response.Intent.Code))
            {
                return string.Empty;
            }

            return ParseResponse(response);
        }

        private PostReq_Param GetPostReq(MsgInformationEx MsgDTO, string ApiKey)
        {
            var bindAi = BindAiSvc[MsgDTO.BindAi];
            var imageInfo = ParseImgText(MsgDTO.FullMsg, bindAi.ImagePath);
            var perception = string.IsNullOrEmpty(imageInfo)
                ? new perceptionData
                {
                    InputText = new inputTextData
                    {
                        Text = MsgDTO.FullMsg
                    }
                }
                : new perceptionData
                {
                    InputImage = new inputImageData
                    {
                        Url = imageInfo
                    }
                };

            var post = new PostReq_Param
            {
                InterfaceName = RequestUrl,
                data = new TulingRequestData
                {
                    ReqType = 0,
                    Perception = perception,
                    UserInfo = new userInfoData
                    {
                        ApiKey = ApiKey,
                        UserId = MsgDTO.FromQQ.ToString()
                    }
                }
            };

            return post;
        }

        private static string ParseResponse(TulingResponseData response)
        {
            var result = string.Empty;
            var builder = new StringBuilder();
            builder.Append(result);
            foreach (var res in response.Results)
            {
                switch (res.ResultType)
                {
                    case "text":
                        builder.Append(res.Values.Text);
                        break;

                    case "image":
                        builder.Append(CodeApi.Code_Image(res.Values.Image));
                        break;

                    case "voice":
                        builder.Append(CodeApi.Code_Voice(res.Values.Voice));
                        break;

                    case "url":
                        builder.Append($" {res.Values.Url} ");
                        break;
                }
            }

            result = builder.ToString();
            return result;
        }

        private static string ParseImgText(string msg, string imagePath)
        {
            try
            {
                var imageGuid = Utility.ParsePicGuid(msg);

                var image = Utility.ReadImageCacheInfo(imageGuid, imagePath);
                return image == null ? string.Empty : image.url;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
