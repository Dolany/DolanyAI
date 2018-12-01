﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dolany.Ai.Reborn.DolanyAI.Cache;
using Dolany.Ai.Reborn.DolanyAI.Common;
using Dolany.Ai.Reborn.DolanyAI.DTO;
using static Dolany.Ai.Reborn.DolanyAI.Common.Utility;
using static Dolany.Ai.Reborn.MahuaApis.CodeApi;

namespace Dolany.Ai.Reborn.DolanyAI.Base
{
    public abstract class AIBase
    {
        // ReSharper disable once MemberCanBeProtected.Global
        public delegate void MsgConsolerDel(ReceivedMsgDTO msgDTO, object[] para);

        // ReSharper disable once MemberCanBePrivate.Global
        protected readonly Dictionary<EnterCommandAttribute, MsgConsolerDel> Consolers =
            new Dictionary<EnterCommandAttribute, MsgConsolerDel>();

        protected AIBase()
        {
            var t = GetType();
            foreach (var method in t.GetMethods())
            {
                foreach (EnterCommandAttribute attr in method.GetCustomAttributes(typeof(EnterCommandAttribute), false))
                {
                    foreach (var command in attr.CommandsList)
                    {
                        var attrClone = attr.Clone();
                        attrClone.Command = command;
                        Consolers.Add(attrClone, method.CreateDelegate(typeof(MsgConsolerDel), this) as MsgConsolerDel);
                    }
                }
            }
        }

        public abstract void Work();

        public virtual bool OnMsgReceived(ReceivedMsgDTO MsgDTO)
        {
            var query = Consolers.Where(c => c.Key.Command == MsgDTO.Command).ToList();
            if (query.IsNullOrEmpty())
            {
                return false;
            }

            try
            {
                foreach (var consoler in query)
                {
                    if (!Check(consoler.Key, MsgDTO, out var param))
                    {
                        continue;
                    }

                    if (RecentCommandCache.IsTooFreq())
                    {
                        MsgSender.Instance.PushMsg(MsgDTO, "哇哇哇~~，AI过热中......");
                        MsgSender.Instance.PushMsg(MsgDTO, Code_Image(new FileInfo("images/过热.jpg").FullName));
                        return true;
                    }
                    consoler.Value(MsgDTO, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
            }

            return false;
        }

        private static bool Check(EnterCommandAttribute enterAttr, ReceivedMsgDTO MsgDTO, out object[] param)
        {
            param = null;
            if (!enterAttr.CommandsList.Contains(MsgDTO.Command))
            {
                return false;
            }

            if (!SyntaxCheck(enterAttr.SyntaxChecker, MsgDTO.Msg, out param))
            {
                return false;
            }

            if (!AuthorityCheck(enterAttr.AuthorityLevel, enterAttr, MsgDTO))
            {
                return false;
            }

            return true;
        }

        private static bool SyntaxCheck(string SyntaxChecker, string msg, out object[] param)
        {
            param = null;
            if (string.IsNullOrEmpty(SyntaxChecker))
            {
                return false;
            }

            try
            {
                var checkers = SyntaxChecker.Split(' ');
                var paramStrs = msg.Split(' ');
                if (checkers.Length != paramStrs.Length)
                {
                    return false;
                }

                var list = new List<object>();
                for (var i = 0; i < checkers.Length; i++)
                {
                    var i1 = i;
                    var checker = AIMgr.Instance.Checkers.Value.FirstOrDefault(c => c.Name == checkers[i1]);
                    if (checker == null ||
                        !checker.Check(paramStrs[i], out var p))
                    {
                        return false;
                    }

                    if (p != null)
                    {
                        list.AddRange(p);
                    }
                }

                param = list.ToArray();
                return true;
            }
            catch (Exception ex)
            {
                RuntimeLogger.Log(ex);
                return false;
            }
        }

        private static bool AuthorityCheck(AuthorityLevel authorityLevel, EnterCommandAttribute enterAttr, ReceivedMsgDTO MsgDTO)
        {
            if (MsgDTO.FromQQ == DeveloperNumber)
            {
                return true;
            }

            return MsgDTO.MsgType == MsgType.Group ?
                GroupCheck(authorityLevel, MsgDTO) :
                PrivateCheck(enterAttr);
        }

        private static bool GroupCheck(AuthorityLevel authorityLevel, ReceivedMsgDTO MsgDTO)
        {
            var mi = GetMemberInfo(MsgDTO);
            if (mi == null)
            {
                return false;
            }

            var authority = mi.Role;
            if (authorityLevel == AuthorityLevel.开发者)
            {
                return false;
            }
            if (authority == 0)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.群主)
            {
                return false;
            }
            if (authority == 1)
            {
                return true;
            }
            if (authorityLevel == AuthorityLevel.管理员)
            {
                return false;
            }

            return true;
        }

        private static bool PrivateCheck(EnterCommandAttribute enterAttr)
        {
            return enterAttr.IsPrivateAvailabe;
        }

        public virtual void OnActiveStateChange(bool state, long GroupNum)
        {
        }
    }
}