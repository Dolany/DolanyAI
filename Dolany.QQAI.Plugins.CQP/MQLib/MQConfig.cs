using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQLib
{
    public static class MQConfig
    {
        public const string CommandQueue = ".\\private$\\CommandMQ";

        public const string InformationQueue = ".\\private$\\InformationMQ";
    }
}
