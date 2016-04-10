using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PaperPusher.Utility
{
    public static class SendFileToRecycleBin
    {
        public static void RecycleFile(FileInfo file)
        {
            var shf = new SHFILEOPSTRUCT();
            shf.wFunc = FO_DELETE;
            shf.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;
            shf.pFrom = file.FullName;
            SHFileOperation(ref shf);
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
        private struct SHFILEOPSTRUCT
        {
            public IntPtr hwnd;
            [MarshalAs(UnmanagedType.U4)]
            public int wFunc;
            public string pFrom;
            public string pTo;
            public short fFlags;
            [MarshalAs(UnmanagedType.Bool)]
            public bool fAnyOperationsAborted;
            public IntPtr hNameMappings;
            public string lpszProgressTitle;
        }

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern int SHFileOperation(ref SHFILEOPSTRUCT FileOp);

        private const int FO_DELETE = 3;
        private const int FOF_ALLOWUNDO = 0x40;
        private const int FOF_NOCONFIRMATION = 0x10; // Don't prompt the user
    }


}
