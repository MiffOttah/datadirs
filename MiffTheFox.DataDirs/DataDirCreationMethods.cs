using System;
using System.Linq;
using System.Reflection;
using System.Text;
using PathType = System.IO.Path;

namespace MiffTheFox.DataDirs
{
    partial class DataDir
    {
        /// <summary>
        /// Creates a DataDir for a generic path for the provided application name.
        /// </summary>
        public static DataDir Create(string applicationName, string companyName = null, DataDirType type = DataDirType.RoamingUser)
        {
            if (string.IsNullOrEmpty(applicationName)) throw new ArgumentException(nameof(applicationName), "Application name cannot be null or empty.");

            bool isUnix = Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX;
            string parentDirectory;

            string homeDirectory = Environment.GetEnvironmentVariable("HOME");
            if (!isUnix && string.IsNullOrEmpty(homeDirectory))
            {
                homeDirectory = Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            }
            if (string.IsNullOrEmpty(homeDirectory))
            {
                homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            }

            switch (type)
            {
                case DataDirType.RoamingUser:
                    parentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    if (string.IsNullOrEmpty(parentDirectory)) parentDirectory = PathType.Combine(homeDirectory, ".config");
                    break;

                case DataDirType.LocalUser:
                    parentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    if (string.IsNullOrEmpty(parentDirectory)) parentDirectory = PathType.Combine(homeDirectory, ".local", "share");
                    break;

                case DataDirType.SystemWide:
                    parentDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                    if (string.IsNullOrEmpty(parentDirectory)) parentDirectory = isUnix ? "/usr/share" : PathType.Combine(homeDirectory, "\\ProgramData");
                    break;

                case DataDirType.Temporary:
                    parentDirectory = PathType.GetTempPath();
                    break;

                case DataDirType.SavedGameData:
                    parentDirectory = Native.GetKnownFolderPath(
                        new Guid("4C5C32FF-BB9D-43b0-B5B4-2D72E54EAAA4"),
                        isUnix ?
                            PathType.Combine(homeDirectory, ".local/share/games") :
                            PathType.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games")
                    );
                    break;

                default:
                    throw new InvalidOperationException("Unsupported data directory type: " + type.ToString());
            }

            var invalids = PathType.GetInvalidFileNameChars();
            var dataDirName = new StringBuilder();
            bool isValid;

            // company name subdirectories are common on windows but not on unix
            // therefore, only add the company name if not on unix
            if (!string.IsNullOrEmpty(companyName) && !isUnix)
            {
                foreach (char c in companyName)
                {
                    isValid = true;
                    for (int i = 0; i < invalids.Length; i++)
                    {
                        if (invalids[i] == c)
                        {
                            isValid = false;
                            break;
                        }
                    }
                    dataDirName.Append(isValid ? c : '_');
                }
                dataDirName.Append(PathType.DirectorySeparatorChar);
            }

            foreach (char c in applicationName)
            {
                isValid = true;
                for (int i = 0; i < invalids.Length; i++)
                {
                    if (invalids[i] == c)
                    {
                        isValid = false;
                        break;
                    }
                }
                dataDirName.Append(isValid ? c : '_');
            }

            // check for reserved file names, and do something about it if necessary
            foreach (string part in dataDirName.ToString().Split(PathType.DirectorySeparatorChar))
            {
                isValid = true;

                if (part == "." || part == "..")
                {
                    isValid = false;
                }

                // thanks dos. thos.
                if (!isUnix)
                {
                    if (part.Length == 3)
                    {
                        switch (part.ToUpperInvariant())
                        {
                            case "CON":
                            case "PRN":
                            case "AUX":
                            case "NUL":
                                isValid = false;
                                break;
                        }
                    }
                    else if (part.Length == 4)
                    {
                        switch (part.Substring(0, 3).ToUpperInvariant())
                        {
                            case "COM":
                            case "LPT":
                                isValid = false;
                                break;
                        }
                    }
                }

                if (isValid)
                {
                    parentDirectory = PathType.Combine(parentDirectory, part);
                }
                else
                {
                    parentDirectory = PathType.Combine(parentDirectory, part + "_");
                }
            }

            var dir = new DataDir(parentDirectory);
            dir.CreateIfNotExists();
            return dir;
        }

        /// <summary>
        /// Creates a DataDir for a generic path for the calling assembly.
        /// </summary>
        public static DataDir Create(DataDirType type = DataDirType.RoamingUser)
        {
            var calling = Assembly.GetCallingAssembly();

            string title = calling.GetCustomAttributes<AssemblyTitleAttribute>().Select(attr => attr.Title).FirstOrDefault();
            string companyName = calling.GetCustomAttributes<AssemblyCompanyAttribute>().Select(attr => attr.Company).FirstOrDefault();
            return Create(string.IsNullOrEmpty(title) ? calling.GetName().Name : title, companyName, type);
        }
    }
}
