using Dolany.Game.OnlineStore;

namespace Dolany.Game.Chess
{
    public partial class ChessEngine
    {
        [ChessEffect(Name = "昙天",
            Description = "48小时内不可以捞瓶子")]
        public void 昙天()
        {

        }

        [ChessEffect(Name = "浓雾",
            Description = "随机复制一件对方的物品")]
        public void 浓雾()
        {

        }

        [ChessEffect(Name = "烈日",
            Description = "抢夺对方50金币")]
        public void 烈日()
        {

        }

        [ChessEffect(Name = "雪",
            Description = "丢失50金币")]
        public void 雪()
        {

        }

        [ChessEffect(Name = "晴岚",
            Description = "没有任何事情发生")]
        public void 晴岚()
        {

        }

        [ChessEffect(Name = "风雨",
            Description = "对方获得50金币")]
        public void 风雨()
        {

        }

        [ChessEffect(Name = "极光",
            Description = "24小时内商店购买享有40%的折扣")]
        public void 极光()
        {

        }

        [ChessEffect(Name = "黄砂",
            Description = "48小时内无法再次挑战")]
        public void 黄砂()
        {

        }

        [ChessEffect(Name = "梅雨",
            Description = "48小时内捞瓶子成功率下降30%")]
        public void 梅雨()
        {

        }

        [ChessEffect(Name = "快晴",
            Description = "36小时内无法从事商业活动（贩卖/购买/交易）")]
        public void 快晴()
        {

        }

        [ChessEffect(Name = "雾雨",
            Description = "随机获得一个商店正在出售的物品")]
        public void 雾雨()
        {

        }

        [ChessEffect(Name = "苍天",
            Description = "24小时内进行交易时享有40%的折扣")]
        public void 苍天()
        {

        }

        [ChessEffect(Name = "雹",
            Description = "强制贩卖一个随机物品给系统商店")]
        public void 雹()
        {

        }

        [ChessEffect(Name = "花昙",
            Description = "随机清除自身的一个负面状态")]
        public void 花昙()
        {

        }

        [ChessEffect(Name = "天气雨",
            Description = "随机移除对方的一个增益状态")]
        public void 天气雨()
        {

        }

        [ChessEffect(Name = "疏雨",
            Description = "24小时内将物品贩卖给商店时将额外获得40%的金币")]
        public void 疏雨()
        {

        }

        [ChessEffect(Name = "川雾",
            Description = "随机复制对手一个负面状态到自己身上")]
        public void 川雾()
        {

        }

        [ChessEffect(Name = "台风",
            Description = "随机复制对手一个增益状态到自己身上")]
        public void 台风()
        {

        }

        [ChessEffect(Name = "凪",
            Description = "增加一次捞瓶子的机会(当日有效)")]
        public void 凪()
        {

        }

        [ChessEffect(Name = "钻石尘",
            Description = "48小时内捞瓶子时有50%的概率丢失40金币")]
        public void 钻石尘()
        {
            OSPerson.AddBuff(SelfQQNum, "钻石尘", "48小时内捞瓶子时有50%的概率丢失40金币", false, 48);
        }
    }
}
