using System;
using System.Collections.Generic;
using System.Text;

namespace Dolany.Ai.Core.Entities
{
    using System.Xml.Linq;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DataColumnAttribute : Attribute
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
                var t = GetType();
                return t.Name.Contains("Entity") ? t.Name.Replace("Entity", "") : t.Name;
            }
        }

        public XElement ToElement()
        {
            var ele = new XElement(EntityName);
            ele.SetValue(Content);

            var t = GetType();
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(DataColumnAttribute), false).Length <= 0)
                {
                    continue;
                }

                ele.SetAttributeValue(prop.Name, prop.GetValue(this).ToString());
            }

            return ele;
        }

        public static Entity FromElement<Entity>(XElement ele) where Entity : EntityBase, new()
        {
            var entity = new Entity();
            if (entity.EntityName != ele.Name)
            {
                return null;
            }

            entity.Content = ele.Value;
            var t = typeof(Entity);
            foreach (var prop in t.GetProperties())
            {
                if (prop.GetCustomAttributes(typeof(DataColumnAttribute), false).Length <= 0)
                {
                    continue;
                }

                var attrValue = ele.Attribute(prop.Name)?.Value;
                prop.SetValue(entity, Convert.ChangeType(attrValue, prop.PropertyType));
            }

            return entity;
        }
    }
}
