using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Ai.Game.Pet.Cooking;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;
using Newtonsoft.Json;

namespace Dolany.Ai.Core.Ai.Game.Pet.Expedition
{
    public class ExpeditionSceneMgr
    {
        public static ExpeditionSceneMgr Instance { get; } = new ExpeditionSceneMgr();

        private readonly List<ExpeditionSceneModel> Scenes;

        public ExpeditionSceneModel this[string SceneName] => Scenes.First(s => s.Name == SceneName);

        private ExpeditionSceneMgr()
        {
            Scenes = CommonUtil.ReadJsonData_NamedList<ExpeditionSceneModel>("Pet/ExpeditionSceneData");
        }

        public List<ExpeditionSceneModel> TodayExpedition()
        {
            var cache = GlobalVarRecord.Get("TodayExpedition");
            if (!string.IsNullOrEmpty(cache.Value))
            {
                var modelNames = JsonConvert.DeserializeObject<string[]>(cache.Value);
                return Scenes.Where(s => modelNames.Contains(s.Name)).ToList();
            }

            var todayScenes = Rander.RandSort(Scenes.ToArray()).Take(3).ToList();
            cache.Value = JsonConvert.SerializeObject(todayScenes.Select(s => s.Name).ToArray());
            cache.ExpiryTime = CommonUtil.UntilTommorow();
            cache.Update();

            return todayScenes;
        }
    }

    public class ExpeditionSceneModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int Endurance { get; set; }

        public int TimeConsume { get; set; }

        public ExpeditionBonus_Gold GoldBonus { get; set; }

        public ExpeditionBonus_Item ItemBonus { get; set; }

        public ExpeditionBonus_Flavoring FlavoringBonus { get; set; }

        public int Level
        {
            get
            {
                var level = 0;
                if (GoldBonus != null)
                {
                    level += GoldBonus.Level;
                }

                if (ItemBonus != null)
                {
                    level += ItemBonus.Level;
                }

                if (FlavoringBonus != null)
                {
                    level += FlavoringBonus.Level;
                }

                return level;
            }
        }

        public string Award(long QQNum)
        {
            var msg = $"恭喜你在 {Name} 的伟大远征中获得以下奖励：";
            var msgList = new List<string>();
            if (GoldBonus != null)
            {
                var gold = GoldBonus.Min + Rander.RandInt(GoldBonus.Max - GoldBonus.Min + 1);
                var osPerson = OSPerson.GetPerson(QQNum);
                osPerson.Golds += gold;
                osPerson.Update();

                msgList.Add($"金币：{gold}{Emoji.钱袋}");
            }

            if (ItemBonus != null)
            {
                var count = ItemBonus.Min + Rander.RandInt(ItemBonus.Max - ItemBonus.Min + 1);
                var items = Enumerable.Range(0, count).Select(p => ItemBonus.Options.RandElement()).ToList();
                var itemColle = ItemCollectionRecord.Get(QQNum);
                foreach (var item in items)
                {
                    itemColle.ItemIncome(item);
                }

                msgList.Add($"物品：{string.Join(",", items)}");
            }

            if (FlavoringBonus != null)
            {
                var count = FlavoringBonus.Min + Rander.RandInt(FlavoringBonus.Max - FlavoringBonus.Min + 1);
                var flavorings = Enumerable.Range(0, count).Select(p => FlavoringBonus.Options.RandElement()).ToList();
                var flavoringRec = CookingRecord.Get(QQNum);
                foreach (var flavoring in flavorings)
                {
                    flavoringRec.FlavoringIncome(flavoring);
                }
                flavoringRec.Update();

                msgList.Add($"调味料：{string.Join(",", flavorings)}");
            }

            return $"{msg}\r{string.Join("\r", msgList)}";
        }

        public string ToString(int curEndurance)
        {
            var str = $"{Name}\r    {Description}\r耐力：{curEndurance}/{Endurance}{(curEndurance < Endurance ? "(耐力不足)" : string.Empty)}\r耗时：{TimeConsume}分钟";
            if (GoldBonus != null)
            {
                str += $"\r金币奖励：{Utility.LevelToStars(GoldBonus.Level)}";
            }

            if (ItemBonus != null)
            {
                str += $"\r物品奖励：{Utility.LevelToStars(ItemBonus.Level)}";
            }

            if (FlavoringBonus != null)
            {
                str += $"\r调味料奖励：{Utility.LevelToStars(FlavoringBonus.Level)}";
            }

            return str;
        }
    }

    public class ExpeditionBonus_Gold
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public int Level
        {
            get
            {
                if (Max > 1000)
                {
                    return 5;
                }

                if (Max > 600)
                {
                    return 4;
                }

                if (Max > 400)
                {
                    return 3;
                }

                if (Max > 200)
                {
                    return 2;
                }

                return 1;
            }
        }
    }

    public class ExpeditionBonus_Item
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public string[] Options { get; set; }

        public int Level
        {
            get
            {
                var models = HonorHelper.Instance.Items.Where(item => Options.Contains(item.Name));
                var sumPrice = models.Max(m => m.Price) * Max;
                if (sumPrice > 3000)
                {
                    return 5;
                }

                if (sumPrice > 2000)
                {
                    return 4;
                }

                if (sumPrice > 1000)
                {
                    return 3;
                }

                if (sumPrice > 500)
                {
                    return 2;
                }

                return 1;
            }
        }
    }

    public class ExpeditionBonus_Flavoring
    {
        public int Min { get; set; }

        public int Max { get; set; }

        public string[] Options { get; set; }

        public int Level
        {
            get
            {
                if (Max > 5)
                {
                    return 5;
                }

                if (Max > 4)
                {
                    return 4;
                }

                if (Max > 3)
                {
                    return 3;
                }

                if (Max > 2)
                {
                    return 2;
                }

                return 1;
            }
        }
    }
}
