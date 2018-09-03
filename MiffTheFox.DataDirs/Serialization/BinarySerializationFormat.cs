using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs.Serialization
{
    public class BinarySerializationFormat : ISerializationFormat
    {
        public T Read<T>(Stream source)
        {
            var formatter = new BinaryFormatter();
            return (T)formatter.Deserialize(source);
        }

        public void Write<T>(Stream destination, T data)
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(destination, data);
        }
    }
}
