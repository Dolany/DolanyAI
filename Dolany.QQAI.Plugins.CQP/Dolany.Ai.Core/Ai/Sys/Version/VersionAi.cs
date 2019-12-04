using System.Collections.Generic;
using System.Linq;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.Ai.Core.Base;

namespace Dolany.Ai.Core.Ai.Sys.Version
{
    public class VersionAi : AIBase
    {
        public override string AIName { get; set; } = "版本信息";
        public override string Description { get; set; } = "Ai for showing verion info";
        public override int PriorityLevel { get; set; } = 10;

        private List<VersionModel> Versions = new List<VersionModel>();

        public override void Initialization()
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
            // todo
            return true;
        }
    }
}
