using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Dolany.IWS2000.Ai.DolanyAI
{
    public static class DbMgr
    {
        private const string DateFolderPath = "./DbData/";

        public static void InitXmls()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var typeArr = assembly.GetTypes();

            foreach (var t in typeArr)
            {
                if (t.BaseType != typeof(EntityBase))
                {
                    continue;
                }

                var EntityName = t.Name.Replace("Entity", "");
                InitXml(EntityName);
            }
        }

        private static void InitXml(string EntityName)
        {
            var FilePath = EntityFilePath(EntityName);
            var fileInfo = new FileInfo(FilePath);
            if (fileInfo.Exists)
            {
                return;
            }

            var doc = new XmlDocument();
            var root = doc.CreateElement(EntityName);
            doc.AppendChild(root);

            doc.Save(FilePath);
        }

        public static void Insert(EntityBase entity)
        {
            var EntityName = entity.GetType().Name.Replace("Entity", "");
            InitXml(EntityName);
            var root = XElement.Load(EntityFilePath(EntityName));
            var ele = entity.ToElement();
            root.Add(ele);
            root.Save(EntityFilePath(EntityName));
        }

        public static bool Update(EntityBase entity)
        {
            var EntityName = entity.GetType().Name.Replace("Entity", "");
            var root = XElement.Load(EntityFilePath(EntityName));
            foreach (var ele in root.Elements())
            {
                if (entity.Id != ele.Attribute("Id")?.Value)
                {
                    continue;
                }

                ele.ReplaceWith(entity.ToElement());
                root.Save(EntityFilePath(EntityName));
                return true;
            }

            return false;
        }

        public static bool Delete<Entity>(string Id) where Entity : EntityBase, new()
        {
            var t = typeof(Entity);
            var EntityName = t.Name.Replace("Entity", "");
            var root = XElement.Load(EntityFilePath(EntityName));
            foreach (var ele in root.Elements())
            {
                var entity = EntityBase.FromElement<Entity>(ele);
                if (entity == null || entity.Id != Id)
                {
                    continue;
                }

                ele.Remove();
                root.Save(EntityFilePath(EntityName));
                return true;
            }

            return false;
        }

        public static int Delete<Entity>(Expression<Func<Entity, bool>> express) where Entity : EntityBase, new()
        {
            var t = typeof(Entity);
            var EntityName = t.Name.Replace("Entity", "");
            var root = XElement.Load(EntityFilePath(EntityName));
            var list = (from ele in root.Elements()
                        let entity = EntityBase.FromElement<Entity>(ele)
                        where entity != null
                        where express.Compile()(entity)
                        select ele).ToList();
            foreach (var ele in list)
            {
                ele.Remove();
            }
            root.Save(EntityFilePath(EntityName));
            return list.Count;
        }

        public static Entity Get<Entity>(string Id) where Entity : EntityBase, new()
        {
            var t = typeof(Entity);
            var EntityName = t.Name.Replace("Entity", "");
            var root = XElement.Load(EntityFilePath(EntityName));
            return root.Elements().Select(EntityBase.FromElement<Entity>).Where(entity => entity != null)
                .FirstOrDefault(entity => entity.Id == Id);
        }

        public static IEnumerable<Entity> Query<Entity>(Expression<Func<Entity, bool>> express = null)
            where Entity : EntityBase, new()
        {
            var t = typeof(Entity);
            var EntityName = t.Name.Replace("Entity", "");
            var root = XElement.Load(EntityFilePath(EntityName));
            if (root.IsEmpty)
            {
                return null;
            }

            var list = new List<Entity>();
            foreach (var ele in root.Elements())
            {
                AppendEntity(ele, express, list);
            }

            return list.Count == 0 ? null : list;
        }

        private static void AppendEntity<Entity>(XElement ele, Expression<Func<Entity, bool>> express, List<Entity> list)
            where Entity : EntityBase, new()
        {
            var entity = EntityBase.FromElement<Entity>(ele);
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