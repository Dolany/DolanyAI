using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Game.SwordExplore
{
    public class SEAreaModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string[] Scenes { get; set; }

        public string DefaultScene { get; set; }

        public string[] NeighbourAreas { get; set; }
    }

    public class SESceneModel
    {
        public string ID { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// 场景类型：Town/Boss/Wild
        /// </summary>
        public string Type { get; set; }

        public string[] AccessableScenes { get; set; }

        #region Wild

        public string[] Monsters { get; set; }

        public int MonsterRate { get; set; }

        /// <summary>
        /// 空闲时的文字提示
        /// </summary>
        public string IdleWords { get; set; }

        /// <summary>
        /// 体力消耗
        /// </summary>
        public int Endurance { get; set; }

        #endregion

        #region Boss

        public string BossName { get; set; }

        /// <summary>
        /// Boss重生时间间隔（小时）
        /// </summary>
        public int BossRebornInterval { get; set; }

        public string HiddenBoss { get; set; }

        public int HiddenBossRate { get; set; }

        /// <summary>
        /// Boss战的体力消耗
        /// </summary>
        public int BossEndurance { get; set; }

        #endregion
    }
}
