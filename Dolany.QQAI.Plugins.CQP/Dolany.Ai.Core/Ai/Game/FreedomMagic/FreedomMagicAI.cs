using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Core.API;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Model;
using Dolany.Database;
using Dolany.Game.FreedomMagic;
using Dolany.Game.OnlineStore;

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
        Enable = true,
        PriorityLevel = 10)]
    public class FreedomMagicAI : AIBase
    {
        private Dictionary<int, SingWordsLevelModel> SwModels;
        private Dictionary<int, int> ForgetConsumeDic = new Dictionary<int, int>();
        private Dictionary<int, int> VolumeIncreaseCostDic = new Dictionary<int, int>();

        public override void Initialization()
        {
            SwModels = CommonUtil.ReadJsonData<Dictionary<int, SingWordsLevelModel>>("singWordsLevelData");

            ForgetConsumeDic = CommonUtil.ReadJsonData<Dictionary<int, int>>("magicForgetConsumeData");
            VolumeIncreaseCostDic = CommonUtil.ReadJsonData<Dictionary<int, int>>("volumeIncreaseCostData");
        }

        [EnterCommand(
            Command = "创建魔法",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "创建一个魔法，等级根据咒文的长度而定（请尽量不要使用特殊字符）",
            Syntax = "[名称] [咒文]",
            SyntaxChecker = "Word Word",
            Tag = "游戏功能",
            IsPrivateAvailable = true,
            IsTesting = true)]
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

            var player = FMPlayer.GetPlayer(MsgDTO.FromQQ);
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

            var swmode = SwModels.Values.FirstOrDefault(m => m.Min <= effLen && m.Max > effLen);
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
            MsgSender.Instance.PushMsg(MsgDTO, $"恭喜你成功创建了新的魔法！\r{magic}\r请牢记你的咒文！", true);
        }

        private int EffectiveLength(string singWords)
        {
            var chars = singWords.Distinct();
            return chars.Count();
        }

        [EnterCommand(Command = "遗忘魔法",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "遗忘一个魔法，需要花费一定量的金币",
            Syntax = "[名称]",
            SyntaxChecker = "Word",
            Tag = "游戏功能",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public void ForgetMagic(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            var player = FMPlayer.GetPlayer(MsgDTO.FromQQ);
            if (player.Magics == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有创建过任何魔法！");
                return;
            }

            var magic = player.Magics.FirstOrDefault(m => m.Name == name);
            if (magic == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还未创建该魔法！");
                return;
            }

            if (!ForgetConsumeDic.Keys.Contains(magic.MagicLevel))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未查找到魔法等级数据！");
                return;
            }

            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var cost = ForgetConsumeDic[magic.MagicLevel];
            var msg = $"遗忘此魔法将消耗 {cost} 个金币！";
            if (cost > osPerson.Golds)
            {
                MsgSender.Instance.PushMsg(MsgDTO, msg + "你的持有的金币不足以支付费用！");
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "操作取消！");
                return;
            }

            player.Magics.Remove(magic);
            MongoService<FMPlayer>.Update(player);

            OSPerson.GoldConsume(MsgDTO.FromQQ, cost);
            MsgSender.Instance.PushMsg(MsgDTO, $"你已经成功遗忘该魔法！你当前学会的魔法数量：{player.Magics.Count}，你当前持有的金币：{osPerson.Golds - cost}", true);
        }

        [EnterCommand(Command = "魔法扩容",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "扩充可以持有的魔法数量上限，需要花费大量金币",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "游戏功能",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public void VolumeIncrease(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            var player = FMPlayer.GetPlayer(MsgDTO.FromQQ);

            if (!VolumeIncreaseCostDic.Keys.Contains(player.MagicVolume))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "未知的容量等级！");
                return;
            }

            var cost = VolumeIncreaseCostDic[player.MagicVolume];
            var msg = $"扩充魔法数量上限需要消耗金币：{cost}!";
            if (cost > osPerson.Golds)
            {
                MsgSender.Instance.PushMsg(MsgDTO, msg + "你的持有的金币不足以支付费用！");
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, msg, 7))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "操作取消！");
                return;
            }

            player.MagicVolume++;
            MongoService<FMPlayer>.Update(player);

            OSPerson.GoldConsume(MsgDTO.FromQQ, cost);
            MsgSender.Instance.PushMsg(MsgDTO, $"扩容成功！你当前魔法容量为：{player.MagicVolume}，你持有的金币为：{osPerson.Golds - cost}", true);
        }

        [EnterCommand(Command = "我的魔法书",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己已经学会的所有魔法",
            Syntax = "",
            SyntaxChecker = "Empty",
            Tag = "游戏功能",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public void MyMagicBook(MsgInformationEx MsgDTO, object[] param)
        {
            var player = FMPlayer.GetPlayer(MsgDTO.FromQQ);
            if (player.Magics.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有学会任何魔法！");
                return;
            }

            var msgs = player.Magics.Select(m => $"{m.Name} 等级：{m.MagicLevel} 消耗：{m.MagicCost}");
            var msg = string.Join("\r", msgs);
            MsgSender.Instance.PushMsg(MsgDTO, msg);
        }

        [EnterCommand(Command = "查看魔法",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己已经学会的某个魔法",
            Syntax = "[魔法名]",
            SyntaxChecker = "Word",
            Tag = "游戏功能",
            IsPrivateAvailable = true,
            IsTesting = true)]
        public void ViewMagic(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            var player = FMPlayer.GetPlayer(MsgDTO.FromQQ);
            if (player.Magics.IsNullOrEmpty())
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你还没有学会任何魔法！");
                return;
            }

            var magic = player.Magics.FirstOrDefault(m => m.Name == name);
            if (magic == null)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你尚未学会该魔法！");
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, magic.ToString());
        }

        [EnterCommand(Command = "魔法挑战",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "向一个成员发起挑战",
            Syntax = "[@QQ号]",
            SyntaxChecker = "At",
            Tag = "游戏功能",
            IsPrivateAvailable = false,
            IsTesting = true)]
        public void MagicChallege(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];

            var firstPlayer = FMPlayer.GetPlayer(MsgDTO.FromQQ);
            if (firstPlayer.Magics == null || firstPlayer.Magics.Count < 5)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你学会的魔法数量不足以进行挑战！");
                return;
            }

            var secondPlayer = FMPlayer.GetPlayer(aimQQ);
            if (secondPlayer.Magics == null || secondPlayer.Magics.Count < 5)
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的对手学会的魔法数量不足以接受挑战！");
                return;
            }

            if (FMGameMgr.IsPlaying_Group(MsgDTO.FromGroup))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "本群正在进行一场挑战，请稍后再试！");
                return;
            }

            if (FMGameMgr.IsPlaying_Player(MsgDTO.FromQQ))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你正在进行一场挑战，请稍后再试！");
                return;
            }

            if (FMGameMgr.IsPlaying_Player(aimQQ))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "你的对手正在进行一场挑战，请稍后再试！");
                return;
            }

            if (!Waiter.Instance.WaitForConfirm(MsgDTO, $"你是否接受来自 {CodeApi.Code_At(MsgDTO.FromQQ)}的挑战？"))
            {
                MsgSender.Instance.PushMsg(MsgDTO, "挑战取消！");
                return;
            }

            MsgSender.Instance.PushMsg(MsgDTO, "挑战即将开始！打扫擂台中......");
            FMGameMgr.GameStart(MsgDTO.FromGroup, new FMPlayerEx(firstPlayer),
                new FMPlayerEx(secondPlayer), (msg, sendTo) =>
                {
                    if (sendTo == 0)
                    {
                        MsgSender.Instance.PushMsg(MsgDTO, msg);
                        return;
                    }

                    MsgDTO.FromQQ = sendTo;
                    MsgSender.Instance.PushMsg(MsgDTO, msg, true);
                },
                (msg, judge, timeout) => Waiter.Instance.WaitForInformation(MsgDTO, msg, judge, timeout));
        }
    }
}
