using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Doremi.Base;
using Dolany.Ai.Doremi.Cache;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Ai.Doremi.Xiuxian;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    [AI(Name = "寻缘",
        Description = "AI for Xunyuan.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = false,
        BindAi = "DoreFun")]
    public class XunYuanAI : AIBase
    {
        [EnterCommand(ID = "XunYuanAI_Xunyuan",
            Command = "寻缘",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "邀请成员开始寻缘",
            Syntax = "[@QQ号]",
            Tag = "寻缘功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 1)]
        public bool Xunyuan(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];

            if (!XunYuanMgr.Instacne.CheckGroup(MsgDTO.FromGroup))
            {
                MsgSender.PushMsg(MsgDTO, "此群正在进行一场寻缘，请稍候再试！");
                return false;
            }

            if (!XunYuanMgr.Instacne.CheckQQNum(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你正在进行一场寻缘，请稍候再试！");
                return false;
            }

            if (!XunYuanMgr.Instacne.CheckQQNum(aimQQ))
            {
                MsgSender.PushMsg(MsgDTO, "对方正在进行一场寻缘，请稍候再试！");
                return false;
            }

            var msg = $"{CodeApi.Code_At(aimQQ)} 你正被邀请参加一次寻缘，是否同意？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg, MsgDTO.BindAi))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var Gamers = new[] {MsgDTO.FromQQ, aimQQ}.Select(p =>
            {
                var armerRecord = PersonArmerRecord.Get(p);
                var osPerson = OSPerson.GetPerson(p);
                var levelModel = LevelMgr.Instance.GetByLevel(osPerson.Level);
                return new XunYuanGamingModel()
                {
                    QQNum = p,
                    Armers = armerRecord.Armers,
                    EscapeArmers = armerRecord.EscapeArmers,
                    BasicHP = levelModel.HP,
                    HP = levelModel.HP + ArmerMgr.Instance.CountHP(armerRecord.Armers),
                    BasicAttack = levelModel.Atk,
                    Attack = levelModel.Atk + ArmerMgr.Instance.CountAtk(armerRecord.Armers)
                };
            }).ToArray();

            XunYuanMgr.Instacne.StartGame(Gamers, MsgDTO.FromGroup, MsgDTO.BindAi);
            return true;
        }
    }
}
