using System.Collections.Generic;

namespace Dolany.Ai.Common
{
    public interface IDataMgr
    {
        void RefreshData();
    }

    public class DataRefresher
    {
        public static DataRefresher Instance { get; } = new DataRefresher();

        private readonly List<IDataMgr> AllMgrs = new List<IDataMgr>();

        private DataRefresher()
        {

        }

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
