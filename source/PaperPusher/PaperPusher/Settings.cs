using System;

namespace PaperPusher
{
    public static class Settings
    {
        #region [ Properties ]

        /// <summary>
        ///     Enable deletion of all files in the trash bin.
        /// </summary>
        public static bool DeleteTrashBinOnExit { get; set; }

        /// <summary>
        ///     Location to move deleted files. This location
        ///     can be set to delete its content on exit.
        /// </summary>
        public static string TrashBinPath { get; set; } = "c:\\temp\\trash";

        #endregion
    }
}