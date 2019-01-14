namespace Dolany.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Dolany.Database;

    public class CacheService
    {
        private readonly List<CacheTable> Tables = new List<CacheTable>();

        private readonly object Tab_lock = new object();

        private readonly RabbitMQService Mq;

        public CacheService(string CacheInQueue)
        {
            this.Mq = new RabbitMQService(CacheInQueue);
            this.Mq.StartReceive<CacheRequestModel>(OnReceive);
        }

        private void OnReceive(CacheRequestModel request)
        {
            Task.Run(
                () =>
                    {
                        switch (request.RequestType)
                        {
                            case 1:
                                SaveCache(request);
                                break;
                            case 2:
                                GetCache(request);
                                break;
                        }
                    });
        }

        private void SaveCache(CacheRequestModel request)
        {
            lock (Tab_lock)
            {
                var table = this.Tables.FirstOrDefault(t => t.Name == request.RequestTable);
                if (table == null)
                {
                    table = new CacheTable { Name = request.RequestTable };
                    this.Tables.Add(table);
                }

                table.Cache(request.RequestKey, request.RequestValue, request.ExpireTime);
            }

            Console.WriteLine($"{DateTime.Now} Save:{request.RequestTable}, {request.RequestKey}");
        }

        private void GetCache(CacheRequestModel request)
        {
            var value = string.Empty;
            lock (Tab_lock)
            {
                var table = this.Tables.FirstOrDefault(t => t.Name == request.RequestTable);
                if (table != null)
                {
                    value = table.Get(request.RequestKey);
                }
            }

            this.Mq.Send(new CacheResponseModel { Id = request.Id, Value = value }, request.ResponseQueueName);
            Console.WriteLine($"{DateTime.Now} Get:{request.RequestTable}, {request.RequestKey}");
        }

        public void Refresh()
        {
            lock (Tables)
            {
                foreach (var cacheTable in this.Tables)
                {
                    cacheTable.Refresh();
                    Console.WriteLine($"{DateTime.Now} Refresh:{cacheTable.Name}");
                }
            }
        }
    }
}
