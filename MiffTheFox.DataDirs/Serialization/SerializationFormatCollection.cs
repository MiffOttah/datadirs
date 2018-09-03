using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs.Serialization
{
    public class SerializationFormatCollection : IDictionary<string, ISerializationFormat>
    {
        private readonly Dictionary<string, ISerializationFormat> _Formats = new Dictionary<string, ISerializationFormat>();

        public ISerializationFormat this[string key] { get => _Formats[_NormalizeExtension(key)]; set => _Formats[_NormalizeExtension(key)] = value; }
        public ICollection<string> Keys => _Formats.Keys;
        public ICollection<ISerializationFormat> Values => _Formats.Values;
        public int Count => _Formats.Count;
        public bool IsReadOnly => false;

        protected static string _NormalizeExtension(string extension)
        {
            if (string.IsNullOrEmpty(extension)) return string.Empty;
            return extension.ToLowerInvariant().TrimStart('.');
        }

        public void Add(string key, ISerializationFormat value) => _Formats.Add(_NormalizeExtension(key), value);
        public void Add(KeyValuePair<string, ISerializationFormat> item) => _Formats.Add(_NormalizeExtension(item.Key), item.Value);
        public void Clear() => _Formats.Clear();
        public bool Contains(KeyValuePair<string, ISerializationFormat> item) => _Formats.Contains(new KeyValuePair<string, ISerializationFormat>(_NormalizeExtension(item.Key), item.Value));
        public bool ContainsKey(string key) => _Formats.ContainsKey(_NormalizeExtension(key));

        public void CopyTo(KeyValuePair<string, ISerializationFormat>[] array, int arrayIndex) => ((IDictionary<string, ISerializationFormat>)_Formats).CopyTo(array, arrayIndex);
        public IEnumerator<KeyValuePair<string, ISerializationFormat>> GetEnumerator() => ((IDictionary<string, ISerializationFormat>)_Formats).GetEnumerator();
        public bool Remove(string key) => _Formats.Remove(key);
        public bool Remove(KeyValuePair<string, ISerializationFormat> item) => ((IDictionary<string, ISerializationFormat>)_Formats).Remove(item);
        public bool TryGetValue(string key, out ISerializationFormat value) => _Formats.TryGetValue(_NormalizeExtension(key), out value);
        IEnumerator IEnumerable.GetEnumerator() => ((IDictionary<string, ISerializationFormat>)_Formats).GetEnumerator();

        public static SerializationFormatCollection GetDefaultFormats()
        {
            return new SerializationFormatCollection
            {
                { "json", new JsonSerializationFormat() },
                { "xml", new XmlSerializationFormat() }
            };
        }
    }
}
