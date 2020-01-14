using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.WorldLine.Standard.Ai.Sys.Version
{
    public class VersionModel : INamedJsonModel
    {
        public string Name { get; set; }

        public string VersionNum => Name;

        public string PublishDate { get; set; }

        public string[] WhatsNewList { get; set; }

        public override string ToString()
        {
            var newListMsg = string.Join("\r", WhatsNewList.Select((n, idx) => $"{idx + 1}.{n}"));
            var msg = $"版本号：{VersionNum}\r发布日期：{PublishDate}\r发布内容：\r{newListMsg}";
            return msg;
        }
    }
}
