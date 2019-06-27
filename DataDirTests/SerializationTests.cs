using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox;
using MiffTheFox.DataDirs;
using System;
using System.Collections.Generic;
using System.IO;

namespace DataDirTests
{
    [TestClass]
    public class SerializationTests
    {
        [Serializable]
        public struct TestObject : IEquatable<TestObject>
        {
            public int SomeInt;
            public double SomeDouble;
            public string SomeString;
            public int[] SomeNumbers;

            public override bool Equals(object obj)
            {
                return obj is TestObject && Equals((TestObject)obj);
            }

            public bool Equals(TestObject other)
            {
                return SomeInt == other.SomeInt &&
                       SomeDouble == other.SomeDouble &&
                       SomeString == other.SomeString &&
                       System.Linq.Enumerable.SequenceEqual(SomeNumbers, other.SomeNumbers);
            }

            public override int GetHashCode()
            {
                var hashCode = 533473783;
                hashCode = hashCode * -1521134295 + SomeInt.GetHashCode();
                hashCode = hashCode * -1521134295 + SomeDouble.GetHashCode();
                hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SomeString);
                hashCode = hashCode * -1521134295 + EqualityComparer<int[]>.Default.GetHashCode(SomeNumbers);
                return hashCode;
            }

            public static bool operator ==(TestObject object1, TestObject object2)
            {
                return object1.Equals(object2);
            }

            public static bool operator !=(TestObject object1, TestObject object2)
            {
                return !(object1 == object2);
            }

            public override string ToString() => $"SomeInt={SomeInt}, SomeDouble={SomeDouble}, SomeString={SomeString}, SomeNumbers={string.Join(",", SomeNumbers)}";
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
        public void ReadObjectWithCreateTest()
        {
            var dd = DataDir.Create(DataDirType.Temporary);
            dd.CreateIfNotExists();

            string filename = dd.GetNewFileName(extension: "json");
            Assert.IsFalse(dd.FileExists(filename));

            var data = new TestObject
            {
                SomeInt = 12345,
                SomeDouble = 54.321,
                SomeString = "ABCD 1234567 abcd",
                SomeNumbers = new int[] { -10, -20, 15, -8 }
            };

            var test1 = dd.ReadObject<TestObject>(filename, () => data);
            Assert.AreEqual(test1, data);

            test1.SomeInt = -117;
            test1.SomeDouble = -42.5;
            dd.WriteObject(filename, test1);

            var test2 = dd.ReadObject<TestObject>(filename, () => data);
            Assert.AreEqual(test1, test2);
            Assert.AreNotEqual(data, test2);
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
            Assert.AreEqual(data, data2);

            dd.DeleteFile(filename);
        }

        [TestMethod]
        public void XmlSerTest()
        {
            var dd = DataDir.Create(DataDirType.Temporary);
            dd.CreateIfNotExists();

            string filename = DateTime.UtcNow.Ticks.ToString("x") + ".xml";
            var data = new TestObject
            {
                SomeInt = 123,
                SomeDouble = 45.6,
                SomeString = "Ξεσκεπάζω τὴν ψυχοφθόρα βδελυγμία",
                SomeNumbers = new int[] { 12, 11, 10 }
            };

            dd.WriteObject(filename, data);

            var data2 = dd.ReadObject<TestObject>(filename);
            Assert.AreEqual(data, data2);

            dd.DeleteFile(filename);
        }

        [TestMethod]
        public void BinarySerTest()
        {
            var dd = DataDir.Create(DataDirType.Temporary);
            dd.CreateIfNotExists();

            var data = new TestObject
            {
                SomeInt = 0xc0ffee,
                SomeDouble = 12345.54321,
                SomeString = "the quick brown fox jumps over the lazy dog",
                SomeNumbers = new int[] { 1, 2, 3, 4, -10, -20, -30, -40 }
            };

            dd.SerializationFormats.Add(".custom", new MiffTheFox.DataDirs.Serialization.BinarySerializationFormat());
            string filename = DateTime.UtcNow.Ticks.ToString("x") + ".custom";

            dd.WriteObject(filename, data);

            var data2 = dd.ReadObject<TestObject>(filename);
            Assert.AreEqual(data, data2);

            dd.DeleteFile(filename);
        }
    }
}
