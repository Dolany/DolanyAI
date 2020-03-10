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
        private static ArmerSvc ArmerSvc => AutofacSvc.Resolve<ArmerSvc>();
        private static LevelSvc LevelSvc => AutofacSvc.Resolve<LevelSvc>();
        private static WaiterSvc WaiterSvc => AutofacSvc.Resolve<WaiterSvc>();

        [EnterCommand(ID = "XunYuanAI_Xunyuan",
            Command = "寻缘",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "邀请成员开始寻缘",
            Syntax = "[@QQ号]",
            Tag = "寻缘功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 3)]
        public bool Xunyuan(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];

            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法和自己寻缘！");
                return false;
            }

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
            if (!WaiterSvc.WaitForConfirm(MsgDTO.FromGroup, aimQQ, msg, MsgDTO.BindAi))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var Gamers = new[] {MsgDTO.FromQQ, aimQQ}.Select(p =>
            {
                var armerRecord = PersonArmerRecord.Get(p);
                var osPerson = OSPerson.GetPerson(p);
                var levelModel = LevelSvc.GetByLevel(osPerson.Level);

                var battleArmers = Rander.RandSort(armerRecord.Armers.ToArray()).Take(10).ToDictionary(a => a.Key, a => a.Value);
                return new XunYuanGamingModel()
                {
                    QQNum = p,
                    Armers = battleArmers,
                    EscapeArmers = armerRecord.EscapeArmers,
                    BasicHP = levelModel.HP,
                    HP = levelModel.HP + ArmerSvc.CountHP(battleArmers),
                    BasicAttack = levelModel.Atk,
                    Attack = levelModel.Atk + ArmerSvc.CountAtk(battleArmers)
                };
            }).ToArray();

            XunYuanMgr.Instacne.StartGame(Gamers, MsgDTO.FromGroup, MsgDTO.BindAi);
            return true;
        }
    }
}
