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

namespace Dolany.QQAI.Plugins.DolanyAI
{
    public static class DbMgr
    {
        private static string DateFolderPath = "./DbData/";

        public static void InitXmls()
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
                InitXml(EntityName);
            }
        }

        private static void InitXml(string EntityName)
        {
            string FilePath = EntityFilePath(EntityName);
            FileInfo fileInfo = new FileInfo(FilePath);
            if (fileInfo.Exists)
            {
                return;
            }

            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement(EntityName);
            doc.AppendChild(root);

            doc.Save(FilePath);
        }

        public static void Insert(EntityBase entity)
        {
            string EntityName = entity.GetType().Name.Replace("Entity", "");
            InitXml(EntityName);
            XElement root = XElement.Load(EntityFilePath(EntityName));
            XElement ele = entity.ToElement();
            root.Add(ele);
            root.Save(EntityFilePath(EntityName));
        }

        public static bool Update(EntityBase entity)
        {
            string EntityName = entity.GetType().Name.Replace("Entity", "");
            XElement root = XElement.Load(EntityFilePath(EntityName));
            foreach (XElement ele in root.Elements())
            {
                if (entity.Id == ele.Attribute("Id").Value)
                {
                    ele.ReplaceWith(entity.ToElement());
                    root.Save(EntityFilePath(EntityName));
                    return true;
                }
            }

            return false;
        }

        public static bool Delete<Entity>(string Id) where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XElement root = XElement.Load(EntityFilePath(EntityName));
            foreach (XElement ele in root.Elements())
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if (entity == null)
                {
                    continue;
                }
                if (entity.Id == Id)
                {
                    ele.Remove();
                    root.Save(EntityFilePath(EntityName));
                    return true;
                }
            }

            return false;
        }

        public static int Delete<Entity>(Expression<Func<Entity, bool>> express) where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XElement root = XElement.Load(EntityFilePath(EntityName));
            List<XElement> list = new List<XElement>();
            foreach (XElement ele in root.Elements())
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if (entity == null)
                {
                    continue;
                }
                if (express.Compile()(entity))
                {
                    list.Add(ele);
                }
            }
            foreach (var ele in list)
            {
                ele.Remove();
            }
            root.Save(EntityFilePath(EntityName));
            return list.Count;
        }

        public static Entity Get<Entity>(string Id) where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XElement root = XElement.Load(EntityFilePath(EntityName));
            foreach (XElement ele in root.Elements())
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if (entity == null)
                {
                    continue;
                }
                if (entity.Id == Id)
                {
                    return entity;
                }
            }

            return null;
        }

        public static IEnumerable<Entity> Query<Entity>(Expression<Func<Entity, bool>> express = null)
            where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XElement root = XElement.Load(EntityFilePath(EntityName));
            if (root == null || root.IsEmpty)
            {
                return null;
            }

            List<Entity> list = new List<Entity>();
            foreach (XElement ele in root.Elements())
            {
                AppendEntity(ele, express, list);
            }

            return list.Count == 0 ? null : list;
        }

        private static void AppendEntity<Entity>(XElement ele, Expression<Func<Entity, bool>> express, List<Entity> list)
            where Entity : EntityBase, new()
        {
            Entity entity = EntityBase.FromElement<Entity>(ele);
            if (entity == null)
            {
                return;
            }
            if (express == null || express.Compile()(entity))
            {
                list.Add(entity);
            }
        }

        private static string EntityFilePath(string EntityName)
        {
            return DateFolderPath + EntityName + ".xml";
        }
    }
}