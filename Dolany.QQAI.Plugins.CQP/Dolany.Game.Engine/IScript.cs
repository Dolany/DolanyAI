using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dolany.Game.Engine
{
    public class IScript
    {
        private readonly Dictionary<string, MethodInfo> SavePointDic = new Dictionary<string, MethodInfo>();

        public IScript()
        {
            var type = GetType();
            var methods = type.GetMethods();
            foreach (var methodInfo in methods)
            {
                if (methodInfo.GetCustomAttribute(typeof(SavePointAttribute)) is SavePointAttribute attr)
                {
                    SavePointDic.Add(attr.Name, methodInfo);
                }
            }
        }

        public void LoadFromSavePoint(string name)
        {
            if (SavePointDic.Keys.Contains(name))
            {
                SavePointDic[name].Invoke(this, null);
            }
        }

        [SavePoint(Name = "Step1")]
        public void ScriptStep1()
        {

        }

        protected void SendMsg(string msg)
        {

        }
    }
}
