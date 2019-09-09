﻿using System.Collections.Generic;
using Dolany.Ai.Common;

namespace Dolany.Ai.Doremi.Ai.Game.XunYuan
{
    public class XunyuanCaveModel : INamedJsonModel
    {
        public string Name { get; set; }

        public List<XunyuanMonsterModel> Monsters { get; set; }

        public XunyuanMonsterModel RandMonster => Monsters.RandElement();
    }

    public class XunyuanMonsterModel
    {
        public string Name { get; set; }

        public int HPTimes { get; set; }

        public int AttackTimes { get; set; }

        public string DropArmerTag { get; set; }

        public int DropGolds { get; set; }
    }
}
