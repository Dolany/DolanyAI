using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.WSMidware.CommandResolver;
using Dolany.Ai.WSMidware.MessageResolver;
using Dolany.Ai.WSMidware.Models;
using Newtonsoft.Json;

namespace Dolany.Ai.WSMidware
{
    public class WSMgr
    {
        public static WSMgr Instance { get; } = new WSMgr();

        public readonly ConcurrentDictionary<string, WSClient> ClientsDic = new ConcurrentDictionary<string, WSClient>();

        public readonly ConcurrentDictionary<string, WaitingModel> WaitingDic = new ConcurrentDictionary<string, WaitingModel>();

        public List<BindAiModel> AllAis { get; }

        private readonly List<ICmdResovler> CommandResolvers;
        private readonly List<IMsgResolver> MessageResolvers;

        private WSMgr()
        {
            AllAis = CommonUtil.ReadJsonData_NamedList<BindAiModel>("BindAiData");

            foreach (var ai in AllAis)
            {
                ClientsDic.TryAdd(ai.Name, new WSClient($"ws://localhost:{ai.BindPort}/{ai.Name}/", ai.Name, MessageInvoke));
            }

            Scheduler.Instance.Add(SchedulerTimer.SecondlyInterval * Global.Config.ReconnectSecords, Reconnect);

            CommandResolvers = CommonUtil.LoadAllInstanceFromInterface<ICmdResovler>();
            MessageResolvers = CommonUtil.LoadAllInstanceFromInterface<IMsgResolver>();

            Global.MQSvc.StartReceive<MsgCommand>(CommandInvoke);
        }

        private void MessageInvoke(string bindAi, QQEventModel model)
        {
            Console.WriteLine($"{bindAi}:{JsonConvert.SerializeObject(model)}");

            try
            {
                if (!string.IsNullOrEmpty(model.Id) && WaitingDic.TryRemove(model.Id, out var waiting))
                {
                    WaitingCallBack(waiting, model);
                    return;
                }

                var resolver = MessageResolvers.FirstOrDefault(p => p.MsgEvent == model.Event);
                resolver?.Resolver(bindAi, model);
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
            }
        }

        public static void PublishInformation(MsgInformation info)
        {
            Global.MQSvc.Send(info, Global.Config.MQSendQueue);
        }

        public static void OnConnectStateChanged(string bindAi, bool isConnected)
        {
            PublishInformation(new MsgInformation()
            {
                BindAi = bindAi,
                Msg = isConnected.ToString()
            });
        }

        private void Reconnect(object sender, ElapsedEventArgs args)
        {
            foreach (var wsClient in ClientsDic.Values.Where(client => !client.IsConnected))
            {
                wsClient.Connect();
            }
        }

        private void WaitingCallBack(WaitingModel model, QQEventModel eventModel)
        {
            var resolver = CommandResolvers.FirstOrDefault(p => p.CommandType == model.Command);
            resolver?.CallBack(model, eventModel);
        }

        private void CommandInvoke(MsgCommand command)
        {
            Console.WriteLine(JsonConvert.SerializeObject(command));
            var resolver = CommandResolvers.FirstOrDefault(p => p.CommandType == command.Command);
            resolver?.Resolve(command);
        }

        public void Send(string bindAi, object msgModel)
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
