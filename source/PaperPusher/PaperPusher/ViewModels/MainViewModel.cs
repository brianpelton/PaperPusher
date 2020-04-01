﻿using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using Microsoft.WindowsAPICodePack.Dialogs;
using PaperPusher.Core;
using PaperPusher.Core.Operations;
using ILog = log4net.ILog;
using LogManager = log4net.LogManager;
using iTextSharp.text;
using iTextSharp.text.pdf;
using PaperPusher.Core.PdfRendering;

namespace PaperPusher.ViewModels
{
    [Export(typeof(MainViewModel))]
    public class MainViewModel : Screen
    {
        #region [ Logging ]

        private static readonly ILog Log = LogManager.GetLogger(typeof(MainViewModel));

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


        public bool CanResetSplit => CurrentPageNumber != 0;
        public bool CanRedoLastUndo { get { return OperationsStack.RedoOperations.Any(); } }
        public bool CanSplitToCurrentPage => CurrentPageNumber != 0;
        public bool CanSplitToPreviousPage => CurrentPageNumber > 1;
        public bool CanShowNextPage { get { return CurrentPageNumber < TotalPageCount; } }
        public bool CanShowPreviousPage { get { return CurrentPageNumber > 1; } }
        public bool CanUndoLastOperation { get { return OperationsStack.UndoOperations.Any(); } }
        public int CurrentPageNumber { get; set; }
        public DateTime? DocumentDate { get; set; }
        public string DocumentTitle { get; set; }
        public bool HasSearchableText { get; private set; }
        public bool MoveToNextPageOnSplit { get; set; } = true;
        public string PageCountDisplay => $"{CurrentPageNumber} / {TotalPageCount}";
        public BitmapSource PreviewImage { get; private set; }
        public string PreviewImageFilename { get; private set; }
        public int TotalPageCount { get; private set; }
        public FileInfo SelectedSourceFile { get; set; }
        public DirectoryInfo SelectedTargetDirectory { get; set; }
        public DirectoryInfo SourceDirectory { get; set; }
        private int? SplitStartPageNumber { get; set; }

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

            RefreshUndoCommands();
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

        public void ShowNextPage()
        {
            var page = CurrentPageNumber + 1;
            CurrentPageNumber = Math.Min(page, TotalPageCount);

            DrawPdfPreviewImage();
        }

        public void ShowPreviousPage()
        {
            var page = CurrentPageNumber - 1;
            CurrentPageNumber = Math.Max(page, 0);

            DrawPdfPreviewImage();
        }

        public void UndoLastOperation()
        {
            OperationsStack.Undo();
            OnSourceDirectoryChanged();

            RefreshUndoCommands();
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

            RefreshUndoCommands();
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

        public void ResetSplit()
        {
            SplitStartPageNumber = CurrentPageNumber;
        }

        public void SplitToCurrentPage()
        {
            SplitPages(SplitStartPageNumber.Value, CurrentPageNumber);
            SplitStartPageNumber = CurrentPageNumber + 1;

            if (MoveToNextPageOnSplit)
                ShowNextPage();
        }

        public void SplitToPreviousPage()
        {
            SplitPages(SplitStartPageNumber.Value, CurrentPageNumber - 1);
            SplitStartPageNumber = CurrentPageNumber;

            if (MoveToNextPageOnSplit)
                ShowNextPage();
        }

        private void SplitPages(int startPage, int endPage)
        {
            if (endPage < startPage)
            {
                Log.Warn("Split end is before split start.");
                return;
            }

            try
            {
                var documentFilename = Path.GetFileNameWithoutExtension(SelectedSourceFile.Name);
                var newFilename = Path.Combine(SelectedSourceFile.DirectoryName, $"{documentFilename} - {startPage} to {endPage}.pdf");

                // Intialize a new PdfReader instance with the contents of the source Pdf file:
                var reader = new PdfReader(SelectedSourceFile.FullName);

                // For simplicity, I am assuming all the pages share the same size
                // and rotation as the first page:
                var sourceDocument = new Document(reader.GetPageSizeWithRotation(CurrentPageNumber));

                // Initialize an instance of the PdfCopyClass with the source 
                // document and an output file stream:
                var pdfCopyProvider = new PdfCopy(sourceDocument,
                    new FileStream(newFilename, FileMode.Create));

                sourceDocument.Open();

                // Walk the specified range and add the page copies to the output file:
                for (int i = startPage; i <= endPage; i++)
                {
                    var importedPage = pdfCopyProvider.GetImportedPage(reader, i);
                    pdfCopyProvider.AddPage(importedPage);
                }
                sourceDocument.Close();
                reader.Close();

                SourceFiles.Add(new FileInfo(newFilename));
            }
            catch (Exception ex)
            {
                Log.Error("Error splitting PDF", ex);
            }
        }

        private async void DrawPdfPreviewImage()
        {
            PreviewImage = null;

            string filename = Path.GetTempFileName();
            await Task.Run(() =>
            {
                try
                {
                    IPdfRenderer previewRenderer = new IronPdfRenderer
                    {
                        Density = 150,
                        OutputFilename = filename,
                        PageNumber = CurrentPageNumber
                    };
                    previewRenderer.Render(SelectedSourceFile.FullName);
                }
                catch (Exception ex)
                {
                    Log.Warn("Unable to preview document.", ex);
                    PreviewImage = null;
                    PreviewImageFilename = null;
                    TotalPageCount = 0;
                }

                try
                {
                    using (var pdfReader = new PdfReader(SelectedSourceFile.FullName))
                    {
                        TotalPageCount = pdfReader.NumberOfPages;

                        var text = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(pdfReader, 1);
                        HasSearchableText = !string.IsNullOrEmpty(text);
                        NotifyOfPropertyChange(nameof(HasSearchableText));
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn("Unable to count pages.", ex);
                    TotalPageCount = 0;
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

        private void OnSelectedSourceFileChanged()
        {
            if (SelectedSourceFile == null ||
                !SelectedSourceFile.Exists)
                return;

            CurrentPageNumber = 1;
            SplitStartPageNumber = 1;

            DrawPdfPreviewImage();
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

        private void RefreshUndoCommands()
        {
            NotifyOfPropertyChange(nameof(CanUndoLastOperation));
            NotifyOfPropertyChange(nameof(CanRedoLastUndo));
        }

        #endregion
    }
}