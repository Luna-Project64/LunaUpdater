using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Octokit;

namespace LunaUpdater
{
    public partial class UpdaterForm : Form
    {
        // Sets the window to be foreground
        [DllImport("User32")]
        private static extern int SetForegroundWindow(IntPtr hwnd);

        // Activate or minimize a window
        [DllImport("User32.DLL")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_RESTORE = 9;

        readonly Release release_;
        readonly Updater updater_;
        string tempFilePath_;

        public UpdaterForm(Updater updater, Release release)
        {
            InitializeComponent();

            release_ = release;
            updater_ = updater;
            labelUpdate.Text = "New version available: " + release_.TagName + "\nDo you want to update?";
            changelogLabel.Text = release_.Body;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (tempFilePath_ != null)
            {
                File.Delete(tempFilePath_);
            }
        }

        private void buttonIgnore_Click(object sender, EventArgs e)
        {
            Close();
        }

        static void BringSelfToForeGround()
        {
            Process[] procList = Process.GetProcessesByName("LunaUpdater");
            if (procList.Length == 0)
            {
                return;
            }

            ShowWindow(procList[0].MainWindowHandle, SW_RESTORE);
            SetForegroundWindow(procList[0].MainWindowHandle);
        }

        private void onProgress(object sender, float progress)
        {
            labelUpdate.Text = $"Downloading '{release_.TagName}'... {progress:P0}";
        }

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (tempFilePath_ == null)
            {
                buttonUpdate.Enabled = false;
                buttonIgnore.Enabled = false;
                labelUpdate.Text = $"Downloading an update '{release_.TagName}'...";
                tempFilePath_ = await updater_.DownloadLatestRelease(release_, onProgress);
                labelUpdate.Text = $"Downloaded '{release_.TagName}' successfully!\nDo you want to install?";
                buttonUpdate.Text = "Install";
                BringSelfToForeGround();
            
                labelUpdate.Text = $"Installing '{release_.TagName}'...";
                
                await Task.Run(() => { 
                    KillAllProject64s();
                    ExtractFiles();
                });
                MessageBox.Show("The update has been installed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
                Process.Start("Project64.exe");
            }
        }

        private void KillAllProject64s()
        {
            Process[] emuProcesses = Process.GetProcessesByName("Project64");
            if(emuProcesses.Length > 0)
            {
                foreach(Process emuProcess in emuProcesses)
                {
                    try
                    {
                        emuProcess.Kill();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        /*
         * TODO:
         * figure out a way to dynamicly get the name of the root folder
         * Exeception handling
         */
        private void ExtractFiles()
        {
            using (var strm = File.OpenRead(tempFilePath_))
            using (ZipArchive archive = new ZipArchive(strm))
            {
                foreach (ZipArchiveEntry file in archive.Entries)
                {
                    string completeFileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, file.FullName);
                    string directory = Path.GetDirectoryName(completeFileName);

                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);

                    if (file.Name != "")
                    {
                        try
                        {
                            if (file.Name == "LunaUpdater.exe")
                            {
                                File.Delete(completeFileName + ".tmp");
                                File.Move(completeFileName, completeFileName + ".tmp");
                                file.ExtractToFile(completeFileName, true);
                            }
                            else
                            {
                                file.ExtractToFile(completeFileName, true);
                            }
                        }
                        catch (Exception)
                        {
                        }
                    }
                }
            }

            File.Delete(tempFilePath_);
        }
    }
}
