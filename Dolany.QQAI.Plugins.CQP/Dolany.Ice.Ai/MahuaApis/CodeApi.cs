using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dolany.Ice.Ai.MahuaApis
{
    public class CodeApi
    {
        public static string CurEnvirenment = "Amanda";

        public static string Code_At(long qqNumber)
        {
            string code = string.Empty;
            switch (CurEnvirenment)
            {
                case "CQ":
                    code = CQCode.CQCode_At(qqNumber);
                    break;

                case "Amanda":
                    code = AmandaCode.AmandaCode_At(qqNumber);
                    break;
            }

            return code;
        }

        public static string Code_Image(string fileName)
        {
            string code = string.Empty;
            switch (CurEnvirenment)
            {
                case "CQ":
                    code = CQCode.CQCode_Image(fileName);
                    break;

                case "Amanda":
                    code = AmandaCode.AmandaCode_Pic(fileName);
                    break;
            }

            return code;
        }

        public static string ImagePath
        {
            get
            {
                switch (CurEnvirenment)
                {
                    case "CQ":
                        return "data/image/";

                    case "Amanda":
                        return "temp/image/";
                }

                return string.Empty;
            }
        }

        public static string ImageExtension
        {
            get
            {
                switch (CurEnvirenment)
                {
                    case "CQ":
                        return ".cqimg";

                    case "Amanda":
                        return ".ini";
                }

                return string.Empty;
            }
        }
    }
}