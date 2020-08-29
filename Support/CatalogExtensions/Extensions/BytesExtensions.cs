using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dom.Extensions
{
    public static class BytesExtensions
    {
        private static readonly DateTime ZeroData = new DateTime(2010, 10, 1, 0, 0, 20);
        public static object Deserialize(this byte[] bytes)
        {
            var stream = new MemoryStream(bytes);
            var binaryFormatter = new BinaryFormatter();
            var deserializedObject = binaryFormatter.Deserialize(stream);
            return deserializedObject;
        }
        public static object DateTimeFromBytes(this byte[] bytes)
        {
            var seconds = BitConverter.ToInt32(bytes, 0);
            return ZeroData.AddSeconds(seconds);
        }
        public static string ToStr(this byte[] bytes)
        {
            var result = string.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                result += bytes[i];
                if (i != bytes.Length - 1)
                {
                    result += "; ";
                }
            }

            return result;
        }
    }
}
