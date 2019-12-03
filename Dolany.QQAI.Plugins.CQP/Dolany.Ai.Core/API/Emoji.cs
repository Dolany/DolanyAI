using System.Linq;
using Dolany.Ai.Common;

namespace Dolany.Ai.Core.API
{
    public class Emoji
    {
        private static string[] allEmojis;

        public static string[] AllEmojis()
        {
            if (!allEmojis.IsNullOrEmpty())
            {
                return allEmojis;
            }

            var type = typeof(Emoji);
            var props = type.GetProperties().Where(p => p.CanRead && !p.CanWrite && p.GetMethod.ReturnType == typeof(string));
            allEmojis = props.Select(p => p.GetValue(null) as string).ToArray();
            return allEmojis;
        }

        public static string 星星 => CodeApi.Code_Emoji(14855568);

        public static string 月亮 => CodeApi.Code_Emoji(4036988057);

        public static string 太阳 => CodeApi.Code_Emoji(4036988062);

        public static string 王冠 => CodeApi.Code_Emoji(4036989329);

        public static string 心 => CodeApi.Code_Emoji(4036989591);

        public static string 剑 => CodeApi.Code_Emoji(4036990881);

        public static string 钱袋 => CodeApi.Code_Emoji(4036989616);

        public static string 雪花 => CodeApi.Code_Emoji(14851460);

        public static string 枫叶 => CodeApi.Code_Emoji(4036988289);

        public static string 火箭 => CodeApi.Code_Emoji(4036991616);

        public static string 葡萄 => CodeApi.Code_Emoji(4036988295);

        public static string 钥匙 => CodeApi.Code_Emoji(4036990097);

        public static string 草莓 => CodeApi.Code_Emoji(4036988307);

        public static string 药丸 => CodeApi.Code_Emoji(4036989578);

        public static string 四叶草 => CodeApi.Code_Emoji(4036988288);

        public static string 糖果 => CodeApi.Code_Emoji(4036988332);

        public static string 闪电 => CodeApi.Code_Emoji(14850721);

        public static string 钻石 => CodeApi.Code_Emoji(4036989582);

        public static string 流星 => CodeApi.Code_Emoji(4036988064);

        public static string 足球 => CodeApi.Code_Emoji(14850749);

        public static string 香蕉 => CodeApi.Code_Emoji(4036988300);

        public static string 礼物 => CodeApi.Code_Emoji(4036988545);

        public static string 蘑菇 => CodeApi.Code_Emoji(4036988292);

        public static string 面包 => CodeApi.Code_Emoji(4036988318);
    }
}
