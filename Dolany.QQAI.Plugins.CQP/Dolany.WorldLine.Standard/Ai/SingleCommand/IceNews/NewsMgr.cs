using System;
using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Cache;
using Newtonsoft.Json;

namespace Dolany.WorldLine.Standard.Ai.SingleCommand.IceNews
{
    public class NewsMgr : IDependency
    {
        private Queue<string> NewsQueue { get; } = new Queue<string>();

        public string LastNews { get; private set; }

        public NewsMgr()
        {
            var lastR = GlobalVarRecord.Get("LastNews");
            if (lastR != null && !string.IsNullOrEmpty(lastR.Value))
            {
                var model = JsonConvert.DeserializeObject<NewsCacheModel>(lastR.Value);
                LastNews = model.Name;
            }

            var record = GlobalVarRecord.Get("NewsQueue");
            if (record == null || string.IsNullOrEmpty(record.Value))
            {
                return;
            }

            var newsList = JsonConvert.DeserializeObject<string[]>(record.Value);
            foreach (var news in newsList)
            {
                NewsQueue.Enqueue(news);
            }

            if (string.IsNullOrEmpty(LastNews))
            {
                TryToRefresh();
            }
        }

        public void AddNews(string news)
        {
            NewsQueue.Enqueue(news);
            UpdateToDb();
        }

        private void UpdateToDb()
        {
            var record = GlobalVarRecord.Get("NewsQueue");
            record.Value = JsonConvert.SerializeObject(NewsQueue.ToArray());
            record.Update();

            var lrc = GlobalVarRecord.Get("LastNews");
            lrc.Value = JsonConvert.SerializeObject(new NewsCacheModel()
            {
                Name = LastNews,
                UpdateTime = DateTime.Now
            });
            lrc.Update();
        }

        public void TryToRefresh()
        {
            if (NewsQueue.IsNullOrEmpty())
            {
                return;
            }

            LastNews = NewsQueue.Dequeue();
            UpdateToDb();
        }
    }

    public class NewsCacheModel
    {
        public string Name { get; set; }

        public DateTime UpdateTime { get; set; }
    }
}
