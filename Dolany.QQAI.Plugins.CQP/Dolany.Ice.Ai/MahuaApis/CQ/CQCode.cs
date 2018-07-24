using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.MahuaApis.CQ
{
    public class CQCode
    {
        /// <summary>
        /// 获取 @指定QQ 的操作代码。
        /// </summary>
        /// <param name="qqNumber">指定的QQ号码。
        /// <para>当该参数为-1时，操作为 @全部成员。</para></param>
        /// <returns>CQ @操作代码。</returns>
        public static string CQCode_At(long qqNumber)
        {
            return "[CQ:at,qq=" + (qqNumber == -1 ? "all" : qqNumber.ToString()) + "]";
        }

        /// <summary>
        /// 获取 指定的emoji表情代码。
        /// </summary>
        /// <param name="id">emoji表情索引ID。</param>
        /// <returns>CQ emoji表情代码。</returns>
        public static string CQCode_Emoji(int id)
        {
            return "[CQ:emoji,id=" + id + "]";
        }

        /// <summary>
        /// 获取 指定的表情代码。
        /// </summary>
        /// <param name="id">表情索引ID。</param>
        /// <returns>CQ 表情代码。</returns>
        public static string CQCode_Face(int id)
        {
            return "[CQ:face,id=" + id + "]";
        }

        /// <summary>
        /// 获取 窗口抖动代码。
        /// </summary>
        /// <returns>CQ 窗口抖动代码。</returns>
        public static string CQCode_Shake()
        {
            return "[CQ:shake]";
        }

        /// <summary>
        /// 获取 匿名代码。
        /// </summary>
        /// <param name="ignore">是否不强制。</param>
        /// <returns>CQ 匿名代码。</returns>
        public static string CQCode_Anonymous(bool ignore = false)
        {
            return "[CQ:anonymous" + (ignore ? ",ignore=true" : "") + "]";
        }

        /// <summary>
        /// 获取 发送图片代码。
        /// </summary>
        /// <param name="fileName">图片路径。</param>
        /// <returns>CQ 发送图片代码。</returns>
        public static string CQCode_Image(string fileName)
        {
            return "[CQ:image,file=" + fileName + "]";
        }

        /// <summary>
        /// 获取 发送音乐代码。
        /// </summary>
        /// <param name="id">音乐索引ID。</param>
        /// <returns>CQ 发送音乐代码。</returns>
        public static string CQCode_Music(int id)
        {
            return "[CQ:music,id=" + id + "]";
        }

        /// <summary>
        /// 获取 发送语音代码。
        /// </summary>
        /// <param name="fileName">语音文件路径。</param>
        /// <returns>CQ 发送语音代码。</returns>
        public static string CQCode_Record(string fileName)
        {
            return "[CQ:record,file=" + fileName + "]";
        }

        /// <summary>
        /// 获取 链接分享代码。
        /// </summary>
        /// <param name="url">链接地址。</param>
        /// <param name="title">标题。</param>
        /// <param name="content">内容。</param>
        /// <param name="imageUrl">图片地址。</param>
        /// <returns>CQ 链接分享代码。</returns>
        public static string CQCode_ShareLink(string url, string title, string content, string imageUrl)
        {
            return String.Format("[CQ:share,url={0},title={1},content={2},image={3}]", url, title, content, imageUrl);
        }
    }
}