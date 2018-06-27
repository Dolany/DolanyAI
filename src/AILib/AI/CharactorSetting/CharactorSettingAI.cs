using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Flexlive.CQP.Framework.Utils;
using AILib.Entities;

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

        public CharactorSettingAI(AIConfigDTO ConfigDTO)
            : base(ConfigDTO)
        {
            RuntimeLogger.Log("CharactorSettingAI constructed");
        }

        public override void Work()
        {
        }

        [EnterCommand(
            Command = "人物设定",
            SourceType = MsgType.Group,
            AuthorityLevel = AuthorityLevel.成员,
            Description = "设定一个人物",
            Syntax = "[人物名] [设定项] [设定内容]",
            Tag = "设定",
            SyntaxChecker = "ThreeWords"
            )]
        public void SetCharactor(GroupMsgDTO MsgDTO, object[] param)
        {
            string charactor = param[0] as string;
            string settingName = param[1] as string;
            string content = param[2] as string;

            if (!IsExistCharactor(MsgDTO.fromGroup, charactor))
            {
                TryToInsertChar(MsgDTO, charactor, settingName, content);
            }
            else
            {
                TryToUpdateChar(MsgDTO, charactor, settingName, content);
            }
        }

        private void TryToInsertChar(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            if (!IsCharactorCreator(MsgDTO, charactor))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = $"只能修改自己创建的人物哦~"
                });
            }
            else if (IsQQFullChar(MsgDTO))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.fromGroup,
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
            if (IsSettingFull(MsgDTO.fromGroup, charactor, settingName))
            {
                MsgSender.Instance.PushMsg(new SendMsgDTO
                {
                    Aim = MsgDTO.fromGroup,
                    Type = MsgType.Group,
                    Msg = $"每个人物只能设定{MaxSettingPerChar}个属性哦~"
                });
                return;
            }

            if (IsSettingExist(MsgDTO.fromGroup, charactor, settingName))
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
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == MsgDTO.fromGroup && cs.Charactor == charactor);
            return query.First().Creator == MsgDTO.fromQQ;
        }

        private bool IsSettingExist(long fromGroup, string charactor, string settingName)
        {
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == fromGroup && cs.Charactor == charactor && cs.SettingName == settingName);
            return !query.IsNullOrEmpty();
        }

        private bool IsQQFullChar(GroupMsgDTO MsgDTO)
        {
            var query = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == MsgDTO.fromGroup && cs.Creator == MsgDTO.fromQQ)
                .GroupBy(p => p.Charactor)
                .Select(p => p.First());
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
                GroupNumber = MsgDTO.fromGroup,
                Creator = MsgDTO.fromQQ,
                Charactor = charactor,
                SettingName = settingName,
                Content = content
            };

            DbMgr.Insert(cs);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.fromGroup,
                Type = MsgType.Group,
                Msg = "设定成功！"
            });
        }

        private void ModifySetting(GroupMsgDTO MsgDTO, string charactor, string settingName, string content)
        {
            var cs = DbMgr.Query<CharactorSettingEntity>(cs => cs.GroupNumber == fromGroup && cs.Charactor == charactor && cs.SettingName == settingName).FirstOrDefault();
            cs.Content = content;
            DbMgr.Update(cs);
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = MsgDTO.fromGroup,
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