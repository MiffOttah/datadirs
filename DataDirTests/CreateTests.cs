using Microsoft.VisualStudio.TestTools.UnitTesting;
using MiffTheFox.DataDirs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DataDirTests
{
    [TestClass]
    public class CreateTests
    {
        [TestMethod]
        public void LocationCreateTests()
        {
            var l = new List<DataDir>();
            string fakeApplicationName = $"DataDir/Test {DateTime.Now:F}";

            l.Add(DataDir.Create(fakeApplicationName, type: DataDirType.RoamingUser));
            l.Add(DataDir.Create(fakeApplicationName, type: DataDirType.LocalUser));
            l.Add(DataDir.Create(fakeApplicationName, type: DataDirType.SystemWide));
            l.Add(DataDir.Create(fakeApplicationName, type: DataDirType.Temporary));
            l.Add(DataDir.Create(fakeApplicationName, type: DataDirType.SavedGameData));

            foreach (var d in l)
            {
                // DataDir.Create should create the directory on disk
                Assert.IsTrue(d.DirectoryExists);

                // clean up
                Directory.Delete(d.Path);
                Assert.IsFalse(d.DirectoryExists);
            }
        }

        [TestMethod]
        public void AutomaticNameTest()
        {
            string title = Assembly.GetExecutingAssembly().GetCustomAttributes<AssemblyTitleAttribute>().Select(attr => attr.Title).FirstOrDefault();

            var tempDirectory = DataDir.Create(DataDirType.Temporary);
            Assert.AreEqual(title, Path.GetFileName(tempDirectory.Path));
        }
    }
}
