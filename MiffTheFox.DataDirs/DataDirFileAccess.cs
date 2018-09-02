using System;
using System.IO;
using System.Text;
using MiffTheFox;
using PathType = System.IO.Path;

namespace MiffTheFox.DataDirs
{
    partial class DataDir
    {
        public Stream OpenFile(string filePath, FileMode mode, FileAccess access)
        {
            if (string.IsNullOrEmpty(filePath)) throw new ArgumentException(nameof(filePath), "File path cannot be null or empty.");

            string fullPath = PathType.Combine(Path, filePath);
            string dir = PathType.GetDirectoryName(fullPath);
            bool createIfNotExists = false;

            if (access != FileAccess.Read)
            {
                switch (mode)
                {
                    case FileMode.Append:
                    case FileMode.Create:
                    case FileMode.CreateNew:
                    case FileMode.OpenOrCreate:
                        createIfNotExists = true;
                        break;
                }
            }

            if (!Directory.Exists(dir))
            {
                if (createIfNotExists)
                {
                    Directory.CreateDirectory(dir);
                }
                else
                {
                    throw new FileNotFoundException();
                }
            }

            return File.Open(fullPath, mode, access);
        }

        public BinString ReadAllBytes(string filePath)
        {
            using (var stream = OpenFile(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var buffer = new MemoryStream())
                {
                    stream.CopyTo(buffer);
                    return buffer.ToBinString();
                }
            }
        }

        public void WriteAllBytes(string filePath, BinString data)
        {
            using (var stream = OpenFile(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.Write(data);
            }
        }

        public string ReadAllText(string filePath, Encoding encoding) => ReadAllBytes(filePath).ToString(encoding);
        public void WriteAllText(string filePath, string data, Encoding encoding) => WriteAllBytes(filePath, BinString.FromTextString(data, encoding));
    }
}
