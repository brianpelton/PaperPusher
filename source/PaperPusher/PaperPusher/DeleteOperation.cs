using System;
using System.IO;

namespace PaperPusher
{
    public class DeleteOperation : IOperation
    {
        #region [ Constructors ]

        public DeleteOperation(FileInfo originalFile)
        {
            OriginalFile = originalFile;
            var filename = Path.Combine(Settings.TrashBinPath, originalFile.Name);
            NewFile = new FileInfo(filename);
        }

        #endregion

        #region [ Properties ]

        public string Description => $"Delete '{OriginalFile.Name}'";
        public FileInfo NewFile { get; }
        public FileInfo OriginalFile { get; }

        #endregion

        #region [ Interface IOperation Members ]

        public void Do()
        {
            OriginalFile.MoveTo(NewFile.FullName);
        }

        public void Undo()
        {
            NewFile.MoveTo(OriginalFile.FullName);
        }

        #endregion
    }
}