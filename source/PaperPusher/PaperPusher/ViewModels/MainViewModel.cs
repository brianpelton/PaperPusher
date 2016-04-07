using System.ComponentModel;
using System.IO;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using PropertyChanged;

namespace PaperPusher.ViewModels
{
    [ImplementPropertyChanged]
    public class MainViewModel : Screen
    {
        #region [ Constructors ]

        public MainViewModel()
        {
            CurrentDirectory = new DirectoryInfo("c:\\temp\\pdfs");
            TargetRootDirectory = new DirectoryInfo("c:\\temp\\target");
        }

        #endregion

        #region [ Properties ]

        public DirectoryInfo CurrentDirectory { get; set; }

        public BindingList<FileInfo> CurrentFiles { get; protected set; }
            = new BindingList<FileInfo>();

        public BitmapSource PreviewImage { get; private set; }
        public FileInfo SelectedCurrentFile { get; set; }

        public BindingList<DirectoryInfo> TargetDirectories { get; protected set; }
            = new BindingList<DirectoryInfo>();

        public DirectoryInfo TargetRootDirectory { get; set; }

        #endregion

        #region [ Methods ]

        private void OnCurrentDirectoryChanged()
        {
            CurrentFiles.Clear();

            if (CurrentDirectory == null ||
                !CurrentDirectory.Exists)
                return;

            foreach (var file in CurrentDirectory.GetFiles())
                CurrentFiles.Add(file);
        }

        private void OnSelectedCurrentFileChanged()
        {
            //todo: generate pdf preview.
        }

        private void OnTargetRootDirectoryChanged()
        {
            TargetDirectories.Clear();

            if (TargetRootDirectory == null ||
                !TargetRootDirectory.Exists)
                return;

            foreach (var directory in TargetRootDirectory.GetDirectories())
                TargetDirectories.Add(directory);
        }

        #endregion
    }
}