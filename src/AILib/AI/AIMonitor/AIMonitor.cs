using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib
{
    [AI(Name = "AIMonitor", Description = "AI for Monitor AIs status.", IsAvailable = true)]
    public class AIMonitor : AIBase
    {
        public AIMonitor(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {

        }

        public override void Work()
        {
            
        }

        public override bool IsPrivateDeveloperOnly()
        {
            return true;
        }

        public override void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            base.OnPrivateMsgReceived(MsgDTO);

            if(!MsgDTO.msg.StartsWith("(debug)"))
            {
                return;
            }

            string EntrancePoint = MsgDTO.msg.Replace("(debug)", "").Trim();
            if(string.IsNullOrEmpty(EntrancePoint))
            {
                return;
            }

            InvodeDebug(EntrancePoint);
        }

        private void InvodeDebug(string EntrancePoint)
        {
            if(DebugMgr(EntrancePoint))
            {
                return;
            }

            if(DebugAIs(EntrancePoint))
            {
                return;
            }

            Common.SendMsgToDeveloper("未找到指定的debug入口！");
        }

        private bool DebugMgr(string EntrancePoint)
        {
            Type mgrType = typeof(AIMgr);
            foreach (var property in mgrType.GetProperties())
            {
                foreach (var attr in property.GetCustomAttributes(false))
                {
                    if (!(attr is AIDebugAttribute))
                    {
                        continue;
                    }

                    AIDebugAttribute debugAttr = attr as AIDebugAttribute;
                    if (debugAttr.EntrancePoint != EntrancePoint)
                    {
                        continue;
                    }

                    object obj = mgrType.InvokeMember(
                        property.Name,
                        System.Reflection.BindingFlags.GetProperty | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public,
                        null,
                        null,
                        null
                        );

                    Common.SendMsgToDeveloper(obj as string);
                    return true;
                }
            }

            return false;
        }

        private bool DebugAIs(string EntrancePoint)
        {
            return AIMgr.DebugAIs(EntrancePoint);
        }
    }
}
