using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking;
using Dolany.WorldLine.Standard.OnlineStore;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet.Expedition
{
    public class ExpeditionSceneSvc : IDataMgr, IDependency
    {
        private List<ExpeditionSceneModel> Scenes;

        public ExpeditionSceneModel this[string SceneName] => Scenes.FirstOrDefault(s => s.Name == SceneName);

        public readonly string[] Flavorings = {"海鲜酱油", "秘制番茄酱", "精品海盐", "风味辣酱", "蓝巧果果汁", "火桂", "龙鳞草茎"};

        public void RefreshData()
        {
            Scenes = CommonUtil.ReadJsonData_NamedList<ExpeditionSceneModel>("Standard/Pet/ExpeditionSceneData");
        }

        public IEnumerable<ExpeditionSceneModel> TodayExpedition()
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

        public Dictionary<string, int> RandFlavorings(int count)
        {
            var result = new Dictionary<string, int>();
            for (var i = 0; i < count; i++)
            {
                var randFlavoring = Flavorings.RandElement();
                if (result.ContainsKey(randFlavoring))
                {
                    result[randFlavoring]++;
                }
                else
                {
                    result.Add(randFlavoring, 1);
                }

                Thread.Sleep(10);
            }

            return result;
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

        public ExpeditionAward Award(long QQNum)
        {
            var award = new ExpeditionAward(){Name = Name};
            if (GoldBonus != null)
            {
                award.Gold = GoldBonus.Min + Rander.RandInt(GoldBonus.Max - GoldBonus.Min + 1);
                var osPerson = OSPerson.GetPerson(QQNum);
                osPerson.Golds += award.Gold;
                osPerson.Update();
            }

            if (ItemBonus != null)
            {
                var count = ItemBonus.Min + Rander.RandInt(ItemBonus.Max - ItemBonus.Min + 1);
                award.Items = Enumerable.Range(0, count).Select(p => ItemBonus.Options.RandElement()).ToList();
                var itemColle = ItemCollectionRecord.Get(QQNum);
                foreach (var item in award.Items)
                {
                    itemColle.ItemIncome(item);
                }
            }

            if (FlavoringBonus != null)
            {
                var count = FlavoringBonus.Min + Rander.RandInt(FlavoringBonus.Max - FlavoringBonus.Min + 1);
                award.Flavorings = Enumerable.Range(0, count).Select(p => FlavoringBonus.Options.RandElement()).ToList();
                var flavoringRec = CookingRecord.Get(QQNum);
                foreach (var flavoring in award.Flavorings)
                {
                    flavoringRec.FlavoringIncome(flavoring);
                }
                flavoringRec.Update();
            }

            return award;
        }

        public string ToString(int curEndurance)
        {
            var str = $"【{Name}】：{Description}\r\n耐力：{Endurance}({curEndurance})     耗时：{TimeConsume}分钟\r\n奖励：";
            if (GoldBonus != null)
            {
                str += $"  金币：{Utility.LevelToStars(GoldBonus.Level)}";
            }

            if (ItemBonus != null)
            {
                str += $"  物品：{Utility.LevelToStars(ItemBonus.Level)}";
            }

            if (FlavoringBonus != null)
            {
                str += $"  调味料：{Utility.LevelToStars(FlavoringBonus.Level)}";
            }

            return str;
        }

        public override string ToString()
        {
            var str = $"【{Name}】\r\n    {Description}\r\n耐力：{Endurance}\r\n耗时：{TimeConsume}分钟";
            if (GoldBonus != null)
            {
                str += $"\r\n金币奖励：{Utility.LevelToStars(GoldBonus.Level)}";
            }

            if (ItemBonus != null)
            {
                str += $"\r\n物品奖励：{Utility.LevelToStars(ItemBonus.Level)}";
            }

            if (FlavoringBonus != null)
            {
                str += $"\r\n调味料奖励：{Utility.LevelToStars(FlavoringBonus.Level)}";
            }

            return str;
        }
    }

    public class ExpeditionAward
    {
        public string Name { get; set; }

        public int Gold { get; set; }

        public List<string> Items { get; set; } = new List<string>();

        public List<string> Flavorings { get; set; } = new List<string>();

        public override string ToString()
        {
            var msg = $"恭喜你在 【{Name}】 的伟大远征中获得以下奖励：";
            var msgList = new List<string>();
            if (Gold > 0)
            {
                msgList.Add($"金币：{Gold.CurencyFormat()}");
            }

            if (!Items.IsNullOrEmpty())
            {
                msgList.Add($"物品：{string.Join(",", Items.GroupBy(p => p).Select(p => $"{p.Key}*{p.Count()}"))}");
            }

            if (!Flavorings.IsNullOrEmpty())
            {
                msgList.Add($"调味料：{string.Join(",", Flavorings.GroupBy(p => p).Select(p => $"{p.Key}*{p.Count()}"))}");
            }

            return $"{msg}\r\n{string.Join("\r\n", msgList)}";
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
                var models = AutofacSvc.Resolve<HonorSvc>().Items.Where(item => Options.Contains(item.Name));
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
