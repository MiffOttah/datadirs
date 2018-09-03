using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using MiffTheFox.DataDirs;
using System;
using System.IO;

namespace DataDirTests
{
    [TestClass]
    public class SerializationTests
    {
        private struct TestObject
        {
            public int SomeInt;
            public double SomeDouble;
            public string SomeString;
            public int[] SomeNumbers;

            public void Test(TestObject that)
            {
                Assert.AreEqual(this.SomeInt, that.SomeInt);
                Assert.AreEqual(this.SomeDouble, that.SomeDouble);
                Assert.AreEqual(this.SomeString, that.SomeString);
                CollectionAssert.AreEqual(this.SomeNumbers, that.SomeNumbers);
            }
        }

        [TestMethod]
        public void FailedSerTest()
        {
            var dd = DataDir.Create(DataDirType.Temporary);
            dd.CreateIfNotExists();
            string filename = "notfound-" + DateTime.UtcNow.Ticks.ToString("x") + ".json";

            Assert.ThrowsException<FileNotFoundException>(() => dd.ReadObject<TestObject>(filename));
            Assert.ThrowsException<MiffTheFox.DataDirs.Serialization.UnknownSerializationFormatException>(() => dd.WriteObject("invalid.xyz", 42));

            filename = "invalid-" + DateTime.UtcNow.Ticks.ToString("x") + ".json";
            dd.WriteAllText(filename, "}{}!{$%{!%#()%(!@#%}!@#{$!}#%", System.Text.Encoding.Unicode);
            Assert.ThrowsException<MiffTheFox.DataDirs.Serialization.SerializationFailException>(() => dd.ReadObject<TestObject>(filename));
        }

        [TestMethod]
        public void JsonSerTest()
        {
            var dd = DataDir.Create(DataDirType.Temporary);
            dd.CreateIfNotExists();

            string filename = DateTime.UtcNow.Ticks.ToString("x") + ".json";
            var data = new TestObject
            {
                SomeInt = 42,
                SomeDouble = 3.14,
                SomeString = "asdfghjkl",
                SomeNumbers = new int[] { 1, 2, 3, 4, 5 }
            };

            dd.WriteObject(filename, data);

            var data2 = dd.ReadObject<TestObject>(filename);
            data.Test(data2);

            dd.DeleteFile(filename);
        }
    }
}
