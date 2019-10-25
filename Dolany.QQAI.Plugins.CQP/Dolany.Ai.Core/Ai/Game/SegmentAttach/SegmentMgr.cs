using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.Ai.Game.SegmentAttach
{
    public class SegmentMgr
    {
        public static SegmentMgr Instance { get; } = new SegmentMgr();

        public readonly List<TreasureModel> Treasures;

        private SegmentMgr()
        {
            Treasures = CommonUtil.ReadJsonData_NamedList<TreasureModel>("TreasureData");
        }

        public SegmentModel RandSegment()
        {
            var randTreasure = Treasures.RandElement();
            return randTreasure.Segments.RandElement();
        }

        public TreasureModel FindTreasureBySegment(string segmentName)
        {
            return Treasures.FirstOrDefault(p => p.Segments.Any(s => s.Name == segmentName));
        }

        public TreasureModel FindTreasureByName(string treasureName)
        {
            return Treasures.FirstOrDefault(p => p.Name == treasureName);
        }

        public SegmentModel FindSegmentByName(string segmentName)
        {
            return Treasures.SelectMany(p => p.Segments).FirstOrDefault(p => p.Name == segmentName);
        }
    }

    public class TreasureModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PicPath { get; set; }

        public List<SegmentModel> Segments { get;set; }

        public bool IsMatch(string segment1, string segment2)
        {
            if (Segments.All(s => s.Name != segment1) || Segments.All(s => s.Name != segment2))
            {
                return false;
            }

            return segment1 != segment2;
        }

        public override string ToString()
        {
            return $"【{Name}】\r{CodeApi.Code_Image_Relational(PicPath)}\r{Description}\r需要碎片：{string.Join(",", Segments.Select(s => s.Name))}";
        }
    }

    public class SegmentModel
    {
        public string Name { get;set; }

        public string Description { get; set; }

        public string PicPath { get; set; }

        public override string ToString()
        {
            return $"【{Name}】\r{CodeApi.Code_Image_Relational(PicPath)}\r{Description}";
        }
    }
}
