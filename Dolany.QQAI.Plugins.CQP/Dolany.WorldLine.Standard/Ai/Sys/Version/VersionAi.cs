using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;
using Dolany.Ai.Core.Cache;

namespace Dolany.WorldLine.Standard.Ai.Sys.Version
{
    public class VersionAi : AIBase, IDataMgr
    {
        public override string AIName { get; set; } = "版本信息";
        public override string Description { get; set; } = "Ai for showing verion info";
        public override AIPriority PriorityLevel { get;} = AIPriority.Normal;

        public List<VersionModel> Versions = new List<VersionModel>();
        private static DataRefresher DataRefresher => AutofacSvc.Resolve<DataRefresher>();

        public override void Initialization()
        {
            RefreshData();
            DataRefresher.Register(this);
        }

        public void RefreshData()
        {
            Versions = CommonUtil.ReadJsonData_NamedList<VersionModel>("VersionData");
            Versions = Versions.OrderByDescending(v => v.PublishDate).ToList();
        }

        [EnterCommand(ID = "VersionAi_VersionInfo",
            Command = "版本信息",
            Description = "获取当前的版本信息",
            Syntax = "",
            Tag = "系统命令",
            SyntaxChecker = "Empty",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool VersionInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var msg = Versions.First().ToString();
            MsgSender.PushMsg(MsgDTO, msg);
            return true;
        }

        [EnterCommand(ID = "VersionAi_VersionHistoryInfo",
            Command = "版本信息",
            Description = "获取指定版本的版本信息",
            Syntax = "[版本号]",
            Tag = "系统命令",
            SyntaxChecker = "Word",
            AuthorityLevel = AuthorityLevel.成员,
            IsPrivateAvailable = true)]
        public bool VersionHistoryInfo(MsgInformationEx MsgDTO, object[] param)
        {
            var versionNo = param[0] as string;
            var version = Versions.FirstOrDefault(p => p.VersionNum == versionNo);
            if (version == null)
            {
                MsgSender.PushMsg(MsgDTO, "未找到指定的版本号！");
                return false;
            }

            MsgSender.PushMsg(MsgDTO, version.ToString());
            return true;
        }
    }
}
