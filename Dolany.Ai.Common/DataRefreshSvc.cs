using System.Collections.Generic;

namespace Dolany.Ai.Common
{
    /// <summary>
    /// 数据托管接口
    /// </summary>
    public interface IDataMgr
    {
        /// <summary>
        /// 刷新数据
        /// </summary>
        void RefreshData();
    }

    public class DataRefreshSvc : IDependency
    {
        private readonly List<IDataMgr> AllMgrs = new List<IDataMgr>();

        public void Register(IDataMgr mgr)
        {
            AllMgrs.Add(mgr);
        }

        public int RefreshAll()
        {
            foreach (var mgr in AllMgrs)
            {
                mgr.RefreshData();
            }

            return AllMgrs.Count;
        }
    }
}
