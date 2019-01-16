﻿using System.IO;

namespace Dolany.Ai.Core.API
{
    using Common;

    using Dolany.Ai.Common;

    public class CodeApi
    {
        public static string Code_At(long qqNumber)
        {
            return $"[QQ:at={qqNumber}]";
        }

        public static string Code_SelfAt()
        {
            return Code_At(Utility.SelfQQNum);
        }

        public static string Code_Image(string fileName)
        {
            return $"[QQ:pic={fileName}]";
        }

        public static string Code_Image_Relational(string relationalName)
        {
            return Code_Image(new FileInfo(relationalName).FullName);
        }

        public static string Code_Flash(string fileName)
        {
            return $"[QQ:flash,pic={fileName}]";
        }

        public static string Code_Voice(string filePath)
        {
            return $"[QQ:voice={filePath}]";
        }

        public static string ImagePath => CommonUtil.GetConfig("ImagePath");

        public static string ImageExtension => ".ini";
    }
}
