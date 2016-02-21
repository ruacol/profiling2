using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Profiling2.Infrastructure.Util
{
    public static class StreamUtil
    {
        public static byte[] StreamToBytes(Stream stream)
        {
            MemoryStream memoryStream = stream as MemoryStream;
            if (memoryStream == null)
            {
                memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
            }
            return memoryStream.ToArray();
        }

        /// <summary>
        /// Use BinaryFormatter to serialize to byte array.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static byte[] Serialize(object obj)
        {
            if (obj == null)
                return null;

            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(ms, obj);
                return StreamUtil.StreamToBytes(ms);
            }
        }

        /// <summary>
        /// Use BinaryFormatter to deserialize a byte array.
        /// </summary>
        /// <param name="binary"></param>
        /// <returns></returns>
        public static object Deserialize(byte[] binary)
        {
            if (binary == null)
                return null;

            BinaryFormatter serializer = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(binary))
                return serializer.Deserialize(ms);
        }
    }
}
