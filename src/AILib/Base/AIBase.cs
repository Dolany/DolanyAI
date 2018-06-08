using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace AILib
{
    public abstract class AIBase
    {
        public AIConfigDTO ConfigDTO { get; set; }

        public abstract void Work();

        public AIBase(AIConfigDTO ConfigDTO)
        {
            this.ConfigDTO = ConfigDTO;
        }

        public virtual void OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            Type t = this.GetType();
            foreach(var method in t.GetMethods())
            {
                foreach(var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute)))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgSourceType.Group)
                    {
                        break;
                    }
                    t.InvokeMember(method.Name,
                            BindingFlags.Public,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                }
            }
        }

        public virtual void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {

        }

        public virtual bool IsPrivateDeveloperOnly()
        {
            return false;
        }
    }
}
