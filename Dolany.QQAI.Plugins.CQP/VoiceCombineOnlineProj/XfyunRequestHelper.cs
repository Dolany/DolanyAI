﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Dolany.Ice.Ai.MahuaApis;
using Dolany.Ice.Ai.DolanyAI;
using System.Security.Cryptography;
using System.IO;

namespace VoiceCombineOnlineProj
{
    public class XfyunRequestHelper
    {
        public static string PostData(PostReq_Param p, RequestBody body)
        {
            using (WebClient wc = new WebClient())
            {
                string appid = "5b62c9d3";
                string CurTime = GetUtcLong().ToString();
                string Params = GetParams(p.data as XfyunRequest);
                string CheckSum = GetCheckSum("1250bc28769bb74ab0aa073a97434058" + CurTime + Params);

                string postData = $"text={Utility.UrlCharConvert(body.text)}";
                byte[] bytes = Encoding.UTF8.GetBytes(postData);

                wc.Headers.Add("Content-Type", "application/x-www-form-urlencoded; charset=utf-8");
                wc.Headers.Add("X-Appid", appid);
                wc.Headers.Add("X-CurTime", CurTime);
                wc.Headers.Add("X-Param", Params);
                wc.Headers.Add("X-CheckSum", CheckSum);
                //wc.Headers.Add("X-Real-Ip", "127.0.0.1");

                byte[] responseData = wc.UploadData(string.Format("{0}", p.InterfaceName), "POST", bytes);
                return WriteFile(responseData);
            }
        }

        private static string WriteFile(byte[] response)
        {
            string filePath = "./test.wav";
            using (FileStream steam = new FileStream(filePath, FileMode.Create))
            {
                steam.Write(response, 0, response.Length);
                steam.Flush();
            }

            return filePath;
        }

        private static long GetUtcLong()
        {
            DateTime UtcTime = new DateTime(1970, 1, 1, 0, 0, 0);
            var span = DateTime.Now.ToUniversalTime() - UtcTime;
            return (long)span.TotalSeconds;
        }

        private static string GetParams(XfyunRequest request)
        {
            string postData = JsonHelper.SerializeObject(request);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(postData));
        }

        private static string GetCheckSum(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(output).Replace("-", "").ToLower();
        }
    }
}