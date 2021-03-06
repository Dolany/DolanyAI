﻿using System;

namespace Dolany.WorldLine.Standard.Ai.Game.Jump300Report.Model
{
    public class JumpMatchBaseInfo
    {
        /// <summary>
        /// 比赛类型
        /// </summary>
        public string MatchKind { get; set; }

        /// <summary>
        /// 总杀人
        /// </summary>
        public int TotalKill { get; set; }

        /// <summary>
        /// 总死亡
        /// </summary>
        public int TotalDie { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 比赛时长
        /// </summary>
        public TimeSpan DuringSpan { get; set; }
    }
}
