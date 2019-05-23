using System;
using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Database;

namespace Dolany.Ai.Core.Ai.Game.Weather
{
    public class WeatherMgr
    {
        public static WeatherMgr Instance { get; } = new WeatherMgr();

        public readonly List<WeatherDataModel> WeatherModels;

        private WeatherMgr()
        {
            WeatherModels = CommonUtil.ReadJsonData_NamedList<WeatherDataModel>("WeatherData");
        }

        public WeatherDataModel TodayWeather(long GroupNum)
        {
            return WeahterForecast(GroupNum).First();
        }

        public IEnumerable<WeatherDataModel> WeahterForecast(long GroupNum)
        {
            var records = MongoService<WeatherRecord>.Get(p => p.GroupNum == GroupNum).OrderBy(p => p.ExpiryTime).ToList();
            if (records.Count >= 7)
            {
                return records.Take(7).Select(p => WeatherModels.First(wd => wd.Name == p.Weather)).ToList();
            }

            var count = records.Count;
            while (count < 7)
            {
                var record = new WeatherRecord()
                {
                    GroupNum = GroupNum,
                    ExpiryTime = CommonUtil.UntilTommorow().AddDays(count),
                    Weather = WeatherModels.RandElement().Name
                };
                records.Add(record);
                MongoService<WeatherRecord>.Insert(record);

                count++;
            }

            return records.Take(7).Select(p => WeatherModels.First(wd => wd.Name == p.Weather)).ToList();
        }
    }

    public class WeatherDataModel
    {
        public string Name { get; set; }

        public string Description { get; set; }
    }

    public class WeatherRecord : DbBaseEntity
    {
        public long GroupNum { get; set; }

        public DateTime ExpiryTime { get; set; }

        public string Weather { get; set; }
    }
}
