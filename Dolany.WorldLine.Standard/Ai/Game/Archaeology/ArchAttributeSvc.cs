using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.UtilityTool;

namespace Dolany.WorldLine.Standard.Ai.Game.Archaeology
{
    public class ArchAttributeSvc : IDependency, IDataMgr
    {
        private List<ArchAttrLevelModel> Levels = new List<ArchAttrLevelModel>();

        private IEnumerable<ArchAttribute> Attributes { get; } = new List<ArchAttribute>()
        {
            new ArchAttribute()
            {
                Code = "Ice",
                Name = "寒冰",
                StrongerThan = "Flame",
                Emoji = Emoji.雪花
            },
            new ArchAttribute()
            {
                Code = "Flame",
                Name = "火焰",
                StrongerThan = "Lightning",
                Emoji = Emoji.火焰
            },
            new ArchAttribute()
            {
                Code = "Lightning",
                Name = "雷电",
                StrongerThan = "Ice",
                Emoji = Emoji.闪电
            }
        };

        public ArchAttrLevelModel this[int level] => Levels.FirstOrDefault(l => l.Level == level);
        public ArchAttribute this[string attrCode] => Attributes.FirstOrDefault(p => p.Code == attrCode);

        public void RefreshData()
        {
            Levels = CommonUtil.ReadJsonData<List<ArchAttrLevelModel>>("Standard/Arch/ArchAttrLevelData");
        }
    }

    public class ArchAttrLevelModel
    {
        public int Level { get; set; }

        public int Max { get; set; }

        public int Min { get; set; }

        public int Rand => Rander.RandRange(Min, Max);
    }

    public class ArchAttribute
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string StrongerThan { get; set; }

        public string Emoji { get; set; }
    }
}
