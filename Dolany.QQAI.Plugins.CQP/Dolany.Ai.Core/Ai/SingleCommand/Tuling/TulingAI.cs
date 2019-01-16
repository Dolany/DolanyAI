using Dolany.Database.Redis;
using Dolany.Database.Sqlite;

namespace Dolany.Ai.Core.Ai.SingleCommand.Tuling
{
    using System;
    using System.Linq;
    using System.Text;

    using Base;

    using Cache;

    using Common;

    using Dolany.Ai.Common;
    using Dolany.Ai.Core.Model.Tuling;
    using Dolany.Database;

    using Model;

    using Net;

    using static Common.Utility;

    using static API.CodeApi;

    [AI(
        Name = nameof(TulingAI),
        Description = "AI for Tuling Robot.",
        IsAvailable = true,
        PriorityLevel = 2)]
    public class TulingAI : AIBase
    {
        private readonly string RequestUrl = CommonUtil.GetConfig("TulingRequestUrl");
        private readonly string ApiKey = CommonUtil.GetConfig("TulingApiKey");

        private readonly int[] ErroCodes =
            {
                5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008
            };

        public TulingAI()
        {
            RuntimeLogger.Log("TulingAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnMsgReceived(MsgInformationEx MsgDTO)
        {
            if (base.OnMsgReceived(MsgDTO))
            {
                return true;
            }

            if (MsgDTO.Type == MsgType.Group &&
                !MsgDTO.FullMsg.Contains(Code_SelfAt()))
            {
                return false;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(Code_SelfAt(), string.Empty);

            var cacheResponse = SqliteCacheService.Get<string>("QuestionnaireDuring-QuestionnaireDuring");

            if (cacheResponse != null)
            {
                Questionnaire(MsgDTO);
                return true;
            }

            var response = RequestMsg(MsgDTO);
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            MsgSender.Instance.PushMsg(MsgDTO, response, true);
            return true;
        }

        private void Questionnaire(MsgInformationEx MsgDTO)
        {
            // todo
        }

        private string RequestMsg(MsgInformationEx MsgDTO)
        {
            var post = GetPostReq(MsgDTO);
            if (post == null)
            {
                return string.Empty;
            }

            var response = RequestHelper.PostData<TulingResponseData>(post);
            if (response == null ||
                ErroCodes.Contains(response.Intent.Code))
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
                        builder.Append(Code_Image(res.Values.Image));
                        break;

                    case "voice":
                        builder.Append(Code_Voice(res.Values.Voice));
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

        [EnterCommand(
            Command = "新增语料",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "新增私有语料",
            Syntax = "[问题] [答案]",
            Tag = "图灵功能",
            SyntaxChecker = "Word Word",
            IsPrivateAvailable = true)]
        public void AddPrivateQA(MsgInformationEx MsgDTO, object[] param)
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
