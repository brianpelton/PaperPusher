using System;
using System.IO;

namespace PaperPusher
{
    public class MoveOperation : IOperation
    {
        #region [ Constructors ]

        public MoveOperation(FileInfo originalFile, FileInfo newFile)
        {
            OriginalFile = originalFile;
            NewFile = newFile;
        }

        #endregion

        #region [ Properties ]

        public string Description => $"Move '{OriginalFile.Name}' to '{NewFile.Name}'";
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