using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Cache;
using Dolany.Game.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.TouhouCard
{
    public class SpellCardMgr
    {
        public static SpellCardMgr Instance { get; } = new SpellCardMgr();

        private readonly Dictionary<SpellCardAttribute, Action<long, OSPerson, OSPerson>> CardDic = new Dictionary<SpellCardAttribute, Action<long, OSPerson, OSPerson>>();

        private SpellCardMgr()
        {
            var type = GetType();
            foreach (var method in type.GetMethods())
            {
                if (!(method.GetCustomAttribute(typeof(SpellCardAttribute)) is SpellCardAttribute attr))
                {
                    continue;
                }

                CardDic.Add(attr, method.CreateDelegate(typeof(Action<long, OSPerson, OSPerson>)) as Action<long, OSPerson, OSPerson>);
            }
        }

        private static string GetPicMsg(string name)
        {
            return CodeApi.Code_Image_Relational($"images/TouhouCardWar/{name}.jpg");
        }

        public void UseCard(long GroupNum, OSPerson Source, OSPerson Aim, string CardName)
        {
            var attr = CardDic.Keys.FirstOrDefault(a => a.Name == CardName);
            if (attr == null)
            {
                MsgSender.Instance.PushMsg(GroupNum, Source.QQNum, "未找到对应的卡牌！");
                return;
            }

            if (Source.CurrentMP < attr.Cost)
            {
                MsgSender.Instance.PushMsg(GroupNum, Source.QQNum, $"法力值不足({Source.CurrentMP}/{attr.Cost})！");
                return;
            }

            if (Source.QQNum == Aim.QQNum)
            {
                Aim = Source;
            }

            Source.MPCost(attr.Cost);
            MsgSender.Instance.PushMsg(GroupNum, Source.QQNum, GetPicMsg(attr.Name));
            CardDic[attr](GroupNum, Source, Aim);

            Source.Update();
            if (Source.QQNum != Aim.QQNum)
            {
                Aim.Update();
            }
        }

        [SpellCard(Name = "能量药剂",
            Cost = 1,
            Description = "立刻恢复全部的法力值",
            Kind = SpellCardKind.对战)]
        private void 能量药剂(long GroupNum, OSPerson Source, OSPerson Aim)
        {
            Aim.MPRestore(Aim.MaxMP);
            MsgSender.Instance.PushMsg(GroupNum, Aim.QQNum, "已恢复全部法力值！", true);
        }
    }
}
