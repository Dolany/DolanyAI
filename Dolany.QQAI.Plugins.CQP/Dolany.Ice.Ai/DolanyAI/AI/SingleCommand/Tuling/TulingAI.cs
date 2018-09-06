using System;
using System.Linq;
using System.Text;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(TulingAI),
        Description = "AI for Tuling Robot.",
        IsAvailable = true,
        PriorityLevel = 2
        )]
    public class TulingAI : AIBase
    {
        private const string RequestUrl = "http://openapi.tuling123.com/openapi/api/v2";
        private const string ApiKey = "fbeeef973da4480bb42dc10c45ba735b";
        private readonly int[] ErroCodes = { 5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008 };

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

            if (!MsgDTO.FullMsg.Contains(CodeApi.Code_SelfAt()))
            {
                return false;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(CodeApi.Code_SelfAt(), "");
            var response = RequestMsg(MsgDTO);
            if (string.IsNullOrEmpty(response))
            {
                return false;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = $"{CodeApi.Code_At(MsgDTO.FromQQ)} {response}"
            });
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
            if (response == null || ErroCodes.Contains(response.intent.code))
            {
                return string.Empty;
            }

            return ParseResponse(response);
        }

        private static PostReq_Param GetPostReq(ReceivedMsgDTO MsgDTO)
        {
            var imageInfo = ParseImgText(MsgDTO.FullMsg);
            var perception = string.IsNullOrEmpty(imageInfo) ? new perceptionData
            {
                inputText = new inputTextData
                {
                    text = MsgDTO.FullMsg
                }
            } : new perceptionData
            {
                inputImage = new inputImageData
                {
                    url = imageInfo
                }
            };

            var mi = Utility.GetMemberInfo(MsgDTO);
            if (mi == null)
            {
                return null;
            }

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
                        groupId = MsgDTO.FromGroup.ToString(),
                        userId = MsgDTO.FromQQ.ToString(),
                        userIdName = mi.Nickname
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
                        builder.Append(CodeApi.Code_Image(res.values.image));
                        break;

                    case "voice":
                        builder.Append(CodeApi.Code_Voice(res.values.voice));
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
            if (!msg.Contains("QQ:pic="))
            {
                return string.Empty;
            }

            try
            {
                var strs1 = msg.Split(new[] { "QQ:pic=" }, StringSplitOptions.RemoveEmptyEntries);
                var strs2 = strs1.Last().Split(']');
                var strs3 = strs2.First().Split('.');
                var imageGuid = strs3.First();

                var image = Utility.ReadImageCacheInfo(imageGuid);
                return image == null ? string.Empty : image.url;
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
    }
}