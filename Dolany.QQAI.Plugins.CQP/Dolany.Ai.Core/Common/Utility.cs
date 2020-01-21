using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using JetBrains.Annotations;

namespace Dolany.Ai.Core.Common
{
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

            return !int.TryParse(strs[1], out var minute) ? null : new HourMinuteModel { Hour = hour, Minute = minute };
        }

        public static AuthorityLevel GetAuth(MsgInformationEx MsgDTO)
        {
            if (MsgDTO.FromQQ == Global.DeveloperNumber)
            {
                return AuthorityLevel.开发者;
            }

            if (MsgDTO.Type == MsgType.Private)
            {
                return AuthorityLevel.成员;
            }

            var setting = GroupSettingMgr.Instance[MsgDTO.FromGroup];
            if (setting.AuthInfo == null)
            {
                return AuthorityLevel.成员;
            }

            if (setting.AuthInfo.Owner == MsgDTO.FromQQ)
            {
                return AuthorityLevel.群主;
            }

            return setting.AuthInfo.Mgrs.Contains(MsgDTO.FromQQ) ? AuthorityLevel.管理员 : AuthorityLevel.成员;
        }

        private static ImageCacheModel ReadImageCacheInfo(FileSystemInfo file)
        {
            var model = CommonUtil.Retry(() =>
            {
                var cache = new ImageCacheModel();
                using var reader = new StreamReader(file.FullName);
                string line;
                while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                {
                    var strs = line.Split(new[] {'='}, StringSplitOptions.RemoveEmptyEntries);
                    if (strs.IsNullOrEmpty() || strs.Length < 2)
                    {
                        continue;
                    }

                    SetPropertyValue(cache, strs[0], strs[1]);
                }

                return cache;
            }, new[] {TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)});

            return model;
        }

        [CanBeNull]
        public static ImageCacheModel ReadImageCacheInfo(string guid, string imagePath)
        {
            var file = new FileInfo(imagePath + guid + CodeApi.ImageExtension);
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
            var imageGuid = strs3.FirstOrDefault();

            return imageGuid;
        }

        public static string LevelEmoji(int level)
        {
            var stack = EmojiConvent(level);
            var msg = "";
            while (stack.Count > 0)
            {
                msg += stack.Pop();
            }

            return msg;
        }

        private static Stack<string> EmojiConvent(int level)
        {
            var stack = new Stack<string>();

            if (level <= 0)
            {
                return stack;
            }

            var count = level % 4;
            for (var i = 0; i < count; i++)
            {
                stack.Push(Emoji.星星);
            }

            level /= 4;

            if (level <= 0)
            {
                return stack;
            }

            count = level % 4;
            for (var i = 0; i < count; i++)
            {
                stack.Push(Emoji.月亮);
            }

            level /= 4;

            if (level <= 0)
            {
                return stack;
            }

            count = level % 4;
            for (var i = 0; i < count; i++)
            {
                stack.Push(Emoji.太阳);
            }

            level /= 4;

            if (level <= 0)
            {
                return stack;
            }

            count = level % 4;
            for (var i = 0; i < count; i++)
            {
                stack.Push(Emoji.王冠);
            }

            return stack;
        }

        public static string LevelToStars(int level)
        {
            var str = string.Empty;
            var stars = level / 2;
            for (var i = 0; i < stars; i++)
            {
                str += "★";
            }

            if (level % 2 == 1)
            {
                str += "☆";
            }

            return str;
        }

        public static bool DownloadImage(string url, string savePath)
        {
            try
            {
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.ServicePoint.Expect100Continue = false;
                req.Method = "GET";
                req.KeepAlive = true;
                req.ContentType = "image/*";
                using var rsp = (HttpWebResponse) req.GetResponse();
                using var stream = rsp.GetResponseStream();
                Image.FromStream(stream).Save(savePath);

                return true;
            }
            catch (Exception e)
            {
                RuntimeLogger.Log(e);
                return false;
            }
        }
    }
}
