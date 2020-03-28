using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Common;

namespace Dolany.WorldLine.Standard
{
    public class StandardWorldLine : IWorldLine
    {
        public override string Name { get; set; } = "经典";
        public override bool IsDefault { get; } = true;
        public override CmdTag CmdTagTree { get; set; } = new CmdTag()
        {
            Tag = CmdTagEnum.Root,
            SubTags = new[]
            {
                new CmdTag()
                {
                    Tag = CmdTagEnum.机器人管理,
                    SubTags = new []
                    {
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.超管
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.开发者后台
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.系统命令
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.群管理
                        }
                    }
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.vip服务
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.娱乐功能,
                    SubTags = new []
                    {
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.打招呼功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.运势功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.语录功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.设定功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.语音功能
                        }
                    }
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.游戏功能,
                    SubTags = new []
                    {
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.商店功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.宝藏功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.礼物功能
                        },
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.漂流瓶功能
                        }
                    }
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.宠物功能,
                    SubTags = new []
                    {
                        new CmdTag()
                        {
                            Tag = CmdTagEnum.烹饪功能
                        }
                    }
                },
                new CmdTag()
                {
                    Tag = CmdTagEnum.骰娘功能
                }
            }
        };
    }
}
