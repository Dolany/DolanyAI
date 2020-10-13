using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using Dolany.Ai.Common;
using Dolany.Ai.Common.Models;
using Dolany.UtilityTool;
using JetBrains.Annotations;

namespace Dolany.Ai.Core.Common
{
    /// <summary>
    /// 通用工具
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// 时间字符串解析
        /// </summary>
        /// <param name="timeStr"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取权限等级
        /// </summary>
        /// <param name="MsgDTO"></param>
        /// <returns></returns>
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

            var setting = AutofacSvc.Resolve<GroupSettingSvc>()[MsgDTO.FromGroup];
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

        /// <summary>
        /// 获取文件中的图片信息（带重试）
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static ImageCacheModel ReadImageCacheInfo(FileSystemInfo file)
        {
            var model = LambdaExtention.DoWithRetry(() => ReadImageCacheInfo_Func(file),
                new[] {TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(3), TimeSpan.FromSeconds(5)});

            return model;
        }

        /// <summary>
        /// 获取文件中的图片信息
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private static ImageCacheModel ReadImageCacheInfo_Func(FileSystemInfo file)
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
        }

        /// <summary>
        /// 读取图片缓存信息
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        [CanBeNull]
        public static ImageCacheModel ReadImageCacheInfo(string guid, string imagePath)
        {
            var file = new FileInfo(imagePath + guid + CodeApi.ImageExtension);
            return !file.Exists ? null : ReadImageCacheInfo(file);
        }

        /// <summary>
        /// 给任意对象的某个属性赋值
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propName"></param>
        /// <param name="propValue"></param>
        private static void SetPropertyValue(object obj, string propName, string propValue)
        {
            var type = obj.GetType();

            var prop = type.GetProperties().FirstOrDefault(p => p.Name == propName && p.CanWrite);
            if (prop != null)
            {
                prop.SetValue(obj, Convert.ChangeType(propValue, prop.PropertyType));
            }
        }

        /// <summary>
        /// 解析消息中的图片ID
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取指定等级的星月等级字符串
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取指定等级的星月等级表示
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取指定等级的星星表示字符串
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 下载并保存文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="savePath"></param>
        /// <returns></returns>
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
