namespace Dolany.Ai.Core.Cache
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Dolany.Ai.Common;
    using Dolany.Ai.Core.Common;
    using Dolany.Database;

    using Newtonsoft.Json;

    public class CacheWaiter
    {
        private readonly object _lockObj = new object();

        private readonly List<CacheWaitUnit> Units = new List<CacheWaitUnit>();

        public static CacheWaiter Instance { get; } = new CacheWaiter();

        public void Listen()
        {
            Global.CacheInfoService.StartReceive<CacheResponseModel>(ListenCallBack);
        }

        private void ListenCallBack(CacheResponseModel response)
        {
            CacheWaitUnit waitUnit;
            lock (_lockObj)
            {
                waitUnit = Units.FirstOrDefault(u => u.Id == response.Id);
            }

            if (waitUnit == null)
            {
                return;
            }

            waitUnit.Response = response;
            waitUnit.Signal.Set();
        }

        public T WaitForResponse<T>(string table, string key, int timeout = 5000)
            where T : class
        {
            var request = new CacheRequestModel
                              {
                                  RequestTable = table,
                                  RequestKey = key,
                                  ResponseQueueName = CommonUtil.GetConfig("CacheResponse"),
                                  RequestType = 2
                              };

            var signal = new AutoResetEvent(false);
            var unit = new CacheWaitUnit { Signal = signal };
            lock (_lockObj)
            {
                Units.Add(unit);
            }

            Global.CacheInfoService.Send(request, CommonUtil.GetConfig("CacheInfoService"));
            signal.WaitOne(timeout);

            lock (_lockObj)
            {
                unit = Units.FirstOrDefault(u => u.Id == unit.Id);
                Units.Remove(unit);
            }

            if (unit?.Response == null || string.IsNullOrEmpty(unit.Response.Value))
            {
                return null;
            }

            return JsonConvert.DeserializeObject<T>(unit.Response.Value);
        }

        public void SendCache<T>(string table, string key, T value, DateTime? ExpireTime = null) // where T : class 
        {
            var valueStr = JsonConvert.SerializeObject(value);
            var request = new CacheRequestModel
                              {
                                  RequestTable = table,
                                  RequestKey = key,
                                  RequestValue = valueStr,
                                  ExpireTime = ExpireTime,
                                  RequestType = 1
                              };

            Global.CacheInfoService.Send(request, CommonUtil.GetConfig("CacheInfoService"));
        }
    }

    public class CacheWaitUnit
    {
        public string Id { get; } = Guid.NewGuid().ToString();

        public AutoResetEvent Signal { get; set; }

        public CacheResponseModel Response { get; set; }
    }
}
