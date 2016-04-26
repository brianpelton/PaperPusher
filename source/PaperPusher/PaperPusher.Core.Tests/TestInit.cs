using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PaperPusher.Core.Tests
{
    [TestClass]
    public class TestInit
    {
        #region [ Methods ]

        [AssemblyInitialize]
        public static void Initialize(TestContext context)
        {
            Settings.TrashFolderPath = "c:\\temp\\Trash";
            Settings.RenamePattern = "{0:MM.dd.yyyy} - {1}{2}";
        }

        #endregion
    }
}