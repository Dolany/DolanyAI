using System;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.Common.PicReview;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Cooking;
using Dolany.WorldLine.Standard.Ai.Game.Pet.Expedition;
using Dolany.WorldLine.Standard.Ai.Game.Pet.PetAgainst;
using Dolany.WorldLine.Standard.Ai.Vip;
using Dolany.WorldLine.Standard.OnlineStore;

namespace Dolany.WorldLine.Standard.Ai.Game.Pet
{
    public class PetAI : AIBase
    {
        public override string AIName { get; set; } = "宠物";

        public override string Description { get; set; } = "AI for Petting.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public override bool NeedManualOpeon { get; } = true;

        private const string CachePath = "./images/Cache/";

        private const string PetPicFolder = "./images/Custom/Pet/";

        private const int FeedInterval = 2;

        public CookingDietSvc CookingDietSvc { get; set; }
        public PicReviewSvc PicReviewSvc { get; set; }
        public PetSkillSvc PetSkillSvc { get; set; }
        public PetLevelSvc PetLevelSvc { get; set; }
        public PetAgainstSvc PetAgainstSvc { get; set; }
        public HonorSvc HonorSvc { get; set; }
        public BindAiSvc BindAiSvc { get; set; }

        public override void Initialization()
        {
            PicReviewSvc.Register("宠物头像", SetPetPicCallBack);
        }

        [EnterCommand(ID = "PetAI_MyPet",
            Command = "我的宠物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看自己的宠物的状态",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool MyPet(MsgInformationEx MsgDTO, object[] param)
        {
            var pet = PetRecord.Get(MsgDTO.FromQQ);

            var levelModel = PetLevelSvc[pet.Level];

            var HasExtEndur = VipArmerRecord.Get(MsgDTO.FromQQ).CheckArmer("耐力护符");
            var extEndur = HasExtEndur ? "(+10)" : string.Empty;
            var petEndur = levelModel.Endurance - PetEnduranceRecord.Get(MsgDTO.FromQQ).ConsumeTotal + (HasExtEndur ? 10 : 0);

            var msg = $"{CodeApi.Code_Image_Relational(pet.PicPath)}\r" +
                      $"名称：{pet.Name}\r" +
                      $"种族：{pet.PetNo}\r" +
                      $"食性：{pet.Attribute ?? "无"}\r" +
                      $"等级：{Utility.LevelEmoji(pet.Level)}\r" +
                      $"{Emoji.心}：{levelModel.HP}\r" +
                      $"耐力：{petEndur}/{levelModel.Endurance}{extEndur}\r" +
                      $"经验值：{pet.Exp}/{levelModel.Exp}";
            if (!pet.Skills.IsNullOrEmpty())
            {
                msg += $"\r技能：{string.Join(",", pet.Skills.Select(p => $"{p.Key}({p.Value})"))}";
            }
            if (pet.RemainSkillPoints > 0)
            {
                msg += $"\r可用技能点：{pet.RemainSkillPoints}";
            }

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "PetAI_RenamePet",
            Command = "重命名宠物 设定宠物名称",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "重命名自己的宠物（名字不能超过十个字）",
            Syntax = "[名称]",
            Tag = "宠物功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = false,
            DailyLimit = 3,
            TestingDailyLimit = 4)]
        public bool RenamePet(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            name = name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MsgSender.PushMsg(MsgDTO, "名字不能为空！", true);
                return false;
            }

            if (name.Length > 10)
            {
                MsgSender.PushMsg(MsgDTO, "名字不能超过10个字！", true);
                return false;
            }
            var pet = PetRecord.Get(MsgDTO.FromQQ);
            pet.Name = name;
            pet.Update();

            MsgSender.PushMsg(MsgDTO, "重命名成功！");
            return true;
        }

        [EnterCommand(ID = "PetAI_SetPetKind",
            Command = "设定宠物种族 设置宠物种族",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定宠物的种族（不能超过十个字）",
            Syntax = "[种族名]",
            Tag = "宠物功能",
            SyntaxChecker = "Any",
            IsPrivateAvailable = false,
            DailyLimit = 3,
            TestingDailyLimit = 4)]
        public bool SetPetKind(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            name = name?.Trim();
            if (string.IsNullOrEmpty(name))
            {
                MsgSender.PushMsg(MsgDTO, "种族不能为空！", true);
                return false;
            }

            if (name.Length > 10)
            {
                MsgSender.PushMsg(MsgDTO, "种族不能超过10个字！", true);
                return false;
            }
            var pet = PetRecord.Get(MsgDTO.FromQQ);
            pet.PetNo = name;
            pet.Update();

            MsgSender.PushMsg(MsgDTO, "设定成功！");
            return true;
        }

        [EnterCommand(ID = "PetAI_SetPetPic",
            Command = "设定宠物头像 设置宠物头像",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "上传宠物头像的图片（不能超过300KB）",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false,
            DailyLimit = 3,
            TestingDailyLimit = 4)]
        public bool SetPetPic(MsgInformationEx MsgDTO, object[] param)
        {
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < 300)
            {
                MsgSender.PushMsg(MsgDTO, $"你的金币余额不足（{osPerson.Golds.CurencyFormat()}/{300.CurencyFormat()}）");
                return false;
            }

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 300))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var info = WaiterSvc.WaitForInformation(MsgDTO, "请上传图片（不能超过300KB）！",
                information => information.FromGroup == MsgDTO.FromGroup && information.FromQQ == MsgDTO.FromQQ &&
                               !string.IsNullOrEmpty(Utility.ParsePicGuid(information.Msg)), 10);
            if(info == null)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消!");
                return false;
            }

            var bindai = BindAiSvc[MsgDTO.BindAi];

            var picGuid = Utility.ParsePicGuid(info.Msg);
            var imageCache = Utility.ReadImageCacheInfo(picGuid, bindai.ImagePath);

            if (imageCache == null)
            {
                MsgSender.PushMsg(MsgDTO, "未读取到图片！");
                return false;
            }

            var fileName = $"PetPic-{MsgDTO.FromQQ}.{imageCache.type}";
            if (!Utility.DownloadImage(imageCache.url, CachePath + fileName))
            {
                MsgSender.PushMsg(MsgDTO, "图片下载失败，请稍后再试！");
                return false;
            }

            var picFile = new FileInfo(CachePath + fileName);
            if (picFile.Length / 1024 > 300)
            {
                MsgSender.PushMsg(MsgDTO, "图片过大，请选择较小的图片重新上传！", true);
                picFile.Delete();
                return false;
            }

            osPerson.Golds -= 300;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "上传成功！待审核通过后方可生效！");
            var review = new PicReviewRecord()
            {
                GroupNum = MsgDTO.FromGroup,
                QQNum = MsgDTO.FromQQ,
                Usage = "宠物头像",
                PicName = picFile.Name
            };
            PicReviewSvc.AddReview(review);

            return true;
        }

        private static void SetPetPicCallBack(PicReviewRecord record)
        {
            if (record.Status == PicReviewStatus.Refused)
            {
                return;
            }

            var picFile = new FileInfo($"{CachePath}{record.PicName}");
            picFile.CopyTo($"{PetPicFolder}{record.PicName}", true);
            var pet = PetRecord.Get(record.QQNum);
            pet.PicPath = $"{PetPicFolder}{record.PicName}";
            pet.Update();
        }

        [EnterCommand(ID = "PetAI_SetPetAttr",
            Command = "设定宠物食性 设置宠物食性",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定宠物的初始食性",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 1)]
        public bool SetPetAttr(MsgInformationEx MsgDTO, object[] param)
        {
            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var needGolds = false;
            OSPerson osPerson = null;
            if (!string.IsNullOrEmpty(pet.Attribute))
            {
                osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);

                if (osPerson.Golds < 300)
                {
                    MsgSender.PushMsg(MsgDTO, $"金币余额不足！({osPerson.Golds.CurencyFormat()}/{300.CurencyFormat()})");
                    return false;
                }

                if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, 300))
                {
                    MsgSender.PushMsg(MsgDTO, "操作取消");
                    return false;
                }

                needGolds = true;
            }

            var randAttrs = Rander.RandSort(PetExtent.AllAttributes.ToArray());
            var msg = $"请选择宠物食性：\r{string.Join("\r", randAttrs.Select((p, idx) => $"{idx + 1}:{p}"))}";
            var selectedIdx = WaiterSvc.WaitForNum(MsgDTO.FromGroup, MsgDTO.FromQQ, msg, i => i > 0 && i <= randAttrs.Length, MsgDTO.BindAi);
            if (selectedIdx == -1)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消");
                return false;
            }

            pet.Attribute = randAttrs[selectedIdx - 1];
            pet.Update();

            if (needGolds)
            {
                osPerson.Golds -= 300;
                osPerson.Update();
            }

            MsgSender.PushMsg(MsgDTO, "设定成功！");
            return true;
        }

        [EnterCommand(ID = "PetAI_FeedPet",
            Command = "喂食宠物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "喂给宠物指定的物品（请遵循宠物的食性）",
            Syntax = "[物品名]",
            Tag = "宠物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool FeedPet(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var expRec = ExpeditionRecord.GetLastest(MsgDTO.FromQQ);
            if (expRec != null && expRec.IsExpediting)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}正在【{expRec.Scene}】进行一项伟大的远征，请于{expRec.EndTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}后再试！");
                return false;
            }

            if (string.IsNullOrEmpty(pet.Attribute))
            {
                MsgSender.PushMsg(MsgDTO, "请先设置宠物食性！", true);
                return false;
            }

            if (pet.LastFeedTime != null && pet.LastFeedTime.Value.AddHours(FeedInterval).ToLocalTime() > DateTime.Now)
            {
                var msg = $"{pet.Name}还饱着呢，不想吃东西（请与{pet.LastFeedTime.Value.AddHours(FeedInterval).ToLocalTime()}后再试）";
                MsgSender.PushMsg(MsgDTO, msg);
                return false;
            }

            var item = HonorSvc.FindItem(name);
            if (item != null)
            {
                return FeedPetWithItem(MsgDTO, pet, item);
            }

            var diet = CookingDietSvc[name];
            if (diet != null)
            {
                return FeedPetWithDiet(MsgDTO, pet, diet);
            }

            MsgSender.PushMsg(MsgDTO, "未查找到相关物品或菜肴！");
            return false;
        }

        private static bool FeedPetWithItem(MsgInformationEx MsgDTO, PetRecord pet, DriftBottleItemModel item)
        {
            if (item.Attributes == null)
            {
                MsgSender.PushMsg(MsgDTO, "该物品无法投喂！");
                return false;
            }

            if (!item.Attributes.Contains(pet.Attribute))
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}说不想吃这个东西（请喂食正确特性的物品）");
                return false;
            }

            var honorRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!honorRecord.CheckItem(item.Name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有该物品！", true);
                return false;
            }

            var resMsg = $"{pet.Name}兴奋的吃掉了 {item.Name}，并打了个饱嗝\r";

            pet.LastFeedTime = DateTime.Now;
            resMsg += pet.ExtGain(MsgDTO, item.Exp);
            honorRecord.ItemConsume(item.Name);
            honorRecord.Update();

            MsgSender.PushMsg(MsgDTO, resMsg);
            return true;
        }

        private static bool FeedPetWithDiet(MsgInformationEx MsgDTO, PetRecord pet, CookingDietModel diet)
        {
            if (!diet.Attributes.Contains(pet.Attribute))
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}说不想吃这个东西（请喂食正确特性的菜肴）");
                return false;
            }

            var cookingRec = CookingRecord.Get(MsgDTO.FromQQ);
            if (!cookingRec.CheckDiet(diet.Name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有该菜肴！");
                return false;
            }

            var resMsg = $"{pet.Name}兴奋的吃掉了 {diet.Name}，并打了个饱嗝\r";

            pet.LastFeedTime = DateTime.Now;
            resMsg += pet.ExtGain(MsgDTO, diet.Exp);
            cookingRec.DietConsume(diet.Name);
            cookingRec.Update();

            MsgSender.PushMsg(MsgDTO, resMsg);
            return true;
        }

        [EnterCommand(ID = "PetAI_PetLevelAnalyze",
            Command = "宠物等级分布",
            AuthorityLevel = AuthorityLevel.开发者,
            Description = "查看宠物的等级分布",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool PetLevelAnalyze(MsgInformationEx MsgDTO, object[] param)
        {
            var data = PetRecord.LevelAnalyze();
            var msg = string.Join("\r", data.Select(p => $"{p.Key}:{p.Value}"));

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "PetAI_PetLevelRank",
            Command = "宠物等级排行",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "宠物等级排行",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = true)]
        public bool PetLevelRank(MsgInformationEx MsgDTO, object[] param)
        {
            var data = PetRecord.LevelTop(10);
            var msg = string.Join("\r", data.Select((p, idx) => $"{idx + 1}:{p.Name}(lv.{p.Level})({p.Exp}/{PetLevelSvc[p.Level].Exp})"));

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "PetAI_ViewPetSkill",
            Command = "查看宠物技能",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "查看指定的宠物技能详细情况",
            Syntax = "[技能名]",
            Tag = "宠物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool ViewPetSkill(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var skill = PetSkillSvc[name];
            if (skill == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该技能！", true);
                return false;
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var msg = $"名称：{skill.Name}\r" +
                      $"描述：{skill.CommDesc}\r" +
                      $"解锁：{skill.LearnLevel}\r" +
                      $"当前：{(pet.Skills != null && pet.Skills.ContainsKey(name) ? pet.Skills[name] : 0)}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "PetAI_UpgradePetSkill",
            Command = "升级宠物技能",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "将指定的宠物技能等级提升一点（只能升级已经学会的技能）（最高5级）",
            Syntax = "[技能名]",
            Tag = "宠物功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool UpgradePetSkill(MsgInformationEx MsgDTO, object[] param)
        {
            var name = param[0] as string;
            var skill = PetSkillSvc[name];
            if (skill == null)
            {
                MsgSender.PushMsg(MsgDTO, "未查找到该技能！", true);
                return false;
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var expRec = ExpeditionRecord.GetLastest(MsgDTO.FromQQ);
            if (expRec != null && expRec.IsExpediting)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}正在【{expRec.Scene}】进行一项伟大的远征，请于{expRec.EndTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}后再试！");
                return false;
            }

            if (pet.Skills.IsNullOrEmpty() || !pet.Skills.ContainsKey(name))
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}尚未学习该技能！", true);
                return false;
            }

            if (pet.Skills[name] >= 5)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}的该技能已经升到了满级！", true);
                return false;
            }

            if (pet.RemainSkillPoints <= 0)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}没有可用的技能点！", true);
                return false;
            }

            pet.Skills[name]++;
            pet.RemainSkillPoints--;
            pet.Update();

            MsgSender.PushMsg(MsgDTO, $"恭喜{pet.Name}的{name}技能成功升到了{pet.Skills[name]}级！");
            return true;
        }

        [EnterCommand(ID = "PetAI_ResetPetSkill",
            Command = "重置宠物技能",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "重置宠物的所有技能加点",
            Syntax = "",
            Tag = "宠物功能",
            SyntaxChecker = "Empty",
            IsPrivateAvailable = false)]
        public bool ResetPetSkill(MsgInformationEx MsgDTO, object[] param)
        {
            const int ResetSkillCost = 100;
            var osPerson = OSPerson.GetPerson(MsgDTO.FromQQ);
            if (osPerson.Golds < ResetSkillCost)
            {
                MsgSender.PushMsg(MsgDTO, $"金币余额不足({osPerson.Golds.CurencyFormat()}/{ResetSkillCost.CurencyFormat()})");
                return false;
            }

            if (!WaiterSvc.WaitForConfirm_Gold(MsgDTO, ResetSkillCost, 10))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            var expRec = ExpeditionRecord.GetLastest(MsgDTO.FromQQ);
            if (expRec != null && expRec.IsExpediting)
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}正在【{expRec.Scene}】进行一项伟大的远征，请于{expRec.EndTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}后再试！");
                return false;
            }

            pet.SkillReset();
            pet.Update();

            osPerson.Golds -= ResetSkillCost;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "重置成功！");
            return true;
        }

        [EnterCommand(ID = "PetAI_Fight",
            Command = "宠物对决",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "邀请指定群员进行宠物对决",
            Syntax = "[@QQ号]",
            Tag = "宠物功能",
            SyntaxChecker = "At",
            IsPrivateAvailable = false,
            DailyLimit = 1,
            TestingDailyLimit = 3)]
        public bool Fight(MsgInformationEx MsgDTO, object[] param)
        {
            var aimQQ = (long) param[0];
            if (aimQQ == MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你无法挑战你自己！");
                return false;
            }

            if (!PetAgainstSvc.CheckGroup(MsgDTO.FromGroup))
            {
                MsgSender.PushMsg(MsgDTO, "本群正在进行一场宠物对决，请稍后再试！");
                return false;
            }

            if (!PetAgainstSvc.CheckQQ(MsgDTO.FromQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你的宠物正在进行一场宠物对决，请稍后再试！");
                return false;
            }

            if (!PetAgainstSvc.CheckQQ(aimQQ))
            {
                MsgSender.PushMsg(MsgDTO, "你的对手正在进行一场宠物对决，请稍后再试！");
                return false;
            }

            if (BindAiSvc.AllAiNums.Contains(aimQQ))
            {
                MsgSender.PushMsg(MsgDTO, "鱼唇的人类，你无法挑战AI的威严！", true);
                return false;
            }

            var sourcePet = PetRecord.Get(MsgDTO.FromQQ);
            var expRec = ExpeditionRecord.GetLastest(MsgDTO.FromQQ);
            if (expRec != null && expRec.IsExpediting)
            {
                MsgSender.PushMsg(MsgDTO, $"{sourcePet.Name}正在【{expRec.Scene}】进行一项伟大的远征，请于{expRec.EndTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}后再试！");
                return false;
            }

            if (sourcePet.Level < 3)
            {
                MsgSender.PushMsg(MsgDTO, $"{sourcePet.Name}还没到3级，无法参加宠物对决！");
                return false;
            }

            var aimPet = PetRecord.Get(aimQQ);
            if (aimPet.Level < 3)
            {
                MsgSender.PushMsg(MsgDTO, "对方的宠物还没到3级，无法参加宠物对决！");
                return false;
            }

            expRec = ExpeditionRecord.GetLastest(aimQQ);
            if (expRec != null && expRec.IsExpediting)
            {
                MsgSender.PushMsg(MsgDTO, $"{aimPet.Name}正在【{expRec.Scene}】进行一项伟大的远征，请于{expRec.EndTime.ToLocalTime():yyyy-MM-dd HH:mm:ss}后再试！");
                return false;
            }

            if(!WaiterSvc.WaitForConfirm(MsgDTO.FromGroup, aimQQ, "你被邀请参加一场宠物对决，是否同意？", MsgDTO.BindAi, 10))
            {
                MsgSender.PushMsg(MsgDTO, "对决取消！");
                return false;
            }

            PetAgainstSvc.StartGame(sourcePet, aimPet, MsgDTO.FromGroup, MsgDTO.BindAi);
            return true;
        }
    }
}
