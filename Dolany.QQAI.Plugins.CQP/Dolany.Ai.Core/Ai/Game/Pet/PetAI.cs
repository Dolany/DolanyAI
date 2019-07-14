using System;
using System.IO;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;
using Dolany.Ai.Core.OnlineStore;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    public class PetAI : AIBase
    {
        public override string AIName { get; set; } = "宠物";

        public override string Description { get; set; } = "AI for Petting.";

        public override int PriorityLevel { get; set; } = 10;

        public override bool NeedManualOpeon { get; set; } = true;

        private const string CachePath = "./images/Cache/";

        private const int FeedInterval = 2;

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

            var levelModel = PetLevelMgr.Instance[pet.Level];
            var msg = $"{CodeApi.Code_Image_Relational(pet.PicPath)}\r" +
                      $"名称：{pet.Name}\r" +
                      $"种族：{pet.PetNo}\r" +
                      $"食性：{pet.Attribute ?? "无"}\r" +
                      $"等级：{Utility.LevelEmoji(pet.Level)}\r" +
                      $"经验值：{pet.Exp}/{levelModel.Exp}";
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
            SyntaxChecker = "Word",
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
            SyntaxChecker = "Word",
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
            if (osPerson.Golds < 500)
            {
                MsgSender.PushMsg(MsgDTO, $"你的金币余额不足（{osPerson.Golds}/500）");
                return false;
            }

            if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 500))
            {
                MsgSender.PushMsg(MsgDTO, "操作取消！");
                return false;
            }

            var info = Waiter.Instance.WaitForInformation(MsgDTO, "请上传图片（不能超过300KB）！",
                information => information.FromGroup == MsgDTO.FromGroup && information.FromQQ == MsgDTO.FromQQ &&
                               !string.IsNullOrEmpty(Utility.ParsePicGuid(information.Msg)), 10);
            if(info == null)
            {
                MsgSender.PushMsg(MsgDTO, "操作取消!");
                return false;
            }

            var bindai = BindAiMgr.Instance[MsgDTO.BindAi];

            var picGuid = Utility.ParsePicGuid(info.Msg);
            var imageCache = Utility.ReadImageCacheInfo(picGuid, bindai.ImagePath);

            if (imageCache?.type.ToLower() != "jpg")
            {
                MsgSender.PushMsg(MsgDTO, "抱歉，暂时只支持jpg格式的图片！");
                return false;
            }

            var fileName = $"{MsgDTO.FromQQ}.{imageCache.type}";
            if (!Utility.DownloadImage(imageCache.url, CachePath + fileName))
            {
                MsgSender.PushMsg(MsgDTO, "图片下载失败，请稍后再试！");
                return false;
            }

            var picFile = new FileInfo(CachePath + fileName);
            if (picFile.Length / 1024 > 300)
            {
                MsgSender.PushMsg(MsgDTO, "图片过大，请重新上传！", true);
                picFile.Delete();
                return false;
            }

            osPerson.Golds -= 500;
            osPerson.Update();

            MsgSender.PushMsg(MsgDTO, "上传成功！待审核通过后方可生效！");
            Confirm(MsgDTO);

            return true;
        }

        private static void Confirm(MsgInformation MsgDTO)
        {
            var folder = new DirectoryInfo(CachePath);
            var count = folder.GetFiles().Length;

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            var msg = $"有新的待审核图片！\r来自 {setting.Name} 的 {MsgDTO.FromQQ}\r当前剩余 {count} 张图片待审核！";
            MsgSender.PushMsg(0, Global.DeveloperNumber, msg, Global.DefaultConfig.MainAi);
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
                    MsgSender.PushMsg(MsgDTO, $"金币余额不足！({osPerson.Golds}/300)");
                    return false;
                }

                if (!Waiter.Instance.WaitForConfirm_Gold(MsgDTO, 300))
                {
                    MsgSender.PushMsg(MsgDTO, "操作取消");
                    return false;
                }

                needGolds = true;
            }

            var randAttrs = CommonUtil.RandSort(PetExtent.AllAttributes.ToArray());
            var msg = $"请选择宠物食性：\r{string.Join("\r", randAttrs.Select((p, idx) => $"{idx + 1}:{p}"))}";
            var selectedIdx = Waiter.Instance.WaitForNum(MsgDTO.FromGroup, MsgDTO.FromQQ, msg, i => i > 0 && i <= randAttrs.Length, MsgDTO.BindAi);
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
            var honorRecord = ItemCollectionRecord.Get(MsgDTO.FromQQ);
            if (!honorRecord.CheckItem(name))
            {
                MsgSender.PushMsg(MsgDTO, "你没有该物品！", true);
                return false;
            }

            var pet = PetRecord.Get(MsgDTO.FromQQ);
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

            var item = HonorHelper.Instance.FindItem(name);
            if (item?.Attributes == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到该物品");
                return false;
            }

            if (!item.Attributes.Contains(pet.Attribute))
            {
                MsgSender.PushMsg(MsgDTO, $"{pet.Name}说不想吃这个东西（请喂食正确特性的物品）");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, $"{pet.Name}兴奋的吃掉了 {name}，并打了个饱嗝");

            pet.LastFeedTime = DateTime.Now;
            pet.ExtGain(MsgDTO, item.Exp);
            honorRecord.ItemConsume(name);
            honorRecord.Update();

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
    }
}
