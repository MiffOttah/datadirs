using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs.Serialization
{
    public class JsonSerializationFormat : ISerializationFormat
    {
        JsonSerializer Serializer { get; set; } = new JsonSerializer();

        public T Read<T>(Stream source)
        {
            // the JSON specification requires UTF-8 encoding
            using (var reader = new StreamReader(source, Encoding.UTF8))
            {
                return Serializer.Deserialize<T>(new JsonTextReader(reader));
            }
        }

        public void Write<T>(Stream destination, T data)
        {
            // The default Encoding.UTF8 emits a nonstandard BOM,
            // so we have to create a new encoding object to use.
            using (var writer = new StreamWriter(destination, new UTF8Encoding(false)))
            {
                Serializer.Serialize(writer, data);
            }
        }
    }
}
