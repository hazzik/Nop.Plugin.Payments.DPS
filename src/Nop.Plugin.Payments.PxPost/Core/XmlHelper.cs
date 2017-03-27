using System.IO;
using System.Xml.Serialization;

namespace Hazzik.Nop.Plugin.Payments.PxPost.Core
{
    public static class XmlHelper
    {
        public static MemoryStream ToStream<T>(this T request)
        {
            var stream = new MemoryStream();
            new XmlSerializer(typeof(T)).Serialize(stream, request);
            stream.Position = 0;
            return stream;
        }
    }
}