using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Pet.Cooking
{
    public class CookingAI : AIBase
    {
        public override string AIName { get; set; } = "烹饪";
        public override string Description { get; set; } = "Ai for cooking.";
        public override int PriorityLevel { get; set; } = 10;

        public override bool Enable { get; set; } = false;

        private List<CookingLevelModel> CookingLevels = new List<CookingLevelModel>();

        public override void Initialization()
        {
            CookingLevels = CommonUtil.ReadJsonData_NamedList<CookingLevelModel>("CookingLevelData").OrderBy(p => p.Level).ToList();
        }

        [EnterCommand(ID = "CookingAI_Cook",
            Command = "烹饪",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "开始烹饪一道指定的菜肴",
            Syntax = "[菜品名]",
            Tag = "烹饪功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = true)]
        public bool Cook(MsgInformationEx MsgDTO, object[] param)
        {
            var dietName = param[0] as string;

            var Diet = CookingDietMgr.Instance[dietName];
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
                MsgSender.PushMsg(MsgDTO, $"你尚未学会{dietName}的烹饪方法！");
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

            var msg = $"烹饪 {dietName} 将需要消耗\r{falvoringNeedStr}\r{materialNeedStr}\r是否确认？";
            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg))
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
            var Diet = CookingDietMgr.Instance[dietName];
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
            var itemConsumeDic = new Dictionary<string, int>();
            var flavoringTotal = 0;
            foreach (var (key, value) in history)
            {
                var diet = CookingDietMgr.Instance[key];
                flavoringTotal += diet.Flavorings?.Sum(p => p.Value) * value ?? 0;
                if (diet.Materials.IsNullOrEmpty())
                {
                    continue;
                }

                foreach (var (material, count) in diet.Materials)
                {
                    if (!itemConsumeDic.ContainsKey(material))
                    {
                        itemConsumeDic.Add(material, 0);
                    }

                    itemConsumeDic[material] += count * value;
                }
            }

            var totalPrice = itemConsumeDic.Sum(p => HonorHelper.Instance.FindItem(p.Key).Price * p.Value) + flavoringTotal * 20;
            var curLevel = CookingLevels.First();
            var nextLevel = CookingLevels.First();
            for (var i = 0; i < CookingLevels.Count; i++)
            {
                if (CookingLevels[i].NeedPrice <= totalPrice)
                {
                    continue;
                }

                curLevel = CookingLevels[i - 1];
                nextLevel = CookingLevels[i];
                break;
            }

            var msg = $"你总共烹饪过 {sumDietCount} 道菜肴\r总共消耗了物品 {itemConsumeDic.Sum(p => p.Value)} 个，调味料 {flavoringTotal} 个\r" +
                      $"总价值：{totalPrice}{Emoji.钱袋}\r当前烹饪评级为：{curLevel.Name}，距离下一等级({nextLevel.Name})还差 {nextLevel.NeedPrice - totalPrice}{Emoji.钱袋}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }
    }

    public class CookingLevelModel : INamedJsonModel
    {
        public string Name { get; set; }

        public int Level { get; set; }

        public int NeedPrice { get; set; }
    }
}
