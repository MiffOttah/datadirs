using System;
using System.IO;

using PathType = System.IO.Path;

namespace MiffTheFox.DataDirs
{
    public class DataDir
    {
        public string Path { get; }

        public DataDir(string path)
        {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException(nameof(path), "Path cannot be null or empty.");
            if (path.IndexOfAny(PathType.GetInvalidPathChars()) != -1) throw new ArgumentException(nameof(path), "Path contains an invalid character.");

            Path = path;
        }

        public override string ToString() => Path;
        public override int GetHashCode() => Path.GetHashCode();
        public override bool Equals(object obj) => obj is DataDir that ? Path.Equals(that.Path) : base.Equals(obj);

        #region Manipulation of the directory itself

        public bool DirectoryExists => Directory.Exists(Path);

        /// <summary>Creates the data directory if it doesn't exist.</summary>
        public void CreateIfNotExists() => Directory.CreateDirectory(Path);

        #endregion
    }
}
