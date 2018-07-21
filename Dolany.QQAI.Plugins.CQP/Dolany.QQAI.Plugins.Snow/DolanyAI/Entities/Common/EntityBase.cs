using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Threading.Tasks;

namespace Dolany.QQAI.Plugins.Snow.DolanyAI
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

        public XElement ToElement()
        {
            XElement ele = new XElement(EntityName);
            ele.SetValue(Content);

            Type t = this.GetType();
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
            Entity entity = new Entity();
            if (entity.EntityName != ele.Name)
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

                string attrValue = ele.Attribute(prop.Name).Value;
                prop.SetValue(entity, Convert.ChangeType(attrValue, prop.PropertyType));
            }

            return entity;
        }
    }
}