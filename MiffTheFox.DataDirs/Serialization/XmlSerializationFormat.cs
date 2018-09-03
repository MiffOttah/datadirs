using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MiffTheFox.DataDirs.Serialization
{
    public class XmlSerializationFormat : ISerializationFormat
    {
        public T Read<T>(Stream source)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(source);
        }

        public void Write<T>(Stream destination, T data)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            xmlSerializer.Serialize(destination, data);
        }
    }
}
