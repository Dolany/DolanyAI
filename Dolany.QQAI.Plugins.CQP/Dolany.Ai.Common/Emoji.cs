namespace Dolany.Ai.Common
{
    public class Emoji
    {
        private static string EmojiCode(int codeNum)
        {
            return $"[QQ:emoji={codeNum}]";
        }

        public static string 剑 => EmojiCode(38817);
        public static string 心 => EmojiCode(37524);
        public static string 钱 => EmojiCode(37552);
        public static string 怪兽 => EmojiCode(39048);
        public static string 礼物 => EmojiCode(36481);
        public static string 炸弹 => EmojiCode(37539);
    }
}
