using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace Scp3999Launcher
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            byte[] htmlBytes = LoadEmbeddedHtml();
            if (htmlBytes == null || htmlBytes.Length == 0)
            {
                MessageBox.Show("无法加载内置文档资源。", "SCP-3999", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string appDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "SCP3999_SCiPNET_Terminal"
            );
            string userData = Path.Combine(appDir, "UserData");
            Directory.CreateDirectory(appDir);
            Directory.CreateDirectory(userData);

            string htmlPath = Path.Combine(appDir, "scp_3999.html");
            File.WriteAllBytes(htmlPath, htmlBytes);

            string fileUrl = new Uri(htmlPath).AbsoluteUri;
            string browserPath = FindAppBrowser();

            if (browserPath != null)
            {
                var psi = new ProcessStartInfo
                {
                    FileName = browserPath,
                    Arguments = string.Format(
                        "--app=\"{0}\" --start-fullscreen --user-data-dir=\"{1}\" --no-first-run --no-default-browser-check",
                        fileUrl, userData
                    ),
                    UseShellExecute = false
                };
                Process.Start(psi);
            }
            else
            {
                Process.Start(new ProcessStartInfo { FileName = htmlPath, UseShellExecute = true });
            }
        }

        private static byte[] LoadEmbeddedHtml()
        {
            var asm = Assembly.GetExecutingAssembly();
            using (Stream s = asm.GetManifestResourceStream("scp_3999.html"))
            {
                if (s == null) return null;
                using (var ms = new MemoryStream())
                {
                    s.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }

        private static string FindAppBrowser()
        {
            string[] candidates =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\Edge\Application\msedge.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Google\Chrome\Application\chrome.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Google\Chrome\Application\chrome.exe")
            };

            foreach (string path in candidates)
            {
                if (File.Exists(path)) return path;
            }
            return null;
        }
    }
}
