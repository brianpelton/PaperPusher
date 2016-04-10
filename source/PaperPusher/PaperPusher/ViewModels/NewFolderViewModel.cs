using System.IO;
using Caliburn.Micro;

namespace PaperPusher.ViewModels
{
    public class NewFolderViewModel : Screen
    {
        #region [ Properties ]

        public DirectoryInfo BaseFolder { get; set; }
        public string FolderName { get; set; }

        #endregion

        #region [ Methods ]

        public void MakeFolder()
        {
            if (BaseFolder == null ||
                !BaseFolder.Exists)
                return;

            var newFolder = Path.Combine(BaseFolder.FullName, FolderName);
            Directory.CreateDirectory(newFolder);

            TryClose(true);
        }

        #endregion
    }
}