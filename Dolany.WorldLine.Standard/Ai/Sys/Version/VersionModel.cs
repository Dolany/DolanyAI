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
            var newListMsg = string.Join("\r\n", WhatsNewList.Select((n, idx) => $"{idx + 1}.{n}"));
            var msg = $"版本号：{VersionNum}\r\n发布日期：{PublishDate}\r\n发布内容：\r\n{newListMsg}";
            return msg;
        }
    }
}
