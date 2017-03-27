using System.IO;
using System.Xml.Serialization;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Core
{
    public static class XmlSerializationHelper
    {
        public static MemoryStream ToStream<T>(this T request)
        {
            var stream = new MemoryStream();
            new XmlSerializer(typeof(T)).Serialize(stream, request);
            stream.Position = 0;
            return stream;
        }

        public static T FromStream<T>(Stream responseStream)
        {
            return (T) new XmlSerializer(typeof(T)).Deserialize(responseStream);
        }
    }
}