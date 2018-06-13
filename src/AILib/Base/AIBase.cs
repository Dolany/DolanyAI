﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using AILib.Entities;

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
            if (MsgDTO.fromQQ < 0)
            {
                var entities = DbMgr.Query<QQNumReflectEntity>(q => q.FakeNum == MsgDTO.fromQQ);
                if(entities == null || entities.Count() == 0)
                {
                    DbMgr.Insert(new LogEntity()
                    {
                        Id = Guid.NewGuid().ToString(),
                        CreateTime = DateTime.Now,
                        Content = $"诶呀呀！粗错辣！{MsgDTO.fromGroup}:{MsgDTO.fromQQ}"
                    });
                    return;
                }

                MsgDTO.fromQQ = long.Parse(entities.FirstOrDefault().Content);
            }

            Type t = this.GetType();
            foreach(var method in t.GetMethods())
            {
                foreach(var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Group)
                    {
                        continue;
                    }
                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    return;
                }
            }
        }

        public virtual void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            Type t = this.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    var enterAttr = attr as EnterCommandAttribute;
                    if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Private)
                    {
                        continue;
                    }
                    if(enterAttr.IsDeveloperOnly && MsgDTO.fromQQ != Common.DeveloperNumber)
                    {
                        continue;
                    }
                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO }
                            );
                    return;
                }
            }
        }
    }
}
