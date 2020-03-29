using System.Collections.Generic;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Common
{
    public class CmdTag
    {
        public CmdTagEnum Tag { get; set; }

        public CmdTag[] SubTags { get; set; } = new CmdTag[0];

        public EnterCommandAttribute[] SubCmds { get; set; } = new EnterCommandAttribute[0];

        public IEnumerable<CmdTag> AllSubTags
        {
            get
            {
                var list = new List<CmdTag>();
                if (SubTags.IsNullOrEmpty())
                {
                    return list;
                }

                list.AddRange(SubTags);
                foreach (var subTag in SubTags)
                {
                    list.AddRange(subTag.AllSubTags);
                }

                return list;
            }
        }
    }

    public enum CmdTagEnum
    {
        Default = 0,
        Root = 1,
        机器人管理 = 2,
        超管 = 3,
        开发者后台 = 4,
        系统命令 = 5,
        群管理 = 6,
        打招呼功能 = 7,
        vip服务 = 8,
        语音功能 = 9,
        娱乐功能 = 10,
        运势功能 = 11,
        语录功能 = 12,
        设定功能 = 13,
        游戏功能 = 14,
        商店功能 = 15,
        宝藏功能 = 16,
        宠物功能 = 17,
        烹饪功能 = 18,
        礼物功能 = 19,
        漂流瓶功能 = 20,
        骰娘功能 = 21,
        人品功能 = 22,
        王国风云 = 23,
        GM奖励 = 24,
        签到功能 = 25,
        帮助系统 = 26
    }
}
