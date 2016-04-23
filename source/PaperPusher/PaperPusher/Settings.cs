using System;

namespace PaperPusher
{
    public static class Settings
    {
        #region [ Constructors ]

        static Settings()
        {
            InitialSourceDirectory = Properties.Settings.Default.SourceDirectory;
            InitialTargetRootDirectory = Properties.Settings.Default.TargetRootDirectory;

            if (string.IsNullOrEmpty(InitialSourceDirectory))
                InitialSourceDirectory = Environment.CurrentDirectory;

            if (string.IsNullOrEmpty(InitialTargetRootDirectory))
                InitialTargetRootDirectory = Environment.CurrentDirectory;
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
        public static string InitialSourceDirectory { get; set; }

        /// <summary>
        ///     The path to the root folder that contains folders to sort files into.
        /// </summary>
        public static string InitialTargetRootDirectory { get; set; }

        /// <summary>
        ///     Location to move deleted files. This location
        ///     can be set to delete its content on exit.
        /// </summary>
        public static string TrashBinPath { get; set; } = "c:\\temp\\trash";

        #endregion

        #region [ Methods ]

        public static void Save()
        {
            Properties.Settings.Default.SourceDirectory = InitialSourceDirectory;
            Properties.Settings.Default.TargetRootDirectory = InitialTargetRootDirectory;
            Properties.Settings.Default.Save();
        }

        #endregion
    }
}