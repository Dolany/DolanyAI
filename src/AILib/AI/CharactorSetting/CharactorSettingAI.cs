using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;
using System.ComponentModel.Composition;

namespace AILib
{
    [AI(
        Name = "CharactorSettingAI",
        Description = "AI for Setting a Charactor.",
        IsAvailable = true,
        PriorityLevel = 10
        )]
    public class CharactorSettingAI : AIBase
    {
        private int MaxCharNumPerQQ = 10;
        private int MaxSettingPerChar = 7;

        public CharactorSettingAI()
            : base()
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
            string charactor = param[0] as string;
            string settingName = param[1] as string;
            string content = param[2] as string;

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
            string charactor = param[0] as string;
            var query = DbMgr.Query<CharactorSettingEntity>(c => c.GroupNumber == MsgDTO.FromGroup && c.Charactor == charactor);
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

            DbMgr.Delete<CharactorSettingEntity>(c => c.GroupNumber == MsgDTO.FromGroup && c.Charactor == charactor);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "删除成功！"
            });
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
            string charactor = param[0] as string;
            var query = DbMgr.Query<CharactorSettingEntity>(c => c.GroupNumber == MsgDTO.FromGroup && c.Charactor == charactor);
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

            string msg = charactor + ':';
            foreach (var c in query)
            {
                msg += '\r' + c.SettingName + ':' + c.Content;
            }

            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = msg
            });
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
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == MsgDTO.FromGroup && cs.Charactor == charactor);
            return query.First().Creator == MsgDTO.FromQQ;
        }

        private bool IsSettingExist(long fromGroup, string charactor, string settingName)
        {
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == fromGroup && cs.Charactor == charactor && cs.SettingName == settingName);
            return !query.IsNullOrEmpty();
        }

        private bool IsQQFullChar(GroupMsgDTO MsgDTO)
        {
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == MsgDTO.FromGroup && cs.Creator == MsgDTO.FromQQ);
            if (query.IsNullOrEmpty())
            {
                return false;
            }
            query = query.GroupBy(p => p.Charactor).Select(p => p.First());
            return !query.IsNullOrEmpty() && query.Count() > MaxCharNumPerQQ;
        }

        private bool IsSettingFull(long fromGroup, string charactor, string settingName)
        {
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == fromGroup && cs.Charactor == charactor && cs.SettingName != settingName);
            return !query.IsNullOrEmpty() && query.Count() > MaxSettingPerChar;
        }

        private void InsertSetting(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            CharactorSettingEntity cs = new CharactorSettingEntity
            {
                Id = Guid.NewGuid().ToString(),
                CreateTime = DateTime.Now,
                GroupNumber = MsgDTO.FromGroup,
                Creator = MsgDTO.FromQQ,
                Charactor = charactor,
                SettingName = settingName,
                Content = content
            };

            DbMgr.Insert(cs);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "设定成功！"
            });
        }

        private void ModifySetting(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            var cs = DbMgr.Query<CharactorSettingEntity>(c => c.GroupNumber == MsgDTO.FromGroup
                && c.Charactor == charactor
                && c.SettingName == settingName)
                .FirstOrDefault();
            cs.Content = content;
            DbMgr.Update(cs);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.FromGroup,
                Type = MsgType.Group,
                Msg = "修改设定成功！"
            });
        }

        private bool IsExistCharactor(long groupNumber, string charactor)
        {
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == groupNumber && cs.Charactor == charactor);
            return !query.IsNullOrEmpty();
        }
    }
}