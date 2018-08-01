using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml;

namespace ExprGenrator.Tools
{
    public class XmlSerializer
    {
        public static string Serialize(object obj)
        {
            if (obj == null) return string.Empty;

            var serializer = new DataContractSerializer(obj.GetType());
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb))
            {
                serializer.WriteObject(writer, obj);
            }
            return sb.ToString();
        }

        public static T Deserialize<T>(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return default(T);

            var serializer = new DataContractSerializer(typeof(T));
            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return (T)serializer.ReadObject(reader);
            }
        }

        public static object Deserialize(Type type, string xml)
        {
            if (string.IsNullOrEmpty(xml)) return null;

            using (var reader = XmlReader.Create(new StringReader(xml)))
            {
                return new DataContractSerializer(type).ReadObject(reader);
            }
        }

        public static string SerializeWithIndentation(object obj)
        {
            if (obj == null) return string.Empty;

            var settings = new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true, NewLineOnAttributes = true };

            var serializer = new DataContractSerializer(obj.GetType());
            var sb = new StringBuilder();
            using (var writer = XmlWriter.Create(sb, settings))
            {
                serializer.WriteObject(writer, obj);
            }
            return sb.ToString();
        }
    }
}
