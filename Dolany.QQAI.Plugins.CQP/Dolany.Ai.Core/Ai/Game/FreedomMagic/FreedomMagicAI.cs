using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Game.FreedomMagic;

namespace Dolany.Ai.Core.Ai.Game.FreedomMagic
{
    public class SingWordsLevelModel
    {
        public int Level { get; set; }

        public int Min { get; set; }

        public int Max { get; set; }
    }

    [AI(Name = nameof(FreedomMagicAI),
        Description = "Ai for freedom magic game",
        Enable = false,
        PriorityLevel = 10)]
    public class FreedomMagicAI : AIBase
    {
        private List<SingWordsLevelModel> SwModels;

        public override void Initialization()
        {
            var data = CommonUtil.ReadJsonData<Dictionary<int, SingWordsLevelModel>>("singWordsLevelData");
            SwModels = data.Select(d =>
            {
                var (key, value) = d;
                value.Level = key;
                return value;
            }).ToList();
        }

        [EnterCommand(
            Command = "创建魔法",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "创建一个魔法，等级根据咒文的长度而定（请尽量不要使用特殊字符）",
            Syntax = "[名称] [咒文]",
            SyntaxChecker = "Word Word",
            Tag = "游戏功能",
            IsPrivateAvailable = true)]
        public void CreateMagic(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var singWords = param[1] as string;

            if (string.IsNullOrEmpty(name) || name.Length < 2 || name.Length > 6)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "魔法名称必须在2-6个字之间！");
                return;
            }

            var effLen = EffectiveLength(singWords);
            if (effLen < 5)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "咒文有效长度不能低于5个字！");
                return;
            }

            var player = GetPlayer(MsgDTO.FromQQ);
            if (player.IsMagicFull)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你持有的魔法数量已到达上限！");
                return;
            }

            if (player.Magics.Any(m => m.Name == name))
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"已存在名为 {name} 的魔法！");
                return;
            }

            if (player.Magics.Any(m => m.SingWords == singWords))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "咒文与已知魔法雷同！");
                return;
            }

            var swmode = SwModels.FirstOrDefault(m => m.Min >= effLen && m.Max < effLen);
            if (swmode == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未知等级的魔法！");
                return;
            }

            if (swmode.Level > player.MaxMagicLevel)
            {
                MsgSender.Instance.PushMsg(MsgDTO, $"你只能创建最大 {player.MaxMagicLevel}级的魔法！");
                return;
            }

            var magic = player.LearnMagic(name, singWords, swmode.Level);
            MongoService<FMPlayer>.Update(player);
            MsgSender.Instance.PushMsg(MsgDTO, $"恭喜你成功创建了新的魔法！\r{magic}\r请牢记你的咒文！");
        }

        private int EffectiveLength(string singWords)
        {
            var chars = singWords.Distinct();
            return chars.Count();
        }

        private FMPlayer GetPlayer(long QQNum)
        {
            var query = MongoService<FMPlayer>.Get(p => p.QQNum == QQNum).FirstOrDefault();
            if (query != null)
            {
                return query;
            }

            var player = FMPlayer.Create(QQNum);
            MongoService<FMPlayer>.Insert(player);
            return player;
        }
    }
}
