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

        public List<CmdTag> AllSubTags
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
        Root,
        机器人管理,
        超管,
        开发者后台,
        系统命令,
        群管理,
        打招呼功能,
        vip服务,
        语音功能,
        娱乐功能,
        运势功能,
        语录功能,
        设定功能,
        游戏功能,
        商店功能,
        宝藏功能,
        宠物功能,
        烹饪功能,
        礼物功能,
        漂流瓶功能,
        骰娘功能,
        人品功能,
        王国风云
    }
}
