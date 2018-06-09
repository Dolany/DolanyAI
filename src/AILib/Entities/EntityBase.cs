using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AILib.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PrimaryKeyAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Property)]
    public class ContentAttribute : Attribute
    {

    }

    public class EntityBase
    {

    }
}
