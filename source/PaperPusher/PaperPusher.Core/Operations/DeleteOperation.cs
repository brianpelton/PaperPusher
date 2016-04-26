using System;
using System.IO;

namespace PaperPusher.Core.Operations
{
    /// <summary>
    /// Delete the given file.
    /// PaperPusher does not actually delete the file from disk, but moves
    /// it to a trash folder in the TartgetRootFolder.
    /// </summary>
    public class DeleteOperation : IOperation
    {
        #region [ Constructors ]

        public DeleteOperation(FileInfo originalFile)
        {
            OriginalFile = originalFile;
            var filename = Path.Combine(Settings.TrashFolderPath, originalFile.Name);
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