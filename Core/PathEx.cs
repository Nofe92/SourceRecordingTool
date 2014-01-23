using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceRecordingTool
{
    public static class PathEx
    {
        public static string GetParentDirectory(string path)
        {
            int lastIndex = path.LastIndexOf('\\');

            if (lastIndex == -1)
                return "";

            return path.Substring(0, lastIndex);
        }

        public static string GetFileExtension(string path)
        {
            int lastIndex = path.LastIndexOf('.');

            if (lastIndex == -1)
                return "";

            return path.Substring(lastIndex);
        }

        public static string GetFileName(string path)
        {
            int lastIndex = path.LastIndexOf('\\');

            if (lastIndex == -1)
                return "";

            return path.Substring(lastIndex + 1);
        }
    }
}
