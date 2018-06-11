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
using AILib.Entities;

namespace AILib
{
    public static class DbMgr
    {
        private static string DateFolderPath = "./DbData/";

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

        public static void Insert(EntityBase entity)
        {
            string EntityName = entity.GetType().Name.Replace("Entity", "");
            XmlDocument doc = GetDocument(EntityName);
            XmlElement ele = entity.ToElement(doc);
            XmlNode root = doc.FirstChild;
            root.AppendChild(ele);
            doc.Save(EntityFilePath(EntityName));
        }

        public static bool Update(EntityBase entity)
        {
            string EntityName = entity.GetType().Name.Replace("Entity", "");
            XmlDocument doc = GetDocument(EntityName);
            XmlNode root = doc.FirstChild;
            foreach (XmlElement ele in root.ChildNodes)
            {
                if (entity.Id == ele.GetAttribute("Id"))
                {
                    root.ReplaceChild(entity.ToElement(doc), ele);
                    doc.Save(EntityFilePath(EntityName));
                    return true;
                }
            }

            return false;
        }

        public static bool Delete<Entity>(string Id) where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XmlDocument doc = GetDocument(EntityName);
            XmlNode root = doc.FirstChild;
            foreach (XmlElement ele in root.ChildNodes)
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if (entity.Id == Id)
                {
                    root.RemoveChild(ele);
                    doc.Save(EntityFilePath(EntityName));
                    return true;
                }
            }

            return false;
        }

        public static int Delete<Entity>(Expression<Func<Entity, bool>> express) where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XmlDocument doc = GetDocument(EntityName);
            XmlNode root = doc.FirstChild;
            List<XmlElement> list = new List<XmlElement>();
            foreach (XmlElement ele in root.ChildNodes)
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if (express.Compile()(entity))
                {
                    list.Add(ele);
                }
            }
            foreach(var ele in list)
            {
                root.RemoveChild(ele);
            }
            doc.Save(EntityFilePath(EntityName));
            return list.Count;
        }

        public static Entity Get<Entity>(string Id) where Entity : EntityBase, new()
        {
            Type t = typeof(Entity);
            string EntityName = t.Name.Replace("Entity", "");
            XmlDocument doc = GetDocument(EntityName);
            XmlNode root = doc.FirstChild;
            foreach(XmlElement ele in root.ChildNodes)
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if(entity.Id == Id)
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
            XmlDocument doc = GetDocument(EntityName);
            XmlNode root = doc.FirstChild;
            List<Entity> list = new List<Entity>();
            foreach (XmlElement ele in root.ChildNodes)
            {
                Entity entity = EntityBase.FromElement<Entity>(ele);
                if (express == null || express.Compile()(entity))
                {
                    list.Add(entity);
                }
            }

            return list.Count == 0 ? null : list;
        }

        public static XmlDocument GetDocument(string EntityName)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(EntityFilePath(EntityName));
            return doc;
        }

        private static string EntityFilePath(string EntityName)
        {
            return DateFolderPath + EntityName + ".xml";
        }
    }
}
