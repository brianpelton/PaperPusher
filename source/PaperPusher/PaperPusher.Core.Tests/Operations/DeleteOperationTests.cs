using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaperPusher.Core.Operations;
using Shouldly;

namespace PaperPusher.Core.Tests.Operations
{
    [TestClass]
    public class DeleteOperationTests
    {
        #region [ Methods ]

        [TestMethod]
        public void Can_Do_Delete_Operation()
        {
            // arrange
            var file = Path.GetTempFileName();
            var operation = new DeleteOperation(new FileInfo(file));

            // assume
            File.Exists(file).ShouldBe(true);

            // act
            operation.Do();

            // assert
            File.Exists(file).ShouldBe(false);
        }

        [TestMethod]
        public void Can_Undo_Delete_Operation()
        {
            // arrange
            var file = Path.GetTempFileName();
            var operation = new DeleteOperation(new FileInfo(file));

            // assume
            operation.Do();
            File.Exists(file).ShouldBe(false);

            // act
            operation.Undo();

            // assert
            File.Exists(file).ShouldBe(true);
        }

        #endregion
    }
}