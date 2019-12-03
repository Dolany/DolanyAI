using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware
{
    public class WSMgr
    {
        public static WSMgr Instance { get; } = new WSMgr();

        private readonly ConcurrentDictionary<string, WSClient> ClientsDic = new ConcurrentDictionary<string, WSClient>();

        private readonly ConcurrentDictionary<string, WaitingModel> WaitingDic = new ConcurrentDictionary<string, WaitingModel>();

        private List<BindAiModel> AllAis { get; }

        private WSMgr()
        {
            AllAis = CommonUtil.ReadJsonData_NamedList<BindAiModel>("BindAiData");

            foreach (var ai in AllAis)
            {
                ClientsDic.TryAdd(ai.Name, new WSClient($"ws://localhost:{ai.BindPort}/{ai.Name}/", ai.Name, MessageInvoke));
            }

            Scheduler.Instance.Add(SchedulerTimer.SecondlyInterval * Global.Config.ReconnectSecords, Reconnect);

            Global.MQSvc.StartReceive<MsgCommand>(CommandInvoke);
        }

        private void MessageInvoke(string bindAi, QQEventModel model)
        {
            Console.WriteLine(JsonConvert.SerializeObject(new {bindAi, model}));

            try
            {
                if (!string.IsNullOrEmpty(model.Id) && WaitingDic.TryRemove(model.Id, out var waiting))
                {
                    WaitingCallBack(waiting, model);
                    return;
                }

                switch (model.Event)
                {
                    case "message":
                    {
                        if (AllAis.Any(ai => ai.QQNum == model.Params.Qq))
                        {
                            return;
                        }

                        var info = new MsgInformation()
                        {
                            BindAi = bindAi,
                            FromGroup = long.TryParse(model.Params.Group, out var groupNum) ? groupNum : 0,
                            FromQQ = long.TryParse(model.Params.Qq, out var qqNum) ? qqNum : 0,
                            Information = InformationType.Message,
                            Msg = model.Params.Content
                        };
                        PublishInformation(info);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }

        private static void PublishInformation(MsgInformation info)
        {
            Global.MQSvc.Send(info, Global.Config.MQSendQueue);
        }

        private void Reconnect(object sender, ElapsedEventArgs args)
        {
            foreach (var wsClient in ClientsDic.Values.Where(client => !client.IsConnected))
            {
                wsClient.Connect();
            }
        }

        private static void WaitingCallBack(WaitingModel model, QQEventModel eventModel)
        {
            switch (model.Command)
            {
                case CommandType.GetGroupMemberInfo:
                {
                    var info = new MsgInformation()
                    {
                        Information = InformationType.CommandBack,
                        RelationId = model.RelationId,
                        Msg = JsonConvert.SerializeObject(eventModel.Result)
                    };
                    PublishInformation(info);
                    break;
                }
            }
        }

        private void CommandInvoke(MsgCommand command)
        {
            Console.WriteLine(JsonConvert.SerializeObject(command));
            switch (command.Command)
            {
                case CommandType.SendGroup:
                {
                    var model = new Dictionary<string, object>
                    {
                        {"id", command.Id },
                        {"method", "sendMessage"},
                        {
                            "params", new Dictionary<string, object>()
                            {
                                {"type", 2},
                                {"group", command.ToGroup.ToString()},
                                {"content", command.Msg}
                            }
                        }
                    };
                    Send(command.BindAi, model);
                    break;
                }

                case CommandType.SendPrivate:
                {
                    var model = new Dictionary<string, object>
                    {
                        {"id", command.Id },
                        {"method", "sendMessage"},
                        {
                            "params", new Dictionary<string, object>()
                            {
                                {"type", 1},
                                {"qq", command.ToQQ.ToString()},
                                {"content", command.Msg}
                            }
                        }
                    };
                    Send(command.BindAi, model);
                    break;
                }

                case CommandType.GetGroupMemberInfo:
                {
                    WaitingDic.TryAdd(command.Id, new WaitingModel() {BindAi = command.BindAi, Command = command.Command, RelationId = command.Id});

                    var model = new Dictionary<string, object>()
                    {
                        {"id", command.Id },
                        {"method", "getGroupMemberList" },
                        {"params", new Dictionary<string, object>()
                        {
                            {"group", command.Msg }
                        } }
                    };
                    Send(command.BindAi, model);
                    break;
                }

                case CommandType.ConnectionState:
                {
                    var stateDic = ClientsDic.ToDictionary(c => c.Key, c => c.Value.IsConnected);
                    var info = new MsgInformation()
                    {
                        Information = InformationType.CommandBack,
                        Msg = JsonConvert.SerializeObject(stateDic),
                        RelationId = command.Id
                    };

                    PublishInformation(info);
                    break;
                }
            }
        }

        private void Send(string bindAi, object msgModel)
        {
            if (ClientsDic.ContainsKey(bindAi) && ClientsDic[bindAi].IsConnected)
            {
                ClientsDic[bindAi].Send(JsonConvert.SerializeObject(msgModel));
            }
        }
    }

    public class WaitingModel
    {
        public string BindAi { get; set; }

        public CommandType Command { get; set; }

        public string RelationId { get; set; }
    }
}
