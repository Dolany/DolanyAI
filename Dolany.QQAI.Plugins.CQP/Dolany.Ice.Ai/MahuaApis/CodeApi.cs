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
        public static string Code_At(long qqNumber)
        {
            string code = string.Empty;
            switch (MahuaGlobal.CurrentPlatform)
            {
                case MahuaPlatform.Cqp:
                    code = CQCode.CQCode_At(qqNumber);
                    break;

                case MahuaPlatform.Amanda:
                    code = AmandaCode.AmandaCode_At(qqNumber);
                    break;
            }

            return code;
        }

        public static string Code_Image(string fileName)
        {
            string code = string.Empty;
            switch (MahuaGlobal.CurrentPlatform)
            {
                case MahuaPlatform.Cqp:
                    code = CQCode.CQCode_Image(fileName);
                    break;

                case MahuaPlatform.Amanda:
                    code = AmandaCode.AmandaCode_Pic(fileName);
                    break;
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
                }

                return string.Empty;
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
                }

                return string.Empty;
            }
        }
    }
}