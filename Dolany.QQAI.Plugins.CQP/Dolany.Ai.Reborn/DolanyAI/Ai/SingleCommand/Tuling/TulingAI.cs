﻿using System;
using System.Linq;
using System.Text;
using Dolany.Ai.Reborn.DolanyAI.Base;
using Dolany.Ai.Reborn.DolanyAI.Cache;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.DTO;
using Dolany.Ai.Reborn.DolanyAI.Model;
using Dolany.Ai.Reborn.DolanyAI.Model.Tuling;
using Dolany.Ai.Reborn.DolanyAI.Net;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;
using static Dolany.Ai.Reborn.MahuaApis.CodeApi;

namespace Dolany.Ai.Reborn.DolanyAI.Ai.SingleCommand.Tuling
{
    [AI(
        Name = nameof(TulingAI),
        Description = "AI for Tuling Robot.",
        IsAvailable = true,
        PriorityLevel = 2
    )]
    public class TulingAI : AIBase
    {
        private readonly string RequestUrl = GetConfig("TulingRequestUrl");
        private readonly string ApiKey = GetConfig("TulingApiKey");

        private readonly int[] ErroCodes =
            {5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008};

        //private readonly string TulingImportUrl = Utility.GetConfig(nameof(TulingImportUrl));

        public TulingAI()
        {
            RuntimeLogger.Log("TulingAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.MsgType == MsgType.Group &&
                !MsgDTO.FullMsg.Contains(Code_SelfAt()))
            {
                return false;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(Code_SelfAt(), "");
            var response = RequestMsg(MsgDTO);
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, response, true);
            return true;
        }

        private string RequestMsg(ReceivedMsgDTO MsgDTO)
        {
            var post = GetPostReq(MsgDTO);
            if (post == null)
            {
                return "";
            }

            var response = RequestHelper.PostData<TulingResponseData>(post);
            if (response == null ||
                ErroCodes.Contains(response.intent.code))
            {
                return string.Empty;
            }

            return ParseResponse(response);
        }

        private PostReq_Param GetPostReq(ReceivedMsgDTO MsgDTO)
        {
            var imageInfo = ParseImgText(MsgDTO.FullMsg);
            var perception = string.IsNullOrEmpty(imageInfo)
                ? new perceptionData
                {
                    inputText = new inputTextData
                    {
                        text = MsgDTO.FullMsg
                    }
                }
                : new perceptionData
                {
                    inputImage = new inputImageData
                    {
                        url = imageInfo
                    }
                };

            var post = new PostReq_Param
            {
                InterfaceName = RequestUrl,
                data = new TulingRequestData
                {
                    reqType = 0,
                    perception = perception,
                    userInfo = new userInfoData
                    {
                        apiKey = ApiKey,
                        userId = MsgDTO.FromQQ.ToString()
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
            foreach (var res in response.results)
            {
                switch (res.resultType)
                {
                    case "text":
                        builder.Append(res.values.text);
                        break;

                    case "image":
                        builder.Append(Code_Image(res.values.image));
                        break;

                    case "voice":
                        builder.Append(Code_Voice(res.values.voice));
                        break;

                    case "url":
                        builder.Append($" {res.values.url} ");
                        break;

                    default:
                        throw new Exception("Unexpected Case");
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

        [EnterCommand(
            Command = "新增语料",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "新增私有语料",
            Syntax = "[问题] [答案]",
            Tag = "图灵功能",
            SyntaxChecker = "Word Word",
            IsPrivateAvailabe = true
        )]
        public void AddPrivateQA(ReceivedMsgDTO MsgDTO, object[] param)
        {
            MsgSender.Instance.PushMsg(MsgDTO, "暂未开放！");
            //var question = param[0] as string;
            //var answer = param[1] as string;

            //var post = new PostReq_Param
            //{
            //    InterfaceName = TulingImportUrl,
            //    data = new TulingImportRequestData
            //    {
            //        apikey = ApiKey,
            //        data = new TulingImportRequestDataData
            //        {
            //            list = new[]
            //            {
            //                new TulingImportRequestDataQA
            //                {
            //                    question = question,
            //                    answer = answer
            //                }
            //            }
            //        }
            //    }
            //};

            //var response = RequestHelper.PostData<TulingImportResponseData>(post);
            //if (response == null ||
            //    response.code != 0)
            //{
            //    MsgSender.Instance.PushMsg(MsgDTO, "新增失败！");
            //}

            //MsgSender.Instance.PushMsg(MsgDTO, "新增成功！");
        }
    }
}