using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking
{
    public class CookingAI : AIBase
    {
        public override string AIName { get; set; } = "烹饪";
        public override string Description { get; set; } = "Ai for cooking.";
        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public CookingDietSvc CookingDietSvc { get; set; }
        public CookingLevelSvc CookingLevelSvc { get; set; }
        public HonorSvc HonorSvc { get; set; }

        [EnterCommand(ID = "CookingAI_Cook",
            Command = "烹饪 烹饪菜肴 制作菜肴",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "开始烹饪一道指定的菜肴",
            Syntax = "[菜品名]",
            Tag = "烹饪功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool Cook(MsgInformationEx MsgDTO, object[] param)
        {
            var dietName = param[0] as string;

            var Diet = CookingDietSvc[dietName];
            if (Diet == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关菜肴名称！");
                return false;
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            if (pet.Level < Diet.Level)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}的等级太低，还不能烹饪该菜肴！({pet.Level}/{Diet.Level})");
                return false;
            }

            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            if (cookingRec.LearndDietMenu.IsNullOrEmpty() || !cookingRec.LearndDietMenu.Contains(dietName))
            {
                MsgSender.PushMsg(MsgDTO, $"你尚未学会{dietName}的烹饪方法！(可使用【{Diet.ExchangeHonor}】兑换，详情请参见【帮助 兑换菜谱】)");
                return false;
            }

            var falvoringNeedStr = string.Join(",", Diet.Flavorings.Select(f => $"{f.Key}({cookingRec.FlavoringDic.GetDicValueSafe(f.Key)}/{f.Value})"));
            if (!Diet.Flavorings.IsNullOrEmpty() && !cookingRec.CheckFlavorings(Diet.Flavorings))
            {
                MsgSender.PushMsg(MsgDTO, $"调味料不足！({falvoringNeedStr})");
                return false;
            }

            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var allitems = itemColle.AllItemsDic;
            var materialNeedStr = string.Join(",", Diet.Materials.Select(m => $"{m.Key}({allitems.GetDicValueSafe(m.Key)}/{m.Value})"));
            if (!Diet.Materials.IsNullOrEmpty() && !itemColle.CheckItem(Diet.Materials))
            {
                MsgSender.PushMsg(MsgDTO, $"材料不足！({materialNeedStr})");
                return false;
            }

            var msg = $"烹饪 {dietName} 将需要消耗\r\n{falvoringNeedStr}\r\n{materialNeedStr}\r\n是否确认？";
            if (!WaiterSvc.WaitForConfirm(MsgDTO, msg))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            cookingRec.FlavoringConsume(Diet.Flavorings);
            itemColle.ItemConsume(Diet.Materials);

            cookingRec.AddDiet(dietName);

            cookingRec.Update();
            itemColle.Update();

            MsgSender.PushMsg(MsgDTO, "烹饪成功！");
            return true;
        }

        [EnterCommand(ID = "CookingAI_ViewDiet",
            Command = "查看菜肴",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看指定菜肴的详细情况",
            Syntax = "[菜品名]",
            SyntaxChecker = "Word",
            Tag = "烹饪功能",
            IsPrivateAvailable = true)]
        public bool ViewDiet(MsgInformationEx MsgDTO, object[] param)
        {
            var dietName = param[0] as string;
            var Diet = CookingDietSvc[dietName];
            if (Diet == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关菜肴名称！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, Diet.ToString());
            return true;
        }

        [EnterCommand(ID = "CookingAI_MyCookingHistory",
            Command = "我的烹饪记录",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的历史烹饪情况",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "烹饪功能",
            IsPrivateAvailable = true)]
        public bool MyCookingHistory(MsgInformationEx MsgDTO, object[] param)
        {
            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            var history = cookingRec.CookingHistory;
            if (history.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "你尚未烹饪过任何东西！");
                return false;
            }

            var sumDietCount = history.Sum(p => p.Value);
            var totalPrice = cookingRec.TotalPrice;

            var curLevel = CookingLevelSvc.LocationLevel(totalPrice);
            var nextLevel = CookingLevelSvc[curLevel.Level + 1];

            var msg = $"你总共烹饪过 {sumDietCount} 道菜肴\r\n" +
                      $"总共消耗了物品 {cookingRec.ItemConsumeDic.Sum(p => p.Value)} 个，调味料 {cookingRec.FlavoringTotal} 个\r\n" +
                      $"总价值：{totalPrice.CurencyFormat()}\r\n" +
                      $"当前烹饪评级为：【{curLevel.Name}(lv.{curLevel.Level})】，距离下一等级【{nextLevel.Name}({nextLevel.Level})】还差 {(nextLevel.NeedPrice - totalPrice).CurencyFormat()}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "CookingAI_ExchangeMenu",
            Command = "兑换菜谱 学习菜谱",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "兑换指定的菜谱",
            Syntax = "[菜谱名]",
            SyntaxChecker = "Word",
            Tag = "烹饪功能",
            IsPrivateAvailable = true)]
        public bool ExchangeMenu(MsgInformationEx MsgDTO, object[] param)
        {
            var dietName = param[0] as string;
            var diet = CookingDietSvc[dietName];
            if (diet == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到相关菜肴");
                return false;
            }

            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            if (cookingRec.LearndDietMenu.Contains(dietName))
            {
                MsgSender.PushMsg(MsgDTO, "你已经学会了该菜肴的烹饪方法！");
                return false;
            }

            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var honorModel = HonorSvc.FindHonor(diet.ExchangeHonor);
            var items = honorModel.Items.ToDictionary(p => p.Name, p => 1);
            if (!itemColle.CheckItem(items))
            {
                MsgSender.PushMsg(MsgDTO, $"你尚未集齐【{diet.ExchangeHonor}】的所有物品！");
                return false;
            }

            itemColle.ItemConsume(items);
            itemColle.Update();

            cookingRec.LearndDietMenu.Add(diet.Name);
            cookingRec.Update();

            MsgSender.PushMsg(MsgDTO, "兑换成功！");
            return true;
        }

        [EnterCommand(ID = "CookingAI_MyKitchen",
            Command = "我的厨房",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的厨房情况",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "烹饪功能",
            IsPrivateAvailable = true)]
        public bool MyKitchen(MsgInformationEx MsgDTO, object[] param)
        {
            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            var level = CookingLevelSvc.LocationLevel(cookingRec.TotalPrice);
            var msg = $"【{level.Name}(lv.{level.Level})】\r\n";
            var itemColle = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            var dietList = cookingRec.LearndDietMenu.Select(diet =>
            {
                var Diet = CookingDietSvc[diet];
                if (!Diet.Flavorings.IsNullOrEmpty() && !cookingRec.CheckFlavorings(Diet.Flavorings) ||
                    !Diet.Materials.IsNullOrEmpty() && !itemColle.CheckItem(Diet.Materials))
                {
                    return $"{diet}(材料不足)";
                }

                return diet;
            });
            msg +=
                $"已学会的菜谱：{string.Join("，", dietList)}\r\n";
            msg += $"当前持有菜肴：{string.Join("，", cookingRec.CookedDietDic.Select(p => $"{p.Key}*{p.Value}"))}\r\n";
            msg += $"当前持有调味料：{string.Join("，", cookingRec.FlavoringDic.Select(p => $"{p.Key}*{p.Value}"))}\r\n";
            msg += $"推荐学习菜谱：【{CookingDietSvc.SuggestDiet(cookingRec.LearndDietMenu)?.Name}】";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "CookingAI_DietMenu",
            Command = "菜谱总览",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看所有的菜谱",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "烹饪功能",
            IsPrivateAvailable = true)]
        public bool DietMenu(MsgInformationEx MsgDTO, object[] param)
        {
            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            var allDiets = CookingDietSvc.DietList;
            var msg = string.Join("\r\n",
                allDiets.Select(
                    diet => $"{diet.Name}{(cookingRec.LearndDietMenu.Contains(diet.Name) ? "(已学会)" : string.Empty)}:{string.Join(",", diet.Attributes)}"));

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }
}
