using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Game.TouhouCardWar.Cards
{
    public interface ICard
    {
        int No { get; set; }
        string Name { get; set; }
        string Description { get; set; }

        /// <summary>
        /// 卡牌类型:1，英雄；2.符卡；3.天气；4.护符；5.武器
        /// </summary>
        int Type { get; set; }
        string Tag { get; set; }
        string PicPath { get; set; }
        /// <summary>
        /// 元素类型：1.风；2.火；3.水；4.土；5.雷；6.光；7.暗；8.无
        /// </summary>
        int Element { get; set; }
    }
}
