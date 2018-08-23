using System;
using System.ComponentModel.Composition;

namespace AIMonitor.Tools
{
    [MetadataAttribute]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class ToolAttribute : ExportAttribute
    {
        public ToolAttribute()
            : base(typeof(ToolBaseForm))
        {
        }

        public string ToolName { get; set; }
        public string Decription { get; set; }
        public string ToolIcon { get; set; }
        public bool IsAutoStart { get; set; }
        public int Order { get; set; }
    }

    public interface IToolCapabilities
    {
        string ToolName { get; }
        string Decription { get; }
        string ToolIcon { get; }
        bool IsAutoStart { get; }
        int Order { get; }
    }
}