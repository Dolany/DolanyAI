using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Doremi.Ai.Game.Xiuxian;
using Dolany.Ai.Doremi.OnlineStore;
using Dolany.Ai.Doremi.Xiuxian;

namespace Dolany.Ai.Doremi.Ai.Game.Shopping
{
    public class ShoppingAI : AIBase
    {
        public override string AIName { get; set; } = "商店";
        public override string Description { get; set; } = "AI for shopping.";
        protected override CmdTagEnum DefaultTag { get; } = CmdTagEnum.修仙功能;

        public RandShopperSvc RandShopperSvc { get; set; }
        public ArmerSvc ArmerSvc { get; set; }
        public LevelSvc LevelSvc { get; set; }

        [EnterCommand(ID = "ShoppingAI_MyStatus",
            Command = "我的状态",
            Description = "获取自身当前状态")]
        public bool MyStatus(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var level = LevelSvc.GetByLevel(osPerson.Level);
            var exp = MsgCounterSvc.Get(MsgDTO.FromQQ);
            var armerRecord = PersonArmerRecord.Get(MsgDTO.FromQQ);

            var msg = $"等级：{level.Name}\r\n" +
                      $"经验值：{exp}/{level.Exp}{(exp >= level.Exp ? "(可渡劫)" : "")}\r\n" +
                      $"{Emoji.心}:{level.HP}(+{ArmerSvc.CountHP(armerRecord.Armers)})\r\n" +
                      $"{Emoji.剑}:{level.Atk}(+{ArmerSvc.CountAtk(armerRecord.Armers)})\r\n" +
                      $"金币：{osPerson.Golds}";

            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_Buy",
            Command = "购买",
            Description = "购买某件物品",
            SyntaxHint = "[物品名]",
            SyntaxChecker = "Word")]
        public bool Buy(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            if (RandShopperSvc.SellingGoods.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "商店尚未营业！", true);
                return false;
            }

            if (!RandShopperSvc.SellingGoods.Contains(name))
            {
                MsgSender.PushMsg(MsgDTO, "此商品未在商店中出售！", true);
                return false;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var armerModel = ArmerSvc[name];
            if (osPerson.Golds < armerModel.Price)
            {
                MsgSender.PushMsg(MsgDTO, $"你持有的金币不足以购买此物品({osPerson.Golds}/{armerModel.Price})", true);
                return false;
            }

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, armerModel.Price))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            osPerson.Golds -= armerModel.Price;
            var paRec = PersonArmerRecord.Get(MsgDTO.FromQQ);
            paRec.ArmerGet(name);
            paRec.Update();
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "购买成功！");
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_MyArmers",
            Command = "我的装备",
            Description = "获取当前持有的装备")]
        public bool MyArmers(MsgInformationEx MsgDTO, object[] param)
        {
            var paRec = PersonArmerRecord.Get(MsgDTO.FromQQ);
            if (paRec.Armers.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有任何装备！", true);
                return true;
            }

            var armerInfos = paRec.Armers.Select(a => new {Count = a.Value, Model = ArmerSvc[a.Key]}).OrderBy(p => p.Model.Kind).ThenByDescending(p => p.Model.Price).ToList();
            var showInfos = armerInfos.Take(20);
            var msg = string.Join(", ", showInfos.Select(a => $"{a.Model.Name}*{a.Count}"));
            msg = $"你持有的装备有：\r\n{msg}";
            if (armerInfos.Count > 20)
            {
                msg += $"\r\n当前显示第 1/{(armerInfos.Count - 1) / 20 + 1}页，请使用 我的装备 [页码] 命令查看更多装备！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }

        [EnterCommand(ID = "ShoppingAI_MyArmers_Paged",
            Command = "我的装备",
            Description = "按页码获取当前持有的装备",
            SyntaxHint = "[页码]",
            SyntaxChecker = "Long")]
        public bool MyArmers_Paged(MsgInformationEx MsgDTO, object[] param)
        {
            var pageNo = (int) (long) param[0];

            var paRec = PersonArmerRecord.Get(MsgDTO.FromQQ);
            if (paRec.Armers.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未拥有任何装备！", true);
                return true;
            }

            var armerInfos = paRec.Armers.Select(a => new {Count = a.Value, Model = ArmerSvc[a.Key]}).OrderBy(p => p.Model.Kind).ThenByDescending(p => p.Model.Price).ToList();
            var totalPageCount = (armerInfos.Count - 1) / 20 + 1;
            if (pageNo <= 0 || pageNo > totalPageCount)
            {
                MsgSender.PushMsg(MsgDTO, "页码错误！", true);
                return false;
            }

            var showInfos = armerInfos.Skip((pageNo - 1) * 20).Take(20);
            var msg = string.Join(", ", showInfos.Select(a => $"{a.Model.Name}*{a.Count}"));
            msg = $"该页的装备有：\r\n{msg}";
            if (armerInfos.Count > 20)
            {
                msg += $"\r\n当前显示第 {pageNo}/{totalPageCount}页，请使用 我的装备 [页码] 命令查看更多装备！";
            }
            MsgSender.PushMsg(MsgDTO, msg, true);
            return true;
        }
    }
}
