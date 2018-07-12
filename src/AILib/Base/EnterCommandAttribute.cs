using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.Composition;

namespace AILib
{
    public enum AuthorityLevel
    {
        群主 = 1,
        管理员 = 2,
        成员 = 0
    }

    [MetadataAttribute]
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class GroupEnterCommandAttributeAttribute : ExportAttribute, IGroupEnterCommandCapabilities
    {
        public GroupEnterCommandAttributeAttribute()
            : base("GroupEnterCommand")
        {
        }

        public string Command { get; set; }
        public AuthorityLevel AuthorityLevel { get; set; }
        public string Description { get; set; }
        public string Syntax { get; set; }
        public string Tag { get; set; }
        public string SyntaxChecker { get; set; }
    }

    public interface IGroupEnterCommandCapabilities
    {
        string Command { get; }
        AuthorityLevel AuthorityLevel { get; }
        string Description { get; }
        string Syntax { get; }
        string Tag { get; }
        string SyntaxChecker { get; }
    }

    [MetadataAttribute]
    [System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
    public class PrivateEnterCommandAttributeAttribute : ExportAttribute, IPrivateEnterCommandCapabilities
    {
        public PrivateEnterCommandAttributeAttribute()
            : base("PrivateEnterCommand")
        {
        }

        public string Command { get; set; }
        public bool IsDeveloperOnly { get; set; }
        public string Description { get; set; }
        public string Syntax { get; set; }
        public string Tag { get; set; }
        public string SyntaxChecker { get; set; }
    }

    public interface IPrivateEnterCommandCapabilities
    {
        string Command { get; }
        bool IsDeveloperOnly { get; }
        string Description { get; }
        string Syntax { get; }
        string Tag { get; }
        string SyntaxChecker { get; }
    }
}