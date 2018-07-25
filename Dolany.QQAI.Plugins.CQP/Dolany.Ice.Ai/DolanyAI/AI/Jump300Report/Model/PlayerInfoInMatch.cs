using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.DolanyAI
{
    public class PlayerInfoInMatch
    {
        /// <summary>
        /// 角色名
        /// </summary>
        public string PlayerName { get; set; }

        /// <summary>
        /// 角色等级
        /// </summary>
        public int PlayerLevel { get; set; }

        /// <summary>
        /// 英雄名
        /// </summary>
        public string HeroName { get; set; }

        /// <summary>
        /// 英雄等级
        /// </summary>
        public int HeroLevel { get; set; }

        /// <summary>
        /// 杀
        /// </summary>
        public int Kill { get; set; }

        /// <summary>
        /// 死
        /// </summary>
        public int Die { get; set; }

        /// <summary>
        /// 助攻
        /// </summary>
        public int Assist { get; set; }

        /// <summary>
        /// 比赛结果
        /// </summary>
        public string Result { get; set; }

        /// <summary>
        /// 建筑摧毁
        /// </summary>
        public int BuildingDestory { get; set; }

        /// <summary>
        /// 小兵杀死
        /// </summary>
        public int SoldierKill { get; set; }

        /// <summary>
        /// 打钱数
        /// </summary>
        public int MoneyGen { get; set; }

        /// <summary>
        /// 评分
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// 获取金币
        /// </summary>
        public int PlayerCoin { get; set; }

        /// <summary>
        /// 获得经验
        /// </summary>
        public int PlayerExp { get; set; }

        /// <summary>
        /// 节操值
        /// </summary>
        public int MoralIntegrity { get; set; }

        /// <summary>
        /// 总胜场
        /// </summary>
        public int TotalWin { get; set; }

        /// <summary>
        /// 总比赛数
        /// </summary>
        public int TotalMatch { get; set; }
    }
}