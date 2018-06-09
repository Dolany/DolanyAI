using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Xml.Linq;
using System.Xml;
using System.IO;
using System.Linq.Expressions;

namespace AILib.Entities
{
    public static class DbMgr
    {
        private static string DateFolderPath = "./Data/";

        public static void InitXml()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Type[] typeArr = assembly.GetTypes();

            foreach (Type t in typeArr)
            {
                if (t.BaseType != typeof(EntityBase))
                {
                    continue;
                }

                string EntityName = t.Name.Replace("Entity", "");
                string FilePath = DateFolderPath + EntityName + ".xml";
                FileInfo fileInfo = new FileInfo(FilePath);
                if (fileInfo.Exists)
                {
                    return;
                }

                try
                {
                    XmlDocument doc = new XmlDocument();
                    XmlElement root = doc.CreateElement(EntityName);
                    doc.AppendChild(root);

                    doc.Save(FilePath);
                }
                catch (Exception ex)
                {
                    Common.SendMsgToDeveloper(ex);
                }
            }
        }

        public static void Add(EntityBase entity)
        {
            // TODO
        }

        public static void Modify(EntityBase entity)
        {
            // TODO
        }

        public static void Delete(EntityBase entity)
        {
            // TODO
        }

        public static Entity Get<Entity>(string Id) where Entity : EntityBase
        {
            // TODO

            return null;
        }

        public static IEnumerable<Entity> Query<Entity>(Expression<Func<Entity, bool>> express) where Entity : EntityBase
        {
            // TODO

            return null;
        }
    }
}
