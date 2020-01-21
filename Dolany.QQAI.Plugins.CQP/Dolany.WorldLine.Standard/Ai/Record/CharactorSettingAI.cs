using System;
using System.Linq;
using System.Text;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;
using Dolany.Database;
using Dolany.Database.Ai;

namespace Dolany.WorldLine.Standard.Ai.Record
{
    public class CharactorSettingAI : AIBase
    {
        public override string AIName { get; set; } = "人物设定";

        public override string Description { get; set; } = "AI for Setting a Charactor.";

        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        private const int MaxCharNumPerQQ = 10;
        private const int MaxSettingPerChar = 7;

        [EnterCommand(ID = "CharactorSettingAI_SetCharactor",
            Command = "人物设定",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定一个人物",
            Syntax = "[人物名] [设定项] [设定内容]",
            Tag = "设定功能",
            SyntaxChecker = "Word Word Word",
            IsPrivateAvailable = false)]
        public bool SetCharactor(MsgInformationEx MsgDTO, object[] param)
        {
            var charactor = param[0] as string;
            var settingName = param[1] as string;
            var content = param[2] as string;

            if (!IsExistCharactor(MsgDTO.FromGroup, charactor))
            {
                TryToInsertChar(MsgDTO, charactor, settingName, content);
            }
            else
            {
                TryToUpdateChar(MsgDTO, charactor, settingName, content);
            }

            return true;
        }

        [EnterCommand(ID = "CharactorSettingAI_DeleteCharactor",
            Command = "删除人物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除一个人物",
            Syntax = "[人物名]",
            Tag = "设定功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool DeleteCharactor(MsgInformationEx MsgDTO, object[] param)
        {
            var charactor = param[0] as string;

            var query = MongoService<CharactorSetting>.Get(c => c.GroupNumber == MsgDTO.FromGroup &&
                                                                c.Charactor == charactor);
            if (query.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "这个人物还没有被创建呢！");
                return false;
            }

            if (query.First().Creator != MsgDTO.FromQQ)
            {
                MsgSender.PushMsg(MsgDTO, "你只能删除自己创建的人物噢！");
                return false;
            }

            foreach (var c in query)
            {
                MongoService<CharactorSetting>.Delete(c);
            }

            MsgSender.PushMsg(MsgDTO, "删除成功！");
            return true;
        }

        [EnterCommand(ID = "CharactorSettingAI_ViewCharactor",
            Command = "人物设定浏览",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "浏览一个人物的全部设定",
            Syntax = "[人物名]",
            Tag = "设定功能",
            SyntaxChecker = "Word",
            IsPrivateAvailable = false)]
        public bool ViewCharactor(MsgInformationEx MsgDTO, object[] param)
        {
            var charactor = param[0] as string;

            var query = MongoService<CharactorSetting>.Get(c => c.GroupNumber == MsgDTO.FromGroup &&
                                                                c.Charactor == charactor);
            if (query.IsNullOrEmpty())
            {
                MsgSender.PushMsg(MsgDTO, "这个人物还没有创建哦~");
                return false;
            }

            var msg = charactor + ':';
            var builder = new StringBuilder();
            builder.Append(msg);
            foreach (var c in query)
            {
                builder.Append('\r' + c.SettingName + ':' + c.Content);
            }
            msg = builder.ToString();

            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        private static void TryToInsertChar(MsgInformationEx MsgDTO, string charactor, string settingName, string content)
        {
            if (IsQQFullChar(MsgDTO))
            {
                MsgSender.PushMsg(MsgDTO, $"每个QQ号只能设定{MaxCharNumPerQQ}个人物哦~");
            }
            else
            {
                InsertSetting(MsgDTO, charactor, settingName, content);
            }
        }

        private static void TryToUpdateChar(MsgInformationEx MsgDTO, string charactor, string settingName, string content)
        {
            if (!IsCharactorCreator(MsgDTO, charactor))
            {
                MsgSender.PushMsg(MsgDTO, "只能修改自己创建的人物哦~");
                return;
            }

            if (IsSettingFull(MsgDTO.FromGroup, charactor, settingName))
            {
                MsgSender.PushMsg(MsgDTO, $"每个人物只能设定{MaxSettingPerChar}个属性哦~");
                return;
            }

            if (IsSettingExist(MsgDTO.FromGroup, charactor, settingName))
            {
                ModifySetting(MsgDTO, charactor, settingName, content);
            }
            else
            {
                InsertSetting(MsgDTO, charactor, settingName, content);
            }
        }

        private static bool IsCharactorCreator(MsgInformation MsgDTO, string charactor)
        {
            var query = MongoService<CharactorSetting>.Get(cs => cs.GroupNumber == MsgDTO.FromGroup &&
                                                                 cs.Charactor == charactor);

            return query.First().Creator == MsgDTO.FromQQ;
        }

        private static bool IsSettingExist(long fromGroup, string charactor, string settingName)
        {
            var query = MongoService<CharactorSetting>.Get(cs => cs.GroupNumber == fromGroup &&
                                                                 cs.Charactor == charactor &&
                                                                 cs.SettingName == settingName);

            return !query.IsNullOrEmpty();
        }

        private static bool IsQQFullChar(MsgInformation MsgDTO)
        {
            var query = MongoService<CharactorSetting>.Get(cs => cs.GroupNumber == MsgDTO.FromGroup &&
                                                                 cs.Creator == MsgDTO.FromQQ);

            if (query.IsNullOrEmpty())
            {
                return false;
            }
            query = query.GroupBy(p => p.Charactor).Select(p => p.First()).ToList();
            return !query.IsNullOrEmpty() && query.Count > MaxCharNumPerQQ;
        }

        private static bool IsSettingFull(long fromGroup, string charactor, string settingName)
        {
            var query = MongoService<CharactorSetting>.Get(cs => cs.GroupNumber == fromGroup &&
                                                                 cs.Charactor == charactor &&
                                                                 cs.SettingName != settingName);

            return !query.IsNullOrEmpty() && query.Count > MaxSettingPerChar;
        }

        private static void InsertSetting(MsgInformationEx MsgDTO, string charactor, string settingName, string content)
        {
            var cs = new CharactorSetting
            {
                CreateTime = DateTime.Now,
                GroupNumber = MsgDTO.FromGroup,
                Creator = MsgDTO.FromQQ,
                Charactor = charactor,
                SettingName = settingName,
                Content = content
            };
            MongoService<CharactorSetting>.Insert(cs);

            MsgSender.PushMsg(MsgDTO, "设定成功！", true);
        }

        private static void ModifySetting(MsgInformationEx MsgDTO, string charactor, string settingName, string content)
        {
            var cs = MongoService<CharactorSetting>.GetOnly(c => c.GroupNumber == MsgDTO.FromGroup &&
                                                             c.Charactor == charactor &&
                                                             c.SettingName == settingName);

            if (cs != null)
            {
                cs.Content = content;
            }
            MongoService<CharactorSetting>.Update(cs);

            MsgSender.PushMsg(MsgDTO, "修改设定成功！");
        }

        private static bool IsExistCharactor(long groupNumber, string charactor)
        {
            var query = MongoService<CharactorSetting>.Get(cs => cs.GroupNumber == groupNumber &&
                                                                 cs.Charactor == charactor);
            return !query.IsNullOrEmpty();
        }
    }
}
