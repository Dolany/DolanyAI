using Dolany.Ai.Reborn.DolanyAI.Common;

namespace Dolany.Ai.Reborn.MahuaApis
{
    public static class CodeApi
    {
        public static string Code_At(long qqNumber)
        {
            return AmandaCode.AmandaCode_At(qqNumber);
        }

        public static string Code_SelfAt()
        {
            return Code_At(Utility.SelfQQNum);
        }

        public static string Code_Image(string fileName)
        {
            return AmandaCode.AmandaCode_Pic(fileName);
        }

        public static string Code_Flash(string fileName)
        {
            return $"[QQ:flash,pic={fileName}]";
        }

        public static string Code_Voice(string filePath)
        {
            return $"[QQ:voice={filePath}]";
        }

        public static string ImagePath => "temp/image/";

        public static string ImageExtension => ".ini";
    }
}
