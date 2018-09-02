using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiffTheFox.DataDirs
{
    /// <summary>
    /// Specifies a type of data directory to create
    /// </summary>
    public enum DataDirType
    {
        /// <summary>
        /// Specifies a user-specific data directory, that on a NT system with profile roaming, will roam across the domain.
        /// </summary>
        RoamingUser = 0,

        /// <summary>
        /// Specifies a user-specific data directory, that on a NT system with profile roaming, will not roam.
        /// </summary>
        LocalUser = 1,

        /// <summary>
        /// Specifies a system-specific data directory, shared between all users.
        /// </summary>
        SystemWide,

        /// <summary>
        /// Specifies a temporary data directory that may be deleted at any time.
        /// </summary>
        Temporary,

        /// <summary>
        /// Specifies a user-specific game data directory.
        /// </summary>
        SavedGameData
    }
}
