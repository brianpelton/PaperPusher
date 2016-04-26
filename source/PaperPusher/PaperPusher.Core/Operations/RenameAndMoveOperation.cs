using System;
using System.IO;

namespace PaperPusher.Core.Operations
{
    public class RenameAndMoveOperation : IOperation
    {
        #region [ Fields ]

        private readonly MoveOperation _operation;

        #endregion

        #region [ Constructors ]

        public RenameAndMoveOperation(FileInfo originalFile, DirectoryInfo targetDirectory, DateTime documentDate,
            string documentTitle)
        {
            OriginalFile = originalFile;
            TargetDirectory = targetDirectory;
            DocumentDate = documentDate;
            DocumentTitle = documentTitle;
            DocumentExtension = Path.GetExtension(OriginalFile.Name);

            NewFilename = string.Format(Settings.RenamePattern, DocumentDate, DocumentTitle, DocumentExtension);

            _operation = new MoveOperation(OriginalFile,
                new FileInfo(Path.Combine(TargetDirectory.FullName, NewFilename)));
        }

        #endregion

        #region [ Properties ]

        public string Description => $"Rename '{OriginalFile.Name}' to '{NewFilename}'";
        public DateTime DocumentDate { get; }
        public string DocumentExtension { get; }
        public string DocumentTitle { get; }
        public string NewFilename { get; }
        public FileInfo OriginalFile { get; }
        public DirectoryInfo TargetDirectory { get; }

        #endregion

        #region [ Interface IOperation Members ]

        public void Do()
        {
            _operation.Do();
        }

        public void Undo()
        {
            _operation.Undo();
        }

        #endregion
    }
}