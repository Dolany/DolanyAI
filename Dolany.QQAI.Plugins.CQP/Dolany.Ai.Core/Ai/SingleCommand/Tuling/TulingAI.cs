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
    using Database;
    using Dolany.Database.Ai;
    using Database.Sqlite.Model;
    using Database.Sqlite;

    using Model;

    using Net;

    using static Common.Utility;

    using static API.CodeApi;

    [AI(
        Name = "图灵",
        Description = "AI for Tuling Robot.",
        Enable = true,
        PriorityLevel = 2)]
    public class TulingAI : AIBase
    {
        private readonly string RequestUrl = Configger.Instance["TulingRequestUrl"];
        private readonly string ApiKey = Configger.Instance["TulingApiKey"];
        private const int QLimit = 5;

        private readonly int[] ErroCodes =
            {
                5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008
            };

        private string ResponseWord
        {
            get
            {
                var words = new[]
                                {
                                    "感谢你的支持！冰冰会变得更强的！", "嗯嗯~冰冰听到啦~",
                                    "瓦卡利马西塔~", "汝的诉求，余已经听闻", "はい！マスター！",
                                    "已收录AI记忆节点", "（拿出小本本记上）", "哇~这就是你的愿望吗？"
                                };

                return words[CommonUtil.RandInt(words.Length)];
            }
        }

        public override void Initialization()
        {
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

            if (!MsgDTO.FullMsg.Contains(Code_SelfAt()))
            {
                return false;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(Code_SelfAt(), string.Empty);

            var cacheResponse = SCacheService.Get<string>("QuestionnaireDuring-QuestionnaireDuring");

            if (cacheResponse != null)
            {
                Questionnaire(MsgDTO, cacheResponse);
                return true;
            }

            var response = RequestMsg(MsgDTO);
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            AIAnalyzer.AddCommandCount();
            MsgSender.Instance.PushMsg(MsgDTO, response, true);
            return true;
        }

        private void Questionnaire(MsgInformationEx MsgDTO, string QNo)
        {
            var cacheResponse = SCacheService.Get<QuestionnaireLimitCache>($"QuestionnaireLimit-{MsgDTO.FromQQ}");
            if (cacheResponse != null && cacheResponse.Count > QLimit)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"每天最多可以反馈{QLimit}哦~", true);
                return;
            }

            if (string.IsNullOrEmpty(MsgDTO.FullMsg))
            {
                return;
            }

            MongoService<QuestionnaireRecord>.Insert(new QuestionnaireRecord
            {
                GroupNum = MsgDTO.FromGroup,
                QQNum = MsgDTO.FromQQ,
                QNo = QNo,
                UpdateTime = DateTime.Now,
                Content = MsgDTO.FullMsg
            });

            MsgSender.Instance.PushMsg(MsgDTO, ResponseWord, true);
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
    }
}
