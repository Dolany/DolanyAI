using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;
using Dolany.Database.Ai;

namespace GroupSetting
{
    class Program
    {
        private static List<AIEnableState> AIEnableStateList;
        private static List<ActiveOffGroups> ActiveOffGroupsList;
        private static List<AlertRegistedGroup> AlertRegistedGroupList;
        private static List<DiceActiveGroup> DiceActiveGroupList;
        private static List<PlusOneAvailable> PlusOneAvailableList;
        private static List<RepeaterAvailable> RepeaterAvailableList;

        static void Main(string[] args)
        {
            InitData();

            var allSettings = MongoService<GroupSettings>.Get();
            MongoService<GroupSettings>.DeleteMany(allSettings);

            var numNameDic = CommonUtil.ReadJsonData<Dictionary<long, string>>("RegisterGroupData");
            foreach (var dic in numNameDic)
            {
                var (key, value) = dic;
                var setting = CreateSetting(key, value);
                MongoService<GroupSettings>.Insert(setting);
            }

            Console.ReadKey();
        }

        private static void InitData()
        {
            AIEnableStateList = MongoService<AIEnableState>.Get();
            ActiveOffGroupsList = MongoService<ActiveOffGroups>.Get();
            AlertRegistedGroupList = MongoService<AlertRegistedGroup>.Get();
            DiceActiveGroupList = MongoService<DiceActiveGroup>.Get();
            PlusOneAvailableList = MongoService<PlusOneAvailable>.Get();
            RepeaterAvailableList = MongoService<RepeaterAvailable>.Get();
        }

        private static GroupSettings CreateSetting(long num, string name)
        {
            var functions = AIEnableStateList.Where(p => p.Groups.Contains(num)).Select(p => p.Name).ToList();
            if (AlertRegistedGroupList.Any(p => p.GroupNum == num && bool.Parse(p.Available)))
            {
                functions.Add("报时");
            }

            if (DiceActiveGroupList.Any(p => p.GroupNum == num))
            {
                functions.Add("骰娘");
            }

            if (PlusOneAvailableList.Any(p => p.GroupNumber == num && p.Available))
            {
                functions.Add("+1复读");
            }

            if (RepeaterAvailableList.Any(p => p.GroupNumber == num && p.Available))
            {
                functions.Add("随机复读");
            }

            var setting = new GroupSettings()
            {
                GroupNum = num,
                Name = name,
                IsPowerOn = ActiveOffGroupsList.All(p => p.GroupNum != num),
                EnabledFunctions = functions
            };

            Console.WriteLine(setting.GroupNum);
            Console.WriteLine(setting.Name);
            Console.WriteLine(setting.IsPowerOn);
            Console.WriteLine(string.Join(",", setting.EnabledFunctions));
            Console.WriteLine();

            return setting;
        }
    }
}
