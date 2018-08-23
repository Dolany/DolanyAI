using System;
using System.Linq;
using System.Text;
using Dolany.Ice.Ai.DolanyAI.Db;

namespace Dolany.Ice.Ai.DolanyAI
{
    [AI(
        Name = nameof(CharactorSettingAI),
        Description = "AI for Setting a Charactor.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class CharactorSettingAI : AIBase
    {
        private int MaxCharNumPerQQ = 10;
        private int MaxSettingPerChar = 7;

        public CharactorSettingAI()
        {
            RuntimeLogger.Log("CharactorSettingAI constructed");
        }

        public override void Work()
        {
        }

        [GroupEnterCommand(
            Command = "人物设定",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定一个人物",
            Syntax = "[人物名] [设定项] [设定内容]",
            Tag = "设定功能",
            SyntaxChecker = "ThreeWords"
            )]
        public void SetCharactor(GroupMsgDTO MsgDTO, object[] param)
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
        }

        [GroupEnterCommand(
            Command = "删除人物",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "删除一个人物",
            Syntax = "[人物名]",
            Tag = "设定功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void DeleteCharactor(GroupMsgDTO MsgDTO, object[] param)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var charactor = param[0] as string;
                var query = db.CharactorSetting.Where(c => c.GroupNumber == MsgDTO.FromGroup && c.Charactor == charactor);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "这个人物还没有被创建呢！"
                    });
                    return;
                }

                if (query.First().Creator != MsgDTO.FromQQ)
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = "你只能删除自己创建的人物噢！"
                    });
                    return;
                }

                foreach (var c in query)
                {
                    db.CharactorSetting.Remove(c);
                }
                db.SaveChanges();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "删除成功！"
                });
            }
        }

        [GroupEnterCommand(
            Command = "人物设定浏览",
            AuthorityLevel = AuthorityLevel.成员,
            Description = "浏览一个人物的全部设定",
            Syntax = "[人物名]",
            Tag = "设定功能",
            SyntaxChecker = "NotEmpty"
            )]
        public void ViewCharactor(GroupMsgDTO MsgDTO, object[] param)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var charactor = param[0] as string;
                var query = db.CharactorSetting.Where(c => c.GroupNumber == MsgDTO.FromGroup && c.Charactor == charactor);
                if (query.IsNullOrEmpty())
                {
                    MsgSender.Instance.PushMsg(new SendMsgDTO
                    {
                        Aim = MsgDTO.FromGroup,
                        Type = MsgType.Group,
                        Msg = $"这个人物还没有创建哦~"
                    });
                    return;
                }

                var msg = charactor + ':';
                var builder = new StringBuilder();
                builder.Append(msg);
                foreach (var c in query)
                {
                    builder.Append('\r' + c.SettingName + ':' + c.Content);
                }
                msg = builder.ToString();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = msg
                });
            }
        }

        private void TryToInsertChar(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            if (IsQQFullChar(MsgDTO))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"每个QQ号只能设定{MaxCharNumPerQQ}个人物哦~"
                });
            }
            else
            {
                InsertSetting(MsgDTO, charactor, settingName, content);
            }
        }

        private void TryToUpdateChar(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            if (!IsCharactorCreator(MsgDTO, charactor))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"只能修改自己创建的人物哦~"
                });
                return;
            }

            if (IsSettingFull(MsgDTO.FromGroup, charactor, settingName))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = $"每个人物只能设定{MaxSettingPerChar}个属性哦~"
                });
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

        private bool IsCharactorCreator(GroupMsgDTO MsgDTO, string charactor)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.CharactorSetting.Where(cs => cs.GroupNumber == MsgDTO.FromGroup && cs.Charactor == charactor);
                return query.First().Creator == MsgDTO.FromQQ;
            }
        }

        private bool IsSettingExist(long fromGroup, string charactor, string settingName)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.CharactorSetting.Where(cs => cs.GroupNumber == fromGroup
                                                            && cs.Charactor == charactor
                                                            && cs.SettingName == settingName
                                                     );
                return !query.IsNullOrEmpty();
            }
        }

        private bool IsQQFullChar(GroupMsgDTO MsgDTO)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.CharactorSetting.Where(cs => cs.GroupNumber == MsgDTO.FromGroup && cs.Creator == MsgDTO.FromQQ);
                if (query.IsNullOrEmpty())
                {
                    return false;
                }
                query = query.GroupBy(p => p.Charactor).Select(p => p.First());
                return !query.IsNullOrEmpty() && query.Count() > MaxCharNumPerQQ;
            }
        }

        private bool IsSettingFull(long fromGroup, string charactor, string settingName)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.CharactorSetting.Where(cs => cs.GroupNumber == fromGroup
                                                            && cs.Charactor == charactor
                                                            && cs.SettingName != settingName
                                                     );
                return !query.IsNullOrEmpty() && query.Count() > MaxSettingPerChar;
            }
        }

        private void InsertSetting(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var cs = new CharactorSetting
                {
                    Id = Guid.NewGuid().ToString(),
                    CreateTime = DateTime.Now,
                    GroupNumber = MsgDTO.FromGroup,
                    Creator = MsgDTO.FromQQ,
                    Charactor = charactor,
                    SettingName = settingName,
                    Content = content
                };

                db.CharactorSetting.Add(cs);
                db.SaveChanges();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "设定成功！"
                });
            }
        }

        private void ModifySetting(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var cs = db.CharactorSetting.FirstOrDefault(c => c.GroupNumber == MsgDTO.FromGroup
                && c.Charactor == charactor
                && c.SettingName == settingName)
;

                if (cs != null) cs.Content = content;
                db.SaveChanges();

                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.FromGroup,
                    Type = MsgType.Group,
                    Msg = "修改设定成功！"
                });
            }
        }

        private bool IsExistCharactor(long groupNumber, string charactor)
        {
            using (AIDatabase db = new AIDatabase())
            {
                var query = db.CharactorSetting.Where(cs => cs.GroupNumber == groupNumber && cs.Charactor == charactor);
                return !query.IsNullOrEmpty();
            }
        }
    }
}