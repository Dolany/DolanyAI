using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.MahuaApis
{
    public class AmandaCode
    {
        /// <summary>
        /// 发送图片
        /// </summary>
        /// <param name="filePath">本地图片路径/网络图片链接/GUID</param>
        /// <returns></returns>
        public static string AmandaCode_Pic(string filePath)
        {
            return $"[QQ:pic={filePath}]";
        }

        /// <summary>
        /// 发送闪照
        /// </summary>
        /// <param name="filePath">本地图片路径/网络图片链接/GUID</param>
        /// <returns></returns>
        public static string AmandaCode_Flash(string filePath)
        {
            return $"[QQ:flash,pic={filePath}]";
        }

        /// <summary>
        /// 艾特
        /// </summary>
        /// <param name="QQNum">qq号码</param>
        /// <returns></returns>
        public static string AmandaCode_At(long QQNum)
        {
            return $"[QQ:at={QQNum}]";
        }

        /// <summary>
        /// 艾特全体
        /// </summary>
        /// <returns></returns>
        public static string AmandaCode_AtAll()
        {
            return "[QQ:at=all]";
        }
    }
}