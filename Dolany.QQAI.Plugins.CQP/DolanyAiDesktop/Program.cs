using System;
using System.Linq;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Database.Sqlite;
using Dolany.WorldLine.Standard;

namespace DolanyAiDesktop
{
    class Program
    {
        private static readonly IWorldLine[] worlds = {new StandardWorldLine(),};
        private static IWorldLine DefaultWorldLine => worlds.First(w => w.IsDefault);

        static void Main(string[] args)
        {
            Global.MsgPublish = PrintMsg;
            SFixedSetService.SetMaxCount("PicCache", Global.DefaultConfig.MaxPicCacheCount);
            Waiter.Instance.MsgReceivedCallBack = OnMsgReceived;
            Waiter.Instance.MoneyReceivedCallBack = OnMoneyReceived;
            AIAnalyzer.Sys_StartTime = DateTime.Now;

            foreach (var worldLine in worlds)
            {
                worldLine.Load();
            }

            Waiter.Instance.Listen();

            var command = Console.ReadLine();
            while (command != "Exit")
            {
                command = Console.ReadLine();
            }
        }

        private static void PrintMsg(string Msg)
        {
            Console.WriteLine(Msg);
        }

        private static void OnMsgReceived(MsgInformation MsgDTO)
        {
            var worldLine = JudgeWorldLine(MsgDTO.FromGroup, MsgDTO.FromQQ);
            var world = worlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMsgReceived(MsgDTO);
        }

        private static void OnMoneyReceived(ChargeModel model)
        {
            var worldLine = JudgeWorldLine(0, model.QQNum);
            var world = worlds.FirstOrDefault(p => p.Name == worldLine);
            world?.OnMoneyReceived(model);
        }

        private static string JudgeWorldLine(long groupNum, long QQNum)
        {
            if (groupNum == 0)
            {
                return JudgePersonalWorldLine(QQNum);
            }

            var group = GroupSettingMgr.Instance[groupNum];
            if (group == null)
            {
                return DefaultWorldLine.Name;
            }

            return string.IsNullOrEmpty(group.WorldLine) ? DefaultWorldLine.Name : group.WorldLine;
        }

        private static string JudgePersonalWorldLine(long QQNum)
        {
            return DefaultWorldLine.Name;
        }
    }
}
