using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Cache;

namespace Dolany.Ai.Core.Common
{
    public class ScriptSvc : IDependency
    {
        public WaiterSvc WaiterSvc { get; set; }

        public bool ProcessShow(MsgInformationEx MsgDTO, IEnumerable<string> Lines, string skipWord = "0")
        {
            MsgSender.PushMsg(MsgDTO, $"【友情提示】剧情演出中，你可以随时使用 “{skipWord}” 来跳过剧情！");
            var cache = new ConcurrentQueue<string>();
            var listenKey = WaiterSvc.ListenQQNum(MsgDTO.FromQQ, information => { cache.Enqueue(information.Msg); });
            foreach (var line in Lines)
            {
                if (CheckSkipWord(cache, skipWord))
                {
                    WaiterSvc.DislistenQQNum(MsgDTO.FromQQ, listenKey);
                    return false;
                }

                ProcessWord(MsgDTO, line);
            }

            WaiterSvc.DislistenQQNum(MsgDTO.FromQQ, listenKey);
            return true;
        }

        private bool CheckSkipWord(ConcurrentQueue<string> CacheQueue, string skipWord)
        {
            while (!CacheQueue.IsEmpty)
            {
                CacheQueue.TryDequeue(out var Word);
                if (Word == skipWord)
                {
                    return true;
                }
            }

            return false;
        }

        private void ProcessWord(MsgInformationEx MsgDTO, string Line)
        {
            var sleepTime = Line.Length / 2 * 1000;
            MsgSender.PushMsg(MsgDTO, Line);
            Thread.Sleep(sleepTime);
        }
    }
}
