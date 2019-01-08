using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using static Dolany.Ai.Core.Common.Utility;

namespace Dolany.Ai.Core.Ai.SingleCommand.Tuling
{
    using Dolany.Ai.Core.Model;
    using Dolany.Ai.Core.Net;

    public class XfyunRequestHelper
    {
        private static string appid = "5b62c9d3";
        private static string apiKey = "1250bc28769bb74ab0aa073a97434058";
        private static string requestUrl = "http://api.xfyun.cn/v1/service/v1/tts";

        private static string voicePath => Environment.CurrentDirectory + @"\VoiceCache\";

        private static string voiceName = "xiaoyan";

        public static string PostData(string text)
        {
            using (var wc = new WebClient())
            {
                var CurTime = GetUtcLong().ToString();
                var Params = GetParams();
                var CheckSum = GetCheckSum(apiKey + CurTime + Params);

                var postData = $"text={UrlCharConvert(text)}";
                var bytes = Encoding.UTF8.GetBytes(postData);

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
                wc.Headers.Add("X-Appid", appid);
                wc.Headers.Add("X-CurTime", CurTime);
                wc.Headers.Add("X-Param", Params);
                wc.Headers.Add("X-CheckSum", CheckSum);

                var responseData = wc.UploadData($"{requestUrl}", "POST", bytes);
                return WriteFile(responseData);
            }
        }

        private static string WriteFile(byte[] response)
        {
            var filePath = voicePath + DateTime.Now.ToString("yyyyMMddHHmmss") + ".mp3";
            using (var steam = new FileStream(filePath, FileMode.Create))
            {
                steam.Write(response, 0, response.Length);
                steam.Flush();
            }

            return filePath;
        }

        private static long GetUtcLong()
        {
            var UtcTime = new DateTime(1970, 1, 1, 0, 0, 0);
            var span = DateTime.Now.ToUniversalTime() - UtcTime;
            return (long)span.TotalSeconds;
        }

        private static string GetParams()
        {
            var request = new XfyunRequestData
            {
                Aue = "lame",
                Auf = "audio/L16;rate=16000",
                Voice_name = voiceName
            };
            var postData = JsonHelper.SerializeObject(request);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(postData));
        }

        private static string GetCheckSum(string input)
        {
            using (MD5 md5 = new MD5CryptoServiceProvider())
            {
                var output = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                return BitConverter.ToString(output).Replace("-", string.Empty).ToLower();
            }
        }
    }
}
