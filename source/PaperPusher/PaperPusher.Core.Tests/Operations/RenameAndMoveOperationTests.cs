using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PaperPusher.Core.Operations;
using ParkSquare.Testing.Helpers;
using Shouldly;

namespace PaperPusher.Core.Tests.Operations
{
    [TestClass]
    public class RenameAndMoveOperationTests
    {
        #region [ Methods ]

        [TestMethod]
        public void Can_Do_RenameAndMove_Operation()
        {
            // arrange
            var file = Path.GetTempFileName();
            var newFilename = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".tmp");
            var operation = new RenameAndMoveOperation(
                new FileInfo(file),
                new DirectoryInfo(Path.GetTempFileName()),
                DateTimeGenerator.AnyDateBetween(
                    DateTimeGenerator.FirstDayOfThisMonth(),
                    DateTimeGenerator.LastDayOfThisMonth()),
                Guid.NewGuid().ToString());

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
        public void Can_Undo_RenameAndMove_Operation()
        {
            // arrange
            var file = Path.GetTempFileName();
            var newFilename = Path.GetTempPath() + Guid.NewGuid() + ".tmp";
            var operation = new RenameAndMoveOperation(
                new FileInfo(file),
                new DirectoryInfo(Path.GetTempFileName()),
                DateTimeGenerator.AnyDateBetween(
                    DateTimeGenerator.FirstDayOfThisMonth(),
                    DateTimeGenerator.LastDayOfThisMonth()),
                Guid.NewGuid().ToString());
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