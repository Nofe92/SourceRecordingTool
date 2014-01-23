using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRecordingTool
{
    public static class DirectoryEx
    {
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        public static bool Exists(string path)
        {
            int attrib = Win32.GetFileAttributes(path);

            return attrib != -1 && (attrib & 16) == 16;
        }

        public static void Create(string path)
        {
            string parent = PathEx.GetParentDirectory(path);

            if (!DirectoryEx.Exists(parent))
                DirectoryEx.Create(parent);

            if (!DirectoryEx.Exists(path))
                Win32.CreateDirectory(path, IntPtr.Zero);
        }

        public static void Copy(string src, string dst)
        {
            DirectoryEx.Create(dst);
            Win32.WIN32_FIND_DATA lpFindFileData;
            IntPtr hFindFile = Win32.FindFirstFile(src + "\\*", out lpFindFileData);

            if (hFindFile == INVALID_HANDLE_VALUE)
                return;

            do
            {
                if (lpFindFileData.cFileName.Equals(".") || lpFindFileData.cFileName.Equals(".."))
                    continue;

                if ((lpFindFileData.dwFileAttributes & 16) == 16)
                    Copy(src + "\\" + lpFindFileData.cFileName, dst + "\\" + lpFindFileData.cFileName);
                else
                    Win32.CopyFile(src + "\\" + lpFindFileData.cFileName, dst + "\\" + lpFindFileData.cFileName, false);

            } while (Win32.FindNextFile(hFindFile, out lpFindFileData));

            Win32.FindClose(hFindFile);
        }

        public static void Move(string src, string dst)
        {
            Win32.MoveFile(src, dst);
        }

        public static void Delete(string path)
        {
            if (!DirectoryEx.Exists(path))
                return;

            Win32.WIN32_FIND_DATA lpFindFileData;
            IntPtr hFindFile = Win32.FindFirstFile(path + "\\*", out lpFindFileData);

            do
            {
                if (lpFindFileData.cFileName.Equals(".") || lpFindFileData.cFileName.Equals(".."))
                    continue;

                if ((lpFindFileData.dwFileAttributes & 16) == 16)
                    Delete(path + "\\" + lpFindFileData.cFileName);
                else
                    Win32.DeleteFile(path + "\\" + lpFindFileData.cFileName);

            } while (Win32.FindNextFile(hFindFile, out lpFindFileData));

            Win32.FindClose(hFindFile);
            Win32.RemoveDirectory(path);
        }
    }
}
