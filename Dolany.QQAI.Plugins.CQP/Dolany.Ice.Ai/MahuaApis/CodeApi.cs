﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.Mahua;

namespace Dolany.Ice.Ai.MahuaApis
{
    public class CodeApi
    {
        public static long SelfQQNum = 2105668527;

        public static string Code_At(long qqNumber)
        {
            var code = string.Empty;
            switch (MahuaGlobal.CurrentPlatform)
            {
                case MahuaPlatform.Cqp:
                    code = CQCode.CQCode_At(qqNumber);
                    break;

                case MahuaPlatform.Amanda:
                    code = AmandaCode.AmandaCode_At(qqNumber);
                    break;

                default:
                    throw new Exception("Unexpected Case");
            }

            return code;
        }

        public static string Code_SelfAt()
        {
            return Code_At(SelfQQNum);
        }

        public static string Code_Image(string fileName)
        {
            var code = string.Empty;
            switch (MahuaGlobal.CurrentPlatform)
            {
                case MahuaPlatform.Cqp:
                    code = CQCode.CQCode_Image(fileName);
                    break;

                case MahuaPlatform.Amanda:
                    code = AmandaCode.AmandaCode_Pic(fileName);
                    break;

                default:
                    throw new Exception("Unexpected Case");
            }

            return code;
        }

        public static string Code_Flash(string fileName)
        {
            var code = string.Empty;
            switch (MahuaGlobal.CurrentPlatform)
            {
                case MahuaPlatform.Cqp:
                    //code = $"[QQ:flash,pic={fileName}]";
                    break;

                case MahuaPlatform.Amanda:
                    code = $"[QQ:flash,pic={fileName}]";
                    break;

                default:
                    throw new Exception("Unexpected Case");
            }

            return code;
        }

        public static string Code_Voice(string filePath)
        {
            var code = string.Empty;
            switch (MahuaGlobal.CurrentPlatform)
            {
                case MahuaPlatform.Cqp:
                    //code = $"[QQ:flash,pic={fileName}]";
                    break;

                case MahuaPlatform.Amanda:
                    code = $"[QQ:voice={filePath}]";
                    break;

                default:
                    throw new Exception("Unexpected Case");
            }

            return code;
        }

        public static string ImagePath
        {
            get
            {
                switch (MahuaGlobal.CurrentPlatform)
                {
                    case MahuaPlatform.Cqp:
                        return "data/image/";

                    case MahuaPlatform.Amanda:
                        return "temp/image/";

                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }

        public static string ImageExtension
        {
            get
            {
                switch (MahuaGlobal.CurrentPlatform)
                {
                    case MahuaPlatform.Cqp:
                        return ".cqimg";

                    case MahuaPlatform.Amanda:
                        return ".ini";

                    default:
                        throw new Exception("Unexpected Case");
                }
            }
        }
    }
}