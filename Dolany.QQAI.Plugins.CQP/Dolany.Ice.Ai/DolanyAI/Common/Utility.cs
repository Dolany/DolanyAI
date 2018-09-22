using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Dolany.Ice.Ai.DolanyAI.Db;
using Dolany.Ice.Ai.DolanyAI.Utils;
using static Dolany.Ice.Ai.DolanyAI.Utils.Utility;
using static Dolany.Ice.Ai.DolanyAI.Utils.CodeApi;

namespace Dolany.Ice.Ai.DolanyAI
{
    public static class Utility
    {
        private static string AuthCode;

        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        public static void SendMsgToDeveloper(string msg)
        {
            MsgSender.Instance.PushMsg(new SendMsgDTO
            {
                Aim = DeveloperNumber,
                Type = MsgType.Private,
                Msg = msg
            });
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            SendMsgToDeveloper(ex.Message + '\r' + ex.StackTrace);
        }

        public static (int hour, int minute)? GenTimeFromStr(string timeStr)
        {
            var strs = timeStr.Split(':', '：');
            if (strs.Length != 2)
            {
                return null;
            }

            if (!int.TryParse(strs[0], out var hour))
            {
                return null;
            }

            if (!int.TryParse(strs[1], out var minute))
            {
                return null;
            }

            return (hour, minute);
        }

        public static T Clone<T>(this T obj) where T : class, new()
        {
            var type = obj.GetType();
            var copyT = new T();
            foreach (var prop in type.GetProperties())
            {
                if (prop.CanRead && prop.CanWrite)
                {
                    prop.SetValue(copyT, prop.GetValue(obj));
                }
            }

            return copyT;
        }

        public static MemberRoleCache GetMemberInfo(ReceivedMsgDTO MsgDTO)
        {
            return GroupMemberInfoCacher.GetMemberInfo(MsgDTO);
        }

        public static string GetAuthCode()
        {
            if (!AuthCode.IsNullOrEmpty())
            {
                return AuthCode;
            }

            const string logPath = "./logs/";
            var dir = new DirectoryInfo(logPath);

            foreach (var file in dir.GetFiles().OrderByDescending(p => p.CreationTime))
            {
                var authLine = "";
                using (var reader = new StreamReader(file.FullName))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("\"AuthCode\":"))
                        {
                            authLine = line;
                        }
                    }
                }

                if (authLine.IsNullOrEmpty())
                {
                    continue;
                }

                var strs = authLine.Split('\"');
                AuthCode = strs[3];
                break;
            }

            return AuthCode;
        }

        public static ImageCacheModel ReadImageCacheInfo(FileInfo file)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var model = new ImageCacheModel();

                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var strs = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() ||
                        strs.Length < 2)
                    {
                        continue;
                    }

                    SetPropertyValue(model, strs[0], strs[1]);
                }

                return model;
            }
        }

        public static ImageCacheModel ReadImageCacheInfo(string guid)
        {
            var file = new FileInfo(ImagePath + guid + ImageExtension);
            return !file.Exists ? null : ReadImageCacheInfo(file);
        }

        private static void SetPropertyValue(object obj, string propName, string propValue)
        {
            var type = obj.GetType();
            foreach (var prop in type.GetProperties())
            {
                if (prop.Name == propName)
                {
                    prop.SetValue(obj, Convert.ChangeType(propValue, prop.PropertyType));
                }
            }
        }

        public static string ToCommonString(this DateTime dt)
        {
            return dt.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static int RandInt(int MaxValue)
        {
            const decimal _base = int.MaxValue;
            var bytes = new byte[4];
            RngCsp.GetBytes(bytes);

            return (int)(Math.Abs(BitConverter.ToInt32(bytes, 0)) / _base * MaxValue);
        }

        public static string ParsePicName(string msg)
        {
            var picIdx = msg.IndexOf("[QQ:pic=", StringComparison.Ordinal);
            if (picIdx < 0)
            {
                return "";
            }

            var startIdx = picIdx + "[QQ:pic=".Length;
            var endIdx = msg.IndexOf("]", startIdx, StringComparison.Ordinal);
            var picName = msg.Substring(startIdx, endIdx - startIdx);

            return picName;
        }

        public static void RemovePicCache(string picName)
        {
            var picShortName = picName.Split('.').First();
            var dir = new DirectoryInfo(ImagePath);
            foreach (var file in dir.GetFiles())
            {
                if (file.Name.Contains(picShortName))
                {
                    file.Delete();
                }
            }
        }
    }
}