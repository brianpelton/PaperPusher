using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using iTextSharp.text.pdf;
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
            SourceDirectory = new DirectoryInfo("c:\\temp\\pdfs");
            TargetRootDirectory = new DirectoryInfo("c:\\temp\\target");
        }

        #endregion

        #region [ Properties ]

        public bool CanDeleteDocument => SelectedSourceFile != null;
        public bool CanMoveDocument => SelectedSourceFile != null;

        public bool CanRenameAndMoveDocument
        {
            get
            {
                if (SelectedSourceFile == null)
                    return false;

                if (DocumentTitle == null)
                    return false;

                return DocumentDate.HasValue;
            }
        }

        public DateTime? DocumentDate { get; set; }
        public string DocumentTitle { get; set; }
        public BitmapSource PreviewImage { get; private set; }
        public string PreviewImageFilename { get; private set; }
        public int SelectedFilePageCount { get; private set; }
        public FileInfo SelectedSourceFile { get; set; }
        public DirectoryInfo SelectedTargetDirectory { get; set; }
        public DirectoryInfo SourceDirectory { get; set; }

        public BindingList<FileInfo> SourceFiles { get; protected set; }
            = new BindingList<FileInfo>();

        public BindingList<DirectoryInfo> TargetDirectories { get; protected set; }
            = new BindingList<DirectoryInfo>();

        public DirectoryInfo TargetRootDirectory { get; set; }

        #endregion

        #region [ Methods ]

        public void ChooseSourceDirectory()
        {
            var directory = ChooseDirectory();
            if (directory != null)
                SourceDirectory = directory;
        }

        public void ChooseTargetRootDirectory()
        {
            var directory = ChooseDirectory();
            if (directory != null)
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

            if (vm.NewFolder == null)
                return;

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
                SendFileToRecycleBin.RecycleFile(SelectedSourceFile);
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
                var newFilename = Path.Combine(SelectedTargetDirectory.FullName, SelectedSourceFile.Name);
                SelectedSourceFile.MoveTo(newFilename);

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
                var extension = Path.GetExtension(SelectedSourceFile.Name);
                var newName = $"{DocumentDate:yyyy.MM.dd} - {DocumentTitle}{extension}";
                var newFilename = Path.Combine(SelectedTargetDirectory.FullName, newName);
                SelectedSourceFile.MoveTo(newFilename);

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
            var index = SourceFiles.IndexOf(SelectedSourceFile);
            SourceFiles.Remove(SelectedSourceFile);
            SelectedSourceFile = SourceFiles[index];

            DocumentTitle = null;
            DocumentDate = null;
        }

        private DirectoryInfo ChooseDirectory()
        {
            var dialog = new CommonOpenFileDialog
            {
                IsFolderPicker = true
            };
            var result = dialog.ShowDialog();
            if (result != CommonFileDialogResult.Ok)
                return null;

            var directory = new DirectoryInfo(dialog.FileName);
            if (!directory.Exists)
                return null;

            return directory;
        }

        private void OnSourceDirectoryChanged()
        {
            SourceFiles.Clear();

            if (SourceDirectory == null ||
                !SourceDirectory.Exists)
                return;

            foreach (var file in SourceDirectory.GetFiles())
                SourceFiles.Add(file);
        }

        private async void OnSelectedSourceFileChanged()
        {
            if (SelectedSourceFile == null ||
                !SelectedSourceFile.Exists)
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
                try
                {
                    using (var images = new MagickImageCollection())
                    {
                        images.Read(SelectedSourceFile, settings);

                        var image = images.First();
                        image.Format = MagickFormat.Jpeg;
                        images.Write(filename);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Unable to preview document.", ex);
                    PreviewImage = null;
                    PreviewImageFilename = null;
                    SelectedFilePageCount = 0;
                }

                try
                {
                    var pdfReader = new PdfReader(SelectedSourceFile.FullName);
                    SelectedFilePageCount = pdfReader.NumberOfPages;
                }
                catch (Exception ex)
                {
                    Log.Warn("Unable to count pages.", ex);
                    SelectedFilePageCount = 0;
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