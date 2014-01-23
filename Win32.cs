using System;
using System.Runtime.InteropServices;

namespace SourceRecordingTool
{
    internal static class Win32
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttribute, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        internal static extern bool WriteFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToWrite, out uint lpNumberOfBytesWritten, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool ReadFile(IntPtr hFile, byte[] lpBuffer, uint nNumberOfBytesToRead, out uint lpNumberOfBytesRead, IntPtr lpOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern uint GetFileSize(IntPtr hFile, IntPtr lpFileSizeHigh);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern int GetFileAttributes(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CopyFile(string src, string dst, bool failIfExists);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool MoveFile(string src, string dst);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool DeleteFile(string path);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool CreateDirectory(string path, IntPtr lpSecurityAttributes);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool RemoveDirectory(string lpPathName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool FindClose(IntPtr hFindFile);

        [DllImport("uxtheme.dll", CharSet = CharSet.Unicode)]
        internal static extern int SetWindowTheme(IntPtr hwnd, string pszSubAppName, string pszSubIdList);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct WIN32_FIND_DATA
        {
            internal int dwFileAttributes;
            internal uint ftCreationTime_dwLowDateTime;
            internal uint ftCreationTime_dwHighDateTime;
            internal uint ftLastAccessTime_dwLowDateTime;
            internal uint ftLastAccessTime_dwHighDateTime;
            internal uint ftLastWriteTime_dwLowDateTime;
            internal uint ftLastWriteTime_dwHighDateTime;
            internal int nFileSizeHigh;
            internal int nFileSizeLow;
            internal int dwReserved0;
            internal int dwReserved1;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            internal string cFileName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
            internal string cAlternateFileName;
        }
    }
}
