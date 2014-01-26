using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace SourceRecordingTool
{
    public static class Updater
    {
        public static string[] Mirrors = new string[]
        {
            "http://hl3mukkel.url.ph/SourceRecordingTool",
        };

        public static Version LocalGUIVersion;
        public static Version LocalMoviefilesVersion;
        public static Version RemoteGUIVersion;
        public static Version RemoteMoviefilesVersion;
        private static XDocument version;
        private static string mirror;

        public static bool Update()
        {
            LocalGUIVersion = Assembly.GetExecutingAssembly().GetName().Version;
            if (File.Exists("moviefiles\\version.txt"))
                LocalMoviefilesVersion = new Version(File.ReadAllText("moviefiles\\version.txt"));
            else
                LocalMoviefilesVersion = new Version(0, 0, 0, 0);

            if (!DownloadVersion())
                return true;
            try
            {
                RemoteMoviefilesVersion = new Version(version.Root.Element("moviefiles").Element("Version").Value);
                if (LocalMoviefilesVersion < RemoteMoviefilesVersion)
                {
                    DownloadFileForm.Start(version.Root.Element("moviefiles").Element("Download").Value.Replace("%MIRROR%", mirror), "moviefiles.zip");

                    if (Directory.Exists("moviefiles"))
                        Directory.Move("moviefiles", "moviefiles_" + LocalMoviefilesVersion.ToString());

                    ZipFile.ExtractToDirectory("moviefiles.zip", "moviefiles");
                    File.Delete("moviefiles.zip");
                }

                string executablePath = Application.ExecutablePath;
                string downloadPath = executablePath + ".download";
                string oldPath = executablePath + ".old";

                if (File.Exists(oldPath))
                    File.Delete(oldPath);

                RemoteGUIVersion = new Version(version.Root.Element("GUI").Element("Version").Value);
                if (LocalGUIVersion < RemoteGUIVersion)
                {
                    DownloadFileForm.Start(version.Root.Element("GUI").Element("Download").Value.Replace("%MIRROR%", mirror), downloadPath);

                    File.Move(executablePath, oldPath);
                    File.Move(downloadPath, executablePath);

                    Shell.Open(executablePath);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Dialogs.Error(ex.Message);
            }

            return true;
        }

        public static bool DownloadVersion()
        {
            System.Collections.IEnumerator mirrorsEnum = Mirrors.GetEnumerator();

            while (mirrorsEnum.MoveNext())
            {
                try
                {
                    mirror = (string)mirrorsEnum.Current;
                    version = XDocument.Load(mirror + "/version2.xml");
                    return true;
                }
                catch
                {
                    continue;
                }
            }

            Dialogs.Error("No Connection to update servers.");
            return false;
        }
    }
}