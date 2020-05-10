using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.UtilityTool;
using Dolany.WorldLine.Doremi.OnlineStore;
using Dolany.WorldLine.Doremi.Xiuxian;

namespace Dolany.WorldLine.Doremi.Ai.Game.XunYuan
{
    public class XunYuanAI : AIBase
    {
        public override string AIName { get; set; } = "寻缘";
        public override string Description { get; set; } = "AI for Xunyuan.";
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.寻缘功能;

        public ArmerSvc ArmerSvc { get; set; }
        public LevelSvc LevelSvc { get; set; }

        public XunYuanMgr XunYuanMgr { get; set; }

        [EnterCommand(ID = "XunYuanAI_Xunyuan",
            Command = "寻缘",
            Description = "邀请成员开始寻缘",
            SyntaxHint = "[@QQ号]",
            SyntaxChecker = "At",
            DailyLimit = 3)]
        public bool Xunyuan(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];

            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法和自己寻缘！");
                return false;
            }

            if (!XunYuanMgr.CheckGroup(MsgDTO.FromGroup))
            {
                MsgSender.PushMsg(MsgDTO, "此群正在进行一场寻缘，请稍候再试！");
                return false;
            }

            if (!XunYuanMgr.CheckQQNum(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你正在进行一场寻缘，请稍候再试！");
                return false;
            }

            if (!XunYuanMgr.CheckQQNum(aimQQ))
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
                var osPerson = OSPerson_Doremi.GetPerson(p);
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

            XunYuanMgr.StartGame(Gamers, MsgDTO.FromGroup, MsgDTO.BindAi);
            return true;
        }
    }
}
