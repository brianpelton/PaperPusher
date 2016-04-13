using System;
using System.Collections.Generic;
using System.Linq;
using log4net;

namespace PaperPusher
{
    public static class Session
    {
        #region [ Logging ]

        private static readonly ILog Log = LogManager.GetLogger(typeof (Session));

        #endregion

        #region [ Properties ]

        public static IEnumerable<IOperation> RedoOperations => RedoStack.AsEnumerable();
        public static IEnumerable<IOperation> UndoOperations => UndoStack.AsEnumerable();

        private static Stack<IOperation> RedoStack { get; }
            = new Stack<IOperation>();

        private static Stack<IOperation> UndoStack { get; }
            = new Stack<IOperation>();

        #endregion

        #region [ Methods ]

        public static void DoOperation(IOperation operation)
        {
            try
            {
                operation.Do();
                Log.Info($"Operation '{operation.Description}' was done.");
            }
            catch (Exception ex)
            {
                Log.Error("Error running operation.", ex);
                throw;
            }

            UndoStack.Push(operation);
            RedoStack.Clear();

            DebugOutStacks();
        }

        private static void DebugOutStacks()
        {
            Log.Info("Redo Stack---------");
            foreach (var item in RedoStack)
                Log.Info(item.Description);

            Log.Info("Undo Stack---------");
            foreach (var item in UndoStack)
                Log.Info(item.Description);

        }

        public static void Undo()
        {
            if (UndoStack.Count == 0)
            {
                Log.Warn("Cannot undo, no operations available.");
                return;
            }

            var operation = UndoStack.Pop();
            try
            {
                operation.Undo();
                Log.Info($"Operation '{operation.Description}' was undone.");
            }
            catch (Exception ex)
            {
                Log.Error("Error undoing operation.", ex);
                throw;
            }
            RedoStack.Push(operation);
        }

        public static void Redo()
        {
            if (RedoStack.Count == 0)
            {
                Log.Warn("Cannot redo, no operations available.");
                return;
            }

            var operation = RedoStack.Pop();
            try
            {
                operation.Do();
                Log.Info($"Operation '{operation.Description}' was redone.");
            }
            catch (Exception ex)
            {
                Log.Error("Error redoing operation.", ex);
                throw;
            }
            UndoStack.Push(operation);
        }

        #endregion
    }
}