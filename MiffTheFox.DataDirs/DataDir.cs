using System;
using System.IO;
using PathType = System.IO.Path;

namespace MiffTheFox.DataDirs
{
    public partial class DataDir
    {
        public string Path { get; }
        public bool DirectoryExists => Directory.Exists(Path);

        public DataDir(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException(nameof(path), "Path cannot be null or empty.");
            if (path.IndexOfAny(PathType.GetInvalidPathChars()) != -1) throw new ArgumentException(nameof(path), "Path contains an invalid character.");

            Path = path;
        }

        public override string ToString() => Path;
        public override int GetHashCode() => Path.GetHashCode();
        public override bool Equals(object obj) => obj is DataDir that ? Path.Equals(that.Path) : base.Equals(obj);

        /// <summary>Creates the data directory if it doesn't exist.</summary>
        public void CreateIfNotExists() => Directory.CreateDirectory(Path);

        public DataDir Subdirectory(string name)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentException(nameof(name), "Name cannot be null or empty.");
            return new DataDir(PathType.Combine(Path, name));
        }
    }
}
