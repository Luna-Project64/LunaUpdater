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

        private async void buttonUpdate_Click(object sender, EventArgs e)
        {
            if (tempFilePath_ == null)
            {
                buttonUpdate.Enabled = false;
                buttonIgnore.Enabled = false;
                labelUpdate.Text = $"Downloading an update '{release_.TagName}'...";
                tempFilePath_ = await updater_.DownloadLatestRelease(release_);
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
            const string rootFolderName = "Project64 3.0";
            const string emulatorExecuteable = "Project64.exe";

            //Extract Root of Zip-File into the App's Directory
            ZipFile.ExtractToDirectory(tempFilePath_, AppDomain.CurrentDomain.BaseDirectory);

            //Get Current Directory
            string currentDirectory = Directory.GetCurrentDirectory();

            //Find the Directory of the extracted folder in which the emulator executeable lies
            string[] emulator = Directory.GetFiles(Path.Combine(currentDirectory, rootFolderName), emulatorExecuteable, SearchOption.AllDirectories);
            string directoryPath = Path.GetDirectoryName(emulator[0]);

            //Get All Files in the executeable directory
            string[] files = Directory.GetFiles(directoryPath, "*.*", SearchOption.TopDirectoryOnly);

            //Iterate through all the files
            foreach (string file in files)
            {
                //Get Filename
                string fileName = Path.GetFileName(file);

                //Build a Path where the file should be moved to
                string destinationFile = Path.Combine(currentDirectory, fileName);

                //Overwrite the File when moving
                if(File.Exists(destinationFile))
                {
                    File.Delete(destinationFile);
                }
                    File.Move(file, destinationFile);
            }

            //Get all the folders in the executeable directory
            string[] directories = Directory.GetDirectories(directoryPath, "*", SearchOption.TopDirectoryOnly);

            //Iterate through the list of directories and move it to the App's Directory
            foreach (string dir in directories)
            {
                string dirName = Path.GetFileName(dir);
                string destinationDir = Path.Combine(currentDirectory, dirName);

                if(!Directory.Exists(destinationDir))
                {
                    Directory.Move(dir, destinationDir);
                }
            }
            //Delete the extracted root folder
            Directory.Delete(Path.Combine(currentDirectory, rootFolderName), true);
        }
    }
}
