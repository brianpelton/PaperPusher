using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaperPusher.Core.Operations;
using Shouldly;

namespace PaperPusher.Core.Tests.Operations
{
    [TestClass]
    public class MoveOperationTests
    {
        #region [ Methods ]

        [TestMethod]
        public void Can_Do_Move_Operation()
        {
            // arrange
            var file = Path.GetTempFileName();
            var newFilename = Path.GetTempPath() + Guid.NewGuid() + ".tmp";
            var operation = new MoveOperation(
                new FileInfo(file), new FileInfo(newFilename));

            // assume
            File.Exists(file).ShouldBe(true);
            File.Exists(newFilename).ShouldBe(false);

            // act
            operation.Do();

            // assert
            File.Exists(file).ShouldBe(false);
            File.Exists(newFilename).ShouldBe(true);
        }

        [TestMethod]
        public void Can_Undo_Move_Operation()
        {
            // arrange
            var file = Path.GetTempFileName();
            var newFilename = Path.GetTempPath() + Guid.NewGuid() + ".tmp";
            var operation = new MoveOperation(
                new FileInfo(file), new FileInfo(newFilename));
            operation.Do();

            // assume
            File.Exists(file).ShouldBe(false);
            File.Exists(newFilename).ShouldBe(true);

            // act
            operation.Undo();

            // assert
            File.Exists(file).ShouldBe(true);
            File.Exists(newFilename).ShouldBe(false);
        }

        #endregion
    }
}