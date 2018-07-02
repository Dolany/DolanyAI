using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Flexlive.CQP.Framework;
using Flexlive.CQP.Framework.Utils;
using AILib.SyntaxChecker;
using AILib.Entities;

namespace AILib
{
    public abstract class AIBase
    {
        public AIConfigDTO ConfigDTO { get; set; }
        public int PriorityLevel { get; set; }

        public abstract void Work();

        public AIBase(AIConfigDTO ConfigDTO)
        {
            this.ConfigDTO = ConfigDTO;
        }

        public virtual bool OnGroupMsgReceived(GroupMsgDTO MsgDTO)
        {
            if (IsAiSealed(MsgDTO))
            {
                return false;
            }

            if (MsgDTO.fromQQ < 0)
            {
                MsgDTO.fromQQ = MsgDTO.fromQQ & 0xFFFFFFFF;
            }

            Type t = this.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    object[] param;

                    if (!GroupCheck(attr as EnterCommandAttribute, MsgDTO, out param))
                    {
                        continue;
                    }

                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO, param }
                            );
                    return true;
                }
            }

            return false;
        }

        private bool IsAiSealed(GroupMsgDTO MsgDTO)
        {
            var query = DbMgr.Query<AISealEntity>(s => s.GroupNum == MsgDTO.fromGroup && s.Content == GetType().Name);
            return !query.IsNullOrEmpty();
        }

        private bool GroupCheck(EnterCommandAttribute enterAttr, GroupMsgDTO MsgDTO, out object[] param)
        {
            if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Group)
            {
                param = null;
                return false;
            }

            if (!SyntaxCheck(enterAttr, MsgDTO.msg, out param))
            {
                return false;
            }

            string authority = CQ.GetGroupMemberInfo(MsgDTO.fromGroup, MsgDTO.fromQQ, true).Authority;
            if (!AuthorityCheck(enterAttr.AuthorityLevel, authority))
            {
                return false;
            }

            return true;
        }

        private bool SyntaxCheck(EnterCommandAttribute enterAttr, string msg, out object[] param)
        {
            if (string.IsNullOrEmpty(enterAttr.SyntaxChecker))
            {
                param = null;
                return true;
            }

            try
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                object scObj = assembly.CreateInstance("AILib.SyntaxChecker." + enterAttr.SyntaxChecker + "Checker");
                ISyntaxChecker checker = scObj as ISyntaxChecker;
                return checker.Check(msg, out param);
            }
            catch
            {
                param = null;
                return false;
            }
        }

        private bool AuthorityCheck(AuthorityLevel authorityLevel, string authority)
        {
            if (authorityLevel == AuthorityLevel.群主 && authority != "群主")
            {
                return false;
            }
            if (authorityLevel == AuthorityLevel.管理员 && (authority != "群主" && authority != "管理员"))
            {
                return false;
            }

            return true;
        }

        public virtual void OnPrivateMsgReceived(PrivateMsgDTO MsgDTO)
        {
            Type t = this.GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (var attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    object[] param;

                    if (!PrivateCheck(attr as EnterCommandAttribute, MsgDTO, out param))
                    {
                        continue;
                    }

                    t.InvokeMember(method.Name,
                            BindingFlags.InvokeMethod,
                            null,
                            this,
                            new object[] { MsgDTO, param }
                            );
                    return;
                }
            }
        }

        private bool PrivateCheck(EnterCommandAttribute enterAttr, PrivateMsgDTO MsgDTO, out object[] param)
        {
            param = null;
            if (enterAttr.Command != MsgDTO.command || enterAttr.SourceType != MsgType.Private)
            {
                return false;
            }

            if (enterAttr.IsDeveloperOnly && MsgDTO.fromQQ != Common.DeveloperNumber)
            {
                return false;
            }

            if (!SyntaxCheck(enterAttr, MsgDTO.msg, out param))
            {
                return false;
            }

            return true;
        }
    }
}