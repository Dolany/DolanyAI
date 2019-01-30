using System;

namespace Dolany.Game.Engine
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SavePointAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
