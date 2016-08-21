using System;

namespace PaperPusher.Core
{
    public static class Settings
    {
        #region [ Properties ]

        public static string RenamePattern { get; set; }
            = @"{0:yyyy.MM.dd} - {1}{2}";

        public static string TrashFolderPath { get; set; }
            = "c:\\temp\\trash";

        #endregion
    }
}