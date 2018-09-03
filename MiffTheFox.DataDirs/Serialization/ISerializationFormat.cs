using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs.Serialization
{
    public interface ISerializationFormat
    {
        void Write<T>(Stream destination, T data);
        T Read<T>(Stream source);
    }
}
