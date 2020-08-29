using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dom.Extensions
{
    public static class ObjectExtensions
    {
        public static byte[] Serialize(this object obj)
        {
            var memoryStream = new MemoryStream();
            IFormatter binFormat = new BinaryFormatter();

            binFormat.Serialize(memoryStream, obj);
            memoryStream.Flush();
            memoryStream.Position = 0;

            return memoryStream.ToArray();
        }
    }
}
