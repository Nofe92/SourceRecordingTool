using System;
using System.Collections.Generic;
using System.IO;
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
    public enum UpdateState
    {
        Latest,
        Update,
        NoConnection,
    }

    public static class Updater
    {
        public static string[] Mirrors = new string[]
        {
            "http://hl3mukkel.url.ph/SourceRecordingTool",
            "http://hl3mukkel.bugs3.com/SourceRecordingTool",
        };
        public static string ChangelogLink;
        public static string DownloadLink;
        public static Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
        public static Version latestVersion;

        public static void CheckForUpdatesASync()
        {
            Thread updateThread = new Thread(() => CheckForUpdates());
            updateThread.Start();
        }

        public static UpdateState CheckForUpdates()
        {
            System.Collections.IEnumerator mirrorsEnum = Mirrors.GetEnumerator();

            while (mirrorsEnum.MoveNext())
            {
                try
                {
                    XDocument version = XDocument.Load((string)mirrorsEnum.Current + "/version.xml");
                    
                    latestVersion = Version.Parse(version.Root.Element("Version").Value);

                    if (version.Root.Elements("ChangelogEx").Any())
                        ChangelogLink = version.Root.Element("ChangelogEx").Value.Replace("%MIRROR%", (string)mirrorsEnum.Current);
                    else
                        ChangelogLink = version.Root.Element("Changelog").Value;

                    if (version.Root.Elements("DownloadLinkEx").Any())
                        DownloadLink = version.Root.Element("DownloadLinkEx").Value.Replace("%MIRROR%", (string)mirrorsEnum.Current);
                    else
                        DownloadLink = version.Root.Element("DownloadLink").Value;

                    if (currentVersion < latestVersion)
                    {
                        ShowUpdateForm();
                        return UpdateState.Update;
                    }

                    return UpdateState.Latest;
                }
                catch
                {
                    continue;
                }
            }

            Dialogs.Error("No Connection to update servers.");
            return UpdateState.NoConnection;
        }

        public static string GetChangelog()
        {
            if (String.IsNullOrEmpty(ChangelogLink) || latestVersion == null)
                throw new Exception("No Connection to update servers.");

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(ChangelogLink);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream responseStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(responseStream, Encoding.ASCII);

            string result = reader.ReadToEnd();

            reader.Close();
            response.Close();

            return result;
        }

        public static void ShowChangelogForm()
        {
            RichTextBoxForm form = new RichTextBoxForm("Changelog",
                GetChangelog(),
                true);

            form.ShowDialog();
            form.Dispose();
        }

        public static void ShowUpdateForm()
        {
            RichTextBoxForm form = new RichTextBoxForm("Version " + latestVersion.ToString() + " available!",
                "A new version is available! Would you like to download it?\r\n" +
                "\r\n" +
                "Changelog:\r\n" +
                GetChangelog(),
                false);

            if (form.ShowDialog() == DialogResult.OK)
                Shell.Open(DownloadLink);

            form.Dispose();
        }
    }
}