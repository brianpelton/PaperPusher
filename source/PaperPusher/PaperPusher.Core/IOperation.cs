using System;

namespace PaperPusher.Core
{
    /// <summary>
    ///     A command / operation that can be performed and undone.
    /// </summary>
    public interface IOperation
    {
        #region [ Properties ]

        string Description { get; }

        #endregion

        #region [ Methods ]

        void Do();
        void Undo();

        #endregion
    }
}