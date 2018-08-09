using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace TicketRusherProj.Utilities
{
    public class SerializeHelper
    {
        public static byte[] SerializeBinary(object request)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            serializer.Serialize(memStream, request);
            return memStream.GetBuffer();
        }

        public static object DeserializeBinary(byte[] buf)
        {
            MemoryStream memStream = new MemoryStream(buf);
            memStream.Position = 0;
            BinaryFormatter deserializer =
                new BinaryFormatter();
            object newobj = deserializer.Deserialize(memStream);
            memStream.Close();
            return newobj;
        }
    }
}