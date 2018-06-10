using System;
using System.Xml;
using System.Reflection;

namespace AILib.Entities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class DataColumnAttribute : Attribute
    {
    }

    public class EntityBase
    {
        [DataColumn]
        public string Id { get; set; }
        public string Content { get; set; }

        public string EntityName
        {
            get
            {
                Type t = this.GetType();
                string typeName = t.Name;
                return t.Name.Contains("Entity") ? t.Name.Replace("Entity", "") : t.Name;
            }
        }

        public XmlElement ToElement(XmlDocument doc)
        {
            XmlElement ele = doc.CreateElement(EntityName);
            ele.Value = Content;

            Type t = this.GetType();
            foreach(var prop in t.GetProperties())
            {
                if(prop.GetCustomAttributes(typeof(DataColumnAttribute), false).Length <= 0)
                {
                    continue;
                }
                var propValue = t.InvokeMember(prop.Name,
                    BindingFlags.GetProperty,
                    null,
                    this,
                    null
                    );
                ele.SetAttribute(prop.Name, propValue as string);
            }

            return ele;
        }

        public static Entity FromElement<Entity>(XmlElement ele) where Entity : EntityBase, new()
        {
            Entity entity = new Entity();
            if(entity.EntityName != ele.Name)
            {
                return null;
            }

            entity.Content = ele.Value;
            Type t = typeof(Entity);
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(DataColumnAttribute), false).Length <= 0)
                {
                    continue;
                }
                t.InvokeMember(prop.Name,
                    BindingFlags.SetProperty,
                    null,
                    entity,
                    new object[] {ele.GetAttribute(prop.Name) }
                    );
            }

            return entity;
        }
    }
}