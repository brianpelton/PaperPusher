using System.IO;
using Caliburn.Micro;
using PropertyChanged;

namespace PaperPusher.ViewModels
{
    public class NewFolderViewModel : Screen
    {
        #region [ Constructors ]

        public NewFolderViewModel()
        {
            DisplayName = "PaperPusher";
        }

        #endregion

        #region [ Properties ]

        public bool CanCreateFolder => !string.IsNullOrEmpty(FolderName);
        public DirectoryInfo BaseFolder { get; set; }
        public string FolderName { get; set; }
        public DirectoryInfo NewFolder { get; private set; }

        #endregion

        #region [ Methods ]

        public void CreateFolder()
        {
            if (BaseFolder == null ||
                !BaseFolder.Exists)
                return;

            var path = Path.Combine(BaseFolder.FullName, FolderName);
            NewFolder = Directory.CreateDirectory(path);

            TryClose(true);
        }

        #endregion
    }
}