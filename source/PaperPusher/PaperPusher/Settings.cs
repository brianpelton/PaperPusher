using System;

namespace PaperPusher
{
    /// <summary>
    ///     Application Settings backed by Properties.Settings
    /// </summary>
    public static class Settings
    {
        #region [ Constructors ]

        static Settings()
        {
            SourceDirectory = Properties.Settings.Default.SourceDirectory;
            TargetRootDirectory = Properties.Settings.Default.TargetRootDirectory;
            TrashFolder = Properties.Settings.Default.TrashFolderName;

            if (string.IsNullOrEmpty(SourceDirectory))
                SourceDirectory = Environment.CurrentDirectory;

            if (string.IsNullOrEmpty(TargetRootDirectory))
                TargetRootDirectory = Environment.CurrentDirectory;
        }

        #endregion

        #region [ Properties ]

        /// <summary>
        ///     Enable deletion of all files in the trash bin.
        /// </summary>
        public static bool DeleteTrashBinOnExit { get; set; }

        /// <summary>
        ///     The path to the folder that contains files to sort / preview.
        /// </summary>
        public static string SourceDirectory { get; set; }

        /// <summary>
        ///     The path to the root folder that contains folders to sort files into.
        /// </summary>
        public static string TargetRootDirectory { get; set; }

        /// <summary>
        /// Path to the folder to move files
        /// when they are requested to be deleted.
        /// </summary>
        public static string TrashFolder { get; set; } 

        #endregion

        #region [ Methods ]

        public static void Save()
        {
            Properties.Settings.Default.SourceDirectory = SourceDirectory;
            Properties.Settings.Default.TargetRootDirectory = TargetRootDirectory;
            Properties.Settings.Default.TrashFolderName = TrashFolder;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}