using System.IO;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Ai.Core.Common;

namespace Dolany.Ai.Core.Ai.Game.Pet
{
    [AI(Name = "宠物",
        Description = "AI for Petting.",
        Enable = true,
        PriorityLevel = 10,
        NeedManulOpen = true)]
    public class PetAI : AIBase
    {
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

            var msg = $"{CodeApi.Code_Image_Relational(pet.PicPath)}\r" + $"名称：{pet.Name}\r" + $"种族：{pet.PetNo}\r" + $"等级：{Utility.LevelEmoji(pet.Level)}\r" +
                      $"经验值：{pet.Exp}";

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "PetAI_RenamePet",
            Command = "重命名宠物",
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
            Command = "设定宠物种族",
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

            const string cachePath = "./images/Cache/";
            var fileName = $"{MsgDTO.FromQQ}.{imageCache?.type}";
            if (!Utility.DownloadImage(imageCache?.url, cachePath + fileName))
            {
                MsgSender.PushMsg(MsgDTO, "图片下载失败，请稍后再试！");
                return false;
            }

            var picFile = new FileInfo(cachePath + fileName);
            if (picFile.Length / 1024 > 300)
            {
                MsgSender.PushMsg(MsgDTO, "图片过大，请重新上传！", true);
                picFile.Delete();
                return false;
            }

            var path = $"./images/Custom/Pet/{MsgDTO.FromQQ}.{imageCache?.type}";
            File.Copy(cachePath + fileName, path, true);
            picFile.Delete();

            var pet = PetRecord.Get(MsgDTO.FromQQ);
            pet.PicPath = path;
            pet.Update();

            MsgSender.PushMsg(MsgDTO, "上传成功！");
            return true;
        }
    }
}
