using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs.Serialization
{
    public abstract class SerializationException : Exception
    {
        public SerializationException(string message) : base(message) { }
        public SerializationException(string message, Exception innerException) : base(message, innerException) { }
    }

    public class UnknownSerializationFormatException : Exception
    {
        public UnknownSerializationFormatException(string message) : base(message) { }
    }

    public class SerializationFailException : Exception
    {
        public SerializationFailException(string message, Exception innerException) : base(message, innerException) { }
    }
}
