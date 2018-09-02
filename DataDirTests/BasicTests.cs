using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox.DataDirs;

namespace DataDirTests
{
    [TestClass]
    public class BasicTests
    {
        [TestMethod]
        public void TestCreation()
        {
            string testPath = Path.Combine(Path.GetTempPath(), "DataDirTests", "creation");

            var d1 = new DataDir(testPath);
            Assert.AreEqual(testPath, d1.Path);
            Assert.AreEqual(testPath, d1.ToString());

            var d2 = new DataDir(testPath);
            Assert.IsTrue(d1.Equals(d2));
            Assert.IsTrue(d2.Equals(d1));

            var d3 = new DataDir(Path.Combine(Path.GetTempPath(), "DataDirTests", "other"));
            Assert.IsFalse(d1.Equals(d3));
            Assert.IsFalse(d3.Equals(d1));

            Assert.ThrowsException<ArgumentException>(() => new DataDir(null));
            Assert.ThrowsException<ArgumentException>(() => new DataDir(string.Empty));
            Assert.ThrowsException<ArgumentException>(() => new DataDir(testPath + Path.GetInvalidPathChars()[0]));
        }

        [TestMethod]
        public void TestDirectoryCreation()
        {
            var rng = new Random();
            string basePath = Path.Combine(Path.GetTempPath(), "DataDirTests", "creation");

            string testPath;
            do
            {
                testPath = $"{basePath}_{DateTime.Now.Ticks:x}_{rng.Next():x}";
            } while (Directory.Exists(testPath));

            var dd = new DataDir(testPath);
            Assert.IsFalse(dd.DirectoryExists);

            dd.CreateIfNotExists();
            Assert.IsTrue(dd.DirectoryExists);
            Assert.IsTrue(Directory.Exists(testPath));

            dd.CreateIfNotExists();

            Directory.Delete(testPath);
            Assert.IsFalse(dd.DirectoryExists);
        }

        [TestMethod]
        public void TestSubdirectory()
        {
            var rng = new Random();
            string basePath = Path.Combine(Path.GetTempPath(), "DataDirTests", "subdir");

            string testPath;
            do
            {
                testPath = $"{basePath}_{DateTime.Now.Ticks:x}_{rng.Next():x}";
            } while (Directory.Exists(testPath));

            var dd = new DataDir(testPath);
            Assert.IsFalse(dd.DirectoryExists);

            dd.CreateIfNotExists();
            Assert.IsTrue(dd.DirectoryExists);
            Assert.IsTrue(Directory.Exists(testPath));

            var subdir = dd.Subdirectory("subdir");
            Assert.IsFalse(subdir.DirectoryExists);
            subdir.CreateIfNotExists();
            Assert.IsTrue(subdir.DirectoryExists);

            Directory.Delete(subdir.Path);
            Directory.Delete(testPath);
            Assert.IsFalse(dd.DirectoryExists);
        }
    }
}
