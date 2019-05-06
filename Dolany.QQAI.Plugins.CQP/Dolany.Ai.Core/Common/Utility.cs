namespace Dolany.Ai.Core.Common
{
    using System;
    using System.IO;
    using System.Linq;
    using Database.Sqlite.Model;

    using Dolany.Ai.Common;
    using Database.Sqlite;
    using JetBrains.Annotations;

    using Model;

    public static class Utility
    {
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

        public static string GetAuthName(MsgInformationEx MsgDTO)
        {
            var tempAuth = GetTempAuth(MsgDTO);
            if (MsgDTO.FromQQ == Global.DeveloperNumber || tempAuth == "开发者")
            {
                return "开发者";
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return "成员";
            }

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (setting.AuthInfo == null)
            {
                return "成员";
            }

            if (setting.AuthInfo.Owner == MsgDTO.FromQQ || tempAuth == "群主")
            {
                return "群主";
            }

            if (setting.AuthInfo.Mgrs.Contains(MsgDTO.FromQQ) || tempAuth == "管理员")
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
