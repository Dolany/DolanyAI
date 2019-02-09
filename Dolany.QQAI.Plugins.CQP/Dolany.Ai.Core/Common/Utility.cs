namespace Dolany.Ai.Core.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using API;

    using Cache;

    using Database.Sqlite.Model;

    using Dolany.Ai.Common;
    using Dolany.Database.Ai;
    using Database.Sqlite;

    using Entities;

    using JetBrains.Annotations;

    using Model;

    public static class Utility
    {
        private static long DeveloperNumber { get; } = long.Parse(Configger.Instance["DeveloperNumber"]);

        public static long SysMsgNumber { get; } = long.Parse(Configger.Instance["SysMsgNumber"]);

        public static long AnonymousNumber { get; } = long.Parse(Configger.Instance["AnonymousNumber"]);

        public static long SelfQQNum { get; } = long.Parse(Configger.Instance["SelfQQNum"]);

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
            var response =
                SCacheService.Get<TempAuthorizeCache>($"TempAuthorize-{MsgDTO.FromGroup}-{MsgDTO.FromQQ}");

            return response != null ? response.AuthName : string.Empty;
        }

        [CanBeNull]
        public static MemberRoleCache GetMemberInfo(MsgInformationEx MsgDTO)
        {
            return GroupMemberInfoCacher.GetMemberInfo(MsgDTO);
        }

        [NotNull]
        private static ImageCacheModel ReadImageCacheInfo(FileSystemInfo file)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var model = new ImageCacheModel();

                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var strs = line.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() || strs.Length < 2)
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
    }
}
