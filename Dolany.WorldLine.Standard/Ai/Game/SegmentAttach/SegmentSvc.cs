using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.SegmentAttach
{
    public class SegmentSvc : IDataMgr, IDependency
    {
        public List<TreasureModel> Treasures;

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

        public void RefreshData()
        {
            Treasures = CommonUtil.ReadJsonData_NamedList<TreasureModel>("Standard/TreasureData");
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
            return $"【{Name}】\r\n{CodeApi.Code_Image_Relational(PicPath)}\r\n{Description}\r\n需要碎片：{string.Join(Emoji.星星, Segments.Select(s => $"【{s.Name}】"))}";
        }
    }

    public class SegmentModel
    {
        public string Name { get;set; }

        public string Description { get; set; }

        public string PicPath { get; set; }

        public override string ToString()
        {
            return $"【{Name}】\r\n{CodeApi.Code_Image_Relational(PicPath)}\r\n{Description}";
        }
    }
}
