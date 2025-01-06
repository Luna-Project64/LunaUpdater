using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace LunaUpdater
{
    internal static class Program
    {
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            string name = args.Name.Split(new char[] { ',' })[0].Trim();
            if (name == "Octokit")
                return Assembly.Load(Binaries.Octokit);

            return null;
        }

        static Form MakeForm(string version)
        {
            var updater = new Updater();
            var releaseTask = updater.HasNewVersion(version);
            releaseTask.Wait();
            var release = releaseTask.Result;
            if (release == null)
            {
                return null;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            return new UpdaterForm(updater, release, version);
        }

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string tmpPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LunaU.exe.tmp");
                File.Delete(tmpPath);
            }
            catch (Exception)
            { }

            Process[] procList = Process.GetProcessesByName("LunaUpdater");
            if (procList.Length != 1)
            {
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            string version = args.Count() < 1 ? null : args[1];
            try
            {
                var form = MakeForm(version);
                if (form == null)
                {
                    return;
                }

                Application.Run(form);

            }
            catch (Exception)
            {
                MessageBox.Show("The Update could not be installed :(\nReason: The Update could not be downloaded successfully", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
