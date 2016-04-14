using System;
using System.IO;

namespace PaperPusher
{
    public class RenameAndMoveOperation : IOperation
    {
        #region [ Constructors ]

        public RenameAndMoveOperation(FileInfo originalFile, FileInfo newFile)
        {
            OriginalFile = originalFile;
            NewFile = newFile;
        }

        #endregion

        #region [ Properties ]

        public string Description => $"Rename '{OriginalFile.Name}' to '{NewFile.Name}'";
        public FileInfo NewFile { get; }
        public FileInfo OriginalFile { get; }

        #endregion

        #region [ Interface IOperation Members ]

        public void Do()
        {
            File.Move(OriginalFile.FullName, NewFile.FullName);
        }

        public void Undo()
        {
            File.Move(NewFile.FullName, OriginalFile.FullName);
        }

        #endregion
    }
}