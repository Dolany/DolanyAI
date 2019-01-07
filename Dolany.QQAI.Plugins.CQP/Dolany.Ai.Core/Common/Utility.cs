namespace Dolany.Ai.Core.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Runtime.ExceptionServices;
    using System.Security.Cryptography;
    using System.Text;

    using Dolany.Ai.Core.API;
    using Dolany.Ai.Core.Cache;
    using Dolany.Ai.Core.Db;
    using Dolany.Ai.Core.Entities;
    using Dolany.Ai.Core.Model;

    using JetBrains.Annotations;

    public static class Utility
    {
        public static long DeveloperNumber => long.Parse(GetConfig(nameof(DeveloperNumber)));

        public static long SysMsgNumber => long.Parse(GetConfig(nameof(SysMsgNumber)));

        public static long SelfQQNum => long.Parse(GetConfig(nameof(SelfQQNum)));

        private static Dictionary<string, string> AIConfig;

        private static readonly RNGCryptoServiceProvider RngCsp = new RNGCryptoServiceProvider();

        public static bool IsNullOrEmpty<T>(this IEnumerable<T> objs)
        {
            return objs == null || !objs.Any();
        }

        public static void Swap<T>(this T[] array, int firstIdx, int secondIdx)
        {
            var temp = array[firstIdx];
            array[firstIdx] = array[secondIdx];
            array[secondIdx] = temp;
        }

        [NotNull]
        [HandleProcessCorruptedStateExceptions]
        public static string GetConfig(string name)
        {
            if (AIConfig == null)
            {
                AIConfig = GetConfigDic();
            }

            return AIConfig.Keys.Contains(name) ? AIConfig[name] : string.Empty;
        }

        public static string GetConfig(string name, string defaltValue)
        {
            var value = GetConfig(name);
            return !string.IsNullOrEmpty(value) ? value : defaltValue;
        }

        [NotNull]
        private static Dictionary<string, string> GetConfigDic()
        {
            var configFile = new FileInfo("AIConfig.ini");
            var dic = new Dictionary<string, string>();
            using (var reader = new StreamReader(configFile.FullName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var strs = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() ||
                        strs.Length != 2)
                    {
                        continue;
                    }

                    dic.Add(strs[0], strs[1]);
                }

                return dic;
            }
        }

        [NotNull]
        public static Dictionary<int, string> LoadFortuneImagesConfig()
        {
            var dic = new Dictionary<int, string>();
            var configFile = new FileInfo("FortuneImagesConfig.ini");
            using (var reader = new StreamReader(configFile.FullName))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var strs = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() ||
                        strs.Length != 2)
                    {
                        continue;
                    }

                    dic.Add(int.Parse(strs[0]), strs[1]);
                }

                return dic;
            }
        }

        public static string UrlCharConvert(string name)
        {
            var result = string.Empty;
            var builder = new StringBuilder();
            builder.Append(result);
            foreach (var c in name)
            {
                if (IsAsciiChar(c))
                {
                    builder.Append(c);
                    continue;
                }

                builder.Append(@"%" + BitConverter.ToString(Encoding.UTF8.GetBytes(new[] { c })).Replace("-", "%"));
            }
            result = builder.ToString();

            return result;
        }

        private static bool IsAsciiChar(char c)
        {
            return c >= 0x20 && c <= 0x7e;
        }

        public static void SendMsgToDeveloper(string msg)
        {
            MsgSender.Instance.PushMsg(
                new MsgCommand
                    {
                        Id = Guid.NewGuid().ToString(),
                        Command = AiCommand.SendPrivate,
                        Msg = msg,
                        ToQQ = DeveloperNumber
                    });
        }

        public static void SendMsgToDeveloper(Exception ex)
        {
            SendMsgToDeveloper(ex.Message + '\r' + ex.StackTrace);
        }

        [CanBeNull]
        public static HourMinuteModel GenTimeFromStr(string timeStr)
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

            return new HourMinuteModel { Hour = hour, Minute = minute };
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

        public static string GetAuthName(MsgInformationEx MsgDTO)
        {
            var tempAuth = GetTempAuth(MsgDTO);
            if (MsgDTO.FromQQ == DeveloperNumber || tempAuth == "开发者")
            {
                return "开发者";
            }

            var mi = GetMemberInfo(MsgDTO);
            if (mi == null)
            {
                MsgSender.Instance.PushMsg(
                    MsgDTO, "获取权限信息失败！请联系开发者！");
                return "成员";
            }

            var authority = mi.Role;
            if (authority == 0 || tempAuth == "群主")
            {
                return "群主";
            }

            if (authority == 1 || tempAuth == "管理员")
            {
                return "管理员";
            }

            return "成员";
        }

        private static string GetTempAuth(MsgInformation MsgDTO)
        {
            using (var db = new AIDatabase())
            {
                var date = DateTime.Now.Date;
                var authInfo = db.TempAuthorize.FirstOrDefault(
                    t => t.GroupNum == MsgDTO.FromGroup && t.QQNum == MsgDTO.FromQQ && t.AuthDate == date);
                if (authInfo != null)
                {
                    return authInfo.AuthName;
                }
            }

            return string.Empty;
        }

        [CanBeNull]
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            return GroupMemberInfoCacher.GetMemberInfo(MsgDTO);
        }

        [NotNull]
        private static ImageCacheModel ReadImageCacheInfo(FileInfo file)
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

        [CanBeNull]
        public static ImageCacheModel ReadImageCacheInfo(string guid)
        {
            var file = new FileInfo(CodeApi.ImagePath + guid + CodeApi.ImageExtension);
            return !file.Exists ? null : ReadImageCacheInfo(file);
        }

        private static void SetPropertyValue(object obj, string propName, string propValue)
        {
            var type = obj.GetType();

            var prop = type.GetProperties().FirstOrDefault(p => p.Name == propName && p.CanWrite);
            if (prop != null)
            {
                prop.SetValue(obj, Convert.ChangeType(propValue, prop.PropertyType));
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
                return string.Empty;
            }

            var startIdx = picIdx + "[QQ:pic=".Length;
            var endIdx = msg.IndexOf("]", startIdx, StringComparison.Ordinal);
            var picName = msg.Substring(startIdx, endIdx - startIdx);

            return picName;
        }

        public static void RemovePicCache(string picName)
        {
            var picShortName = picName.Split('.').First();
            var dir = new DirectoryInfo(CodeApi.ImagePath);
            foreach (var file in dir.GetFiles())
            {
                if (file.Name.Contains(picShortName))
                {
                    file.Delete();
                }
            }

            DbMgr.Delete<PicCacheEntity>(p => p.Content == picName);
        }

        public static string ParsePicGuid(string msg)
        {
            if (!msg.Contains("QQ:pic="))
            {
                return string.Empty;
            }

            var strs1 = msg.Split(new[] { "QQ:pic=" }, StringSplitOptions.RemoveEmptyEntries);
            var strs2 = strs1.Last().Split(']');
            var strs3 = strs2.First().Split('.');
            var imageGuid = strs3.First();

            return imageGuid;
        }

        public static T[] RandSort<T>(T[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var randIdx = RandInt(array.Length - i) + i;

                array.Swap(i, randIdx);
            }

            return array;
        }
    }
}
