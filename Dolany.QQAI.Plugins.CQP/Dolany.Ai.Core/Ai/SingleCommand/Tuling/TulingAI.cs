﻿namespace Dolany.Ai.Core.Ai.SingleCommand.Tuling
{
    using System;
    using System.Linq;
    using System.Text;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Ai.Core.Model.Tuling;
    using Model;

    using Net;

    using static Common.Utility;

    [AI(
        Name = "图灵",
        Description = "AI for Tuling Robot.",
        Enable = true,
        PriorityLevel = 2)]
    public class TulingAI : AIBase
    {
        private readonly string RequestUrl = Configger.Instance["TulingRequestUrl"];
        private readonly string ApiKey = Configger.Instance["TulingApiKey"];

        private readonly int[] ErroCodes =
            {
                5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008
            };

        public override void Initialization()
        {
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Group && !MsgDTO.FullMsg.Contains(CodeApi.Code_At(Global.SelfQQNum)))
            {
                return false;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(CodeApi.Code_At(Global.SelfQQNum), string.Empty);

            var response = RequestMsg(MsgDTO);
            if (string.IsNullOrEmpty(response))
            {
                MsgSender.PushMsg(MsgDTO, "...", MsgDTO.Type == MsgType.Group);
                return false;
            }

            AIAnalyzer.AddCommandCount(new CommandAnalyzeDTO()
            {
                Ai = AIAttr.Name,
                Command = "TulingOverride",
                GroupNum = MsgDTO.FromGroup
            });
            MsgSender.PushMsg(MsgDTO, response, MsgDTO.Type == MsgType.Group);
            return true;
        }

        private string RequestMsg(MsgInformationEx MsgDTO)
        {
            var post = GetPostReq(MsgDTO);
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

        private PostReq_Param GetPostReq(MsgInformationEx MsgDTO)
        {
            var imageInfo = ParseImgText(MsgDTO.FullMsg);
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

        private static string ParseImgText(string msg)
        {
            try
            {
                var imageGuid = ParsePicGuid(msg);

                var image = ReadImageCacheInfo(imageGuid);
                return image == null ? string.Empty : image.url;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}
