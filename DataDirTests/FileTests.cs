using System;
using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using MiffTheFox.DataDirs;

namespace DataDirTests
{
    [TestClass]
    public class FileTests
    {
        [TestMethod]
        public void TestOpenFile()
        {
            var contents = BinString.FromTextString("Hello, world!\n", System.Text.Encoding.ASCII);
            var dd = DataDir.Create(DataDirType.Temporary);

            string filename = DateTime.Now.Ticks.ToString("x") + ".tmp";
            string fullFilename = Path.Combine(dd.Path, filename);

            Assert.IsFalse(File.Exists(fullFilename));
            Assert.ThrowsException<FileNotFoundException>(() => dd.OpenFile(filename, FileMode.Open, FileAccess.Read));

            using (var stream = dd.OpenFile(filename, FileMode.Create, FileAccess.ReadWrite))
            {
                stream.Write(contents);
                stream.Close();
            }

            Assert.IsTrue(File.Exists(fullFilename));
            Assert.AreEqual(contents, (BinString)File.ReadAllBytes(fullFilename));

            using (var stream = dd.OpenFile(filename, FileMode.Open, FileAccess.Read))
            {
                var read = stream.ReadBinString(contents.Length);
                Assert.AreEqual(contents, read);
            }

            File.Delete(fullFilename);

            Assert.ThrowsException<ArgumentException>(() => dd.OpenFile(null, FileMode.Create, FileAccess.ReadWrite));
            Assert.ThrowsException<ArgumentException>(() => dd.OpenFile(string.Empty, FileMode.Create, FileAccess.ReadWrite));
        }

        [TestMethod]
        public void TestSimpleReadWrite()
        {
            string stringData = "Hello, world! This is a string.\n";
            var binData = BinString.FromBytes("09F911029D74E35BD84156C5635688C0");

            var dd = DataDir.Create(DataDirType.Temporary);

            dd.WriteAllText("test.txt", stringData, System.Text.Encoding.ASCII);
            Assert.AreEqual(stringData, dd.ReadAllText("test.txt", Encoding.ASCII));

            dd.WriteAllBytes("test.bin", binData);
            Assert.AreEqual(binData, dd.ReadAllBytes("test.bin"));
        }
    }
}
