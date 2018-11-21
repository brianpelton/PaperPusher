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
using PaperPusher.Core;
using PaperPusher.Core.Operations;
using PropertyChanged;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;

namespace PaperPusher.ViewModels
{
    [Export(typeof (MainViewModel))]
    public class MainViewModel : Screen
    {
        #region [ Logging ]

        private static readonly ILog Log = LogManager.GetLogger(typeof (MainViewModel));

        #endregion

        #region [ Constructors ]

        public MainViewModel()
        {
            DisplayName = "PaperPusher";
            SourceDirectory = new DirectoryInfo(Settings.SourceDirectory);
            TargetRootDirectory = new DirectoryInfo(Settings.TargetRootDirectory);
        }

        #endregion

        #region [ Properties ]

        public bool CanDeleteDocument => SelectedSourceFile != null;
        public bool CanMoveDocument => SelectedSourceFile != null && SelectedTargetDirectory != null;

        public bool CanRenameAndMoveDocument
        {
            get
            {
                if (SelectedSourceFile == null)
                    return false;

                if (DocumentTitle == null)
                    return false;

                if (DocumentDate == null)
                    return false;

                return SelectedTargetDirectory != null;
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
                var action = new DeleteOperation(
                    SelectedSourceFile);
                OperationsStack.DoOperation(action);
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
                var filename = Path.Combine(SelectedTargetDirectory.FullName, SelectedSourceFile.Name);
                var newFile = new FileInfo(filename);
                var operation = new MoveOperation(SelectedSourceFile, newFile);
                OperationsStack.DoOperation(operation);

                AfterAction();
            }
            catch (Exception ex)
            {
                Log.Error("Error moving document.", ex);
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        public void RedoLastUndo()
        {
            OperationsStack.Redo();
            OnSourceDirectoryChanged();
        }

        public void RenameAndMoveDocument()
        {
            if (!DocumentDate.HasValue)
                return;

            try
            {
                //var extension = Path.GetExtension(SelectedSourceFile.Name);
                //var filename = $"{DocumentDate:yyyy.MM.dd} - {DocumentTitle}{extension}";
                //var fullFilename = Path.Combine(SelectedTargetDirectory.FullName, filename);
                //var newFile = new FileInfo(fullFilename);
                var operation = new RenameAndMoveOperation(
                    SelectedSourceFile,
                    SelectedTargetDirectory,
                    DocumentDate.Value,
                    DocumentTitle);
                OperationsStack.DoOperation(operation);

                AfterAction();
            }
            catch (Exception ex)
            {
                Log.Error("Error renameing / moving document.", ex);
                MessageBox.Show("Error:" + ex.Message);
            }
        }

        public void UndoLastOperation()
        {
            OperationsStack.Undo();
            OnSourceDirectoryChanged();
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);

            Settings.SourceDirectory = SourceDirectory.FullName;
            Settings.TargetRootDirectory = TargetRootDirectory.FullName;
            Settings.Save();
        }

        private void AfterAction()
        {
            var index = SourceFiles.IndexOf(SelectedSourceFile);
            SourceFiles.Remove(SelectedSourceFile);

            if (SourceFiles.Count == index)
                index = SourceFiles.Count - 1;

            if (SourceFiles.Count == 0)
                return;

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

        private async void OnSelectedSourceFileChanged()
        {
            if (SelectedSourceFile == null ||
                !SelectedSourceFile.Exists)
                return;

            var settings = new MagickReadSettings
            {
                Density = new Density(150, 150),
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
                    using (var pdfReader = new PdfReader(SelectedSourceFile.FullName))
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
            catch (Exception)
            {
                Log.Warn("Unable to preview selected document.");

                try
                {
                    var uri = new Uri("pack://application:,,,/PaperPusher;component/Art/unknown_icon_512.png");
                    var bitmap = new BitmapImage(uri);
                    PreviewImage = bitmap;
                }
                catch (Exception ex)
                {
                    Log.Error("Error showing unknown file icon.", ex);
                }
            }
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