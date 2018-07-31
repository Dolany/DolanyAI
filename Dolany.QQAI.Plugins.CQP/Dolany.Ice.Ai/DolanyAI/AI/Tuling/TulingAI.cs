using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dolany.Ice.Ai.MahuaApis;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = "TulingAI",
        Description = "AI for Tuling Robot.",
        IsAvailable = true,
        PriorityLevel = 8
        )]
    public class TulingAI : AIBase
    {
        private string RequestUrl = "http://openapi.tuling123.com/openapi/api/v2";
        private const string ApiKey = "fbeeef973da4480bb42dc10c45ba735b";
        private int[] ErroCodes = { 5000, 6000, 4000, 4001, 4002, 4003, 4005, 4007, 4100, 4200, 4300, 4400, 4500, 4600, 4602, 7002, 8008 };

        public TulingAI()
            : base()
        {
            RuntimeLogger.Log("TulingAI started.");
        }

        public override void Work()
        {
        }

        public override bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (base.OnGroupMsgReceived(MsgDTO))
            {
                return true;
            }

            if (!MsgDTO.FullMsg.Contains(CodeApi.Code_SelfAt()))
            {
                return false;
            }

            MsgDTO.FullMsg = MsgDTO.FullMsg.Replace(CodeApi.Code_SelfAt(), "");
            string response = RequestMsg(MsgDTO);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = $"{CodeApi.Code_At(MsgDTO.FromQQ)} {response}"
            });
            return true;
        }

        private string RequestMsg(GroupMsgDTO MsgDTO)
        {
            PostReq_Param post = GetPostReq(MsgDTO);

            var response = RequestHelper.PostData<TulingResponseData>(post);
            if (response == null || ErroCodes.Contains(response.intent.code))
            {
                return string.Empty;
            }

            return ParseResponse(response);
        }

        private PostReq_Param GetPostReq(GroupMsgDTO MsgDTO)
        {
            PostReq_Param post = new PostReq_Param
            {
                InterfaceName = RequestUrl,
                data = new TulingRequestData
                {
                    reqType = 0,
                    perception = new perceptionData
                    {
                        inputText = new inputTextData
                        {
                            text = MsgDTO.FullMsg
                        }
                    },
                    userInfo = new userInfoData
                    {
                        apiKey = ApiKey,
                        groupId = MsgDTO.FromGroup.ToString(),
                        userId = MsgDTO.FromQQ.ToString(),
                        userIdName = Utility.GetMemberInfo(MsgDTO).Nickname
                    }
                }
            };

            return post;
        }

        private string ParseResponse(TulingResponseData response)
        {
            string result = string.Empty;
            foreach (var res in response.results)
            {
                switch (res.resultType)
                {
                    case "text":
                        result += res.values.text;
                        break;

                    case "image":
                        result += CodeApi.Code_Image(res.values.image);
                        break;

                    case "voice":
                        result += CodeApi.Code_Voice(res.values.voice);
                        break;

                    case "url":
                        result += $" {res.values.url} ";
                        break;
                }
            }
            return result;
        }
    }
}