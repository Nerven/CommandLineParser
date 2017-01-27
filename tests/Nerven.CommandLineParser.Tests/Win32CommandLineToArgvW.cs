using System;
using System.Runtime.InteropServices;

namespace Nerven.CommandLineParser.Tests
{
    public static class Win32CommandLineToArgvW
    {
        // http://www.pinvoke.net/default.aspx/shell32/CommandLineToArgvW.html
        public static string[] Split(string unsplitArgumentLine)
        {
            int _argsCount;
            var _argsPointer = CommandLineToArgvW(unsplitArgumentLine, out _argsCount);
            
            if (_argsPointer == IntPtr.Zero)
                throw new Exception("Failed to split line.");
            
            try
            {
                var _args = new string[_argsCount];
                for (var _i = 0; _i < _argsCount; _i++)
                {
                    _args[_i] = Marshal.PtrToStringUni(Marshal.ReadIntPtr(_argsPointer, _i * IntPtr.Size));
                }

                return _args;
            }
            finally
            {
                LocalFree(_argsPointer);
            }
        }

        [DllImport("shell32.dll", SetLastError = true)]
        private static extern IntPtr CommandLineToArgvW([MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LocalFree(IntPtr hMem);
    }
}
