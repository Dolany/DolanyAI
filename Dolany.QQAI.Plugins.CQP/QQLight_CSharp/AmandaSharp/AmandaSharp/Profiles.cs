using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

namespace AmandaSharp
{
    public class Profiles
    {
        private static bool inited = false;
        public static Dictionary<string, string> dic = new Dictionary<string, string>();

        public Profiles()
        {
            if (inited)
                return;
            string path = API.GetPath() + "\\config.ini";
            if (!File.Exists(path))
                File.CreateText(path).Close();
            FileStream fs = File.OpenRead(path);
            StreamReader sr = new StreamReader(fs);
            string line = null;
            while (null != (line = sr.ReadLine()))
            {
                string[] tmp = line.Split(new char[] { '=' }, 2);
                if (tmp.Length == 2)
                    dic.Add(tmp[0].Trim(), tmp[1].Trim());
            }
            sr.Close();
            fs.Close();
        }

        public string get(string key)
        {
            return dic[key];
        }

        public void set(string key, string value)
        {
            dic.Add(key, value);
        }

        public void save()
        {
            string path = API.GetPath() + "\\config.ini";

            if (!File.Exists(path))
                File.CreateText(path).Close();
            FileStream fs = File.OpenWrite(path);
            StreamWriter sw = new StreamWriter(fs);
            foreach (string key in dic.Keys)
            {
                sw.WriteLine(key + "=" + dic[key]);
            }
            sw.Close();
            fs.Close();
        }
    }
}