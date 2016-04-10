using System.IO;
using Caliburn.Micro;

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

        public DirectoryInfo BaseFolder { get; set; }
        public string FolderName { get; set; }

        #endregion

        #region [ Methods ]

        public void CreateFolder()
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