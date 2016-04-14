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
            TrashFile = new FileInfo(filename);
        }

        #endregion

        #region [ Properties ]

        public string Description => $"Delete '{OriginalFile.Name}'";
        public FileInfo TrashFile { get; }
        public FileInfo OriginalFile { get; }

        #endregion

        #region [ Interface IOperation Members ]

        public void Do()
        {
            File.Move(OriginalFile.FullName, TrashFile.FullName);
        }

        public void Undo()
        {
            File.Move(TrashFile.FullName, OriginalFile.FullName);
        }

        #endregion
    }
}