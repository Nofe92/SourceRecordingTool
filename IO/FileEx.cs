using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRecordingTool
{
    public static class FileEx
    {
        public static bool Exists(string path)
        {
            int attrib = Win32.GetFileAttributes(path);

            return attrib != -1 && (attrib & 16) != 16;
        }

        public static bool Copy(string src, string dst)
        {
            return Win32.CopyFile(src, dst, false);
        }

        public static bool Move(string src, string dst)
        {
            return Win32.MoveFile(src, dst);
        }

        public static bool Delete(string path)
        {
            return Win32.DeleteFile(path);
        }
    }
}
