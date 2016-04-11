using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using ImageMagick;
using Microsoft.WindowsAPICodePack.Dialogs;
using PaperPusher.Utility;
using PropertyChanged;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace PaperPusher.ViewModels
{
    [Export(typeof (MainViewModel))]
    [ImplementPropertyChanged]
    public class MainViewModel : Screen
    {
        #region [ Logging ]

        private static readonly ILog Log = LogManager.GetLogger(typeof (MainViewModel));

        #endregion

        #region [ Constructors ]

        public MainViewModel()
        {
            DisplayName = "PaperPusher";
            CurrentDirectory = new DirectoryInfo("c:\\temp\\pdfs");
            TargetRootDirectory = new DirectoryInfo("c:\\temp\\target");
        }

        #endregion

        #region [ Properties ]

        public bool CanDeleteDocument => SelectedCurrentFile != null;
        public bool CanMoveDocument => SelectedCurrentFile != null;

        public bool CanRenameAndMoveDocument
        {
            get
            {
                if (SelectedCurrentFile == null)
                    return false;

                if (DocumentTitle == null)
                    return false;

                return DocumentDate.HasValue;
            }
        }

        public DirectoryInfo CurrentDirectory { get; set; }

        public BindingList<FileInfo> CurrentFiles { get; protected set; }
            = new BindingList<FileInfo>();

        public DateTime? DocumentDate { get; set; }
        public string DocumentTitle { get; set; }
        public BitmapSource PreviewImage { get; private set; }
        public string PreviewImageFilename { get; private set; }
        public FileInfo SelectedCurrentFile { get; set; }
        public DirectoryInfo SelectedTargetDirectory { get; set; }

        public BindingList<DirectoryInfo> TargetDirectories { get; protected set; }
            = new BindingList<DirectoryInfo>();

        public DirectoryInfo TargetRootDirectory { get; set; }

        #endregion

        #region [ Methods ]

        public void ChooseSourceDirectory()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
                return;

            var directory = new DirectoryInfo(dialog.FileName);
            if (!directory.Exists)
                return;

            CurrentDirectory = directory;
        }

        public void ChooseTargetRootDirectory()
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
                return;

            var directory = new DirectoryInfo(dialog.FileName);
            if (!directory.Exists)
                return;

            TargetRootDirectory = directory;
        }

        public void CreateDirectory()
        {
            var windowManager = IoC.Get<IWindowManager>();
            var vm = new NewFolderViewModel
            {
                BaseFolder = TargetRootDirectory
            };
            windowManager.ShowDialog(vm);

            RefreshDirectories();

            var folder = (from f in TargetDirectories
                where f.Name == vm.FolderName
                select f).FirstOrDefault();

            SelectedTargetDirectory = folder;
        }

        public void DeleteDocument()
        {
            try
            {
                SendFileToRecycleBin.RecycleFile(SelectedCurrentFile);
                AfterAction();
            }
            catch (Exception ex)
            {
                Log.Error("Error deleteting document.", ex);
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        public void MoveDocument()
        {
            try
            {
                var newFilename = Path.Combine(SelectedTargetDirectory.FullName, SelectedCurrentFile.Name);
                SelectedCurrentFile.MoveTo(newFilename);

                AfterAction();
            }
            catch (Exception ex)
            {
                Log.Error("Error moving document.", ex);
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        public void RenameAndMoveDocument()
        {
            try
            {
                var extension = Path.GetExtension(SelectedCurrentFile.Name);
                var newName = $"{DocumentDate:yyyy.MM.dd} - {DocumentTitle}{extension}";
                var newFilename = Path.Combine(SelectedTargetDirectory.FullName, newName);
                SelectedCurrentFile.MoveTo(newFilename);

                AfterAction();
            }
            catch (Exception ex)
            {
                Log.Error("Error renameing / moving document.", ex);
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        private void AfterAction()
        {
            var index = CurrentFiles.IndexOf(SelectedCurrentFile);
            CurrentFiles.Remove(SelectedCurrentFile);
            SelectedCurrentFile = CurrentFiles[index];

            DocumentTitle = null;
            DocumentDate = null;
        }

        private void OnCurrentDirectoryChanged()
        {
            CurrentFiles.Clear();

            if (CurrentDirectory == null ||
                !CurrentDirectory.Exists)
                return;

            foreach (var file in CurrentDirectory.GetFiles())
                CurrentFiles.Add(file);
        }

        private async void OnSelectedCurrentFileChanged()
        {
            if (SelectedCurrentFile == null ||
                !SelectedCurrentFile.Exists)
                return;

            var settings = new MagickReadSettings
            {
                Density = new Density(100, 100),
                FrameIndex = 0,
                FrameCount = 1
            };

            var filename = Path.GetTempFileName();
            await Task.Run(() =>
            {
                using (var images = new MagickImageCollection())
                {
                    images.Read(SelectedCurrentFile, settings);

                    var image = images.First();
                    image.Format = MagickFormat.Jpeg;
                    images.Write(filename);
                }
            });

            try
            {
                var uri = new Uri(filename);
                var bitmap = new BitmapImage(uri);
                PreviewImage = bitmap;
                PreviewImageFilename = filename;
            }
            catch (Exception ex)
            {
                Log.Error("Error generating preview image.", ex);
            }
        }

        private void OnTargetRootDirectoryChanged()
        {
            RefreshDirectories();
        }

        private void RefreshDirectories()
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