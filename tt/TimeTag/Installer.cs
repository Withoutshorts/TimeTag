using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;

namespace TimeTag
{
    [RunInstaller(true)]
    public partial class Installer : System.Configuration.Install.Installer
    {
        public Installer()
        {
            InitializeComponent();
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            Process application = null;
            foreach (var process in Process.GetProcesses())
            {
                if (!process.ProcessName.ToLower().Contains("TimeTag")) continue;
                application = process;
                break;
            }

            if (application != null && application.Responding)
            {
                application.Kill();
                base.Uninstall(savedState);
            }

            #region delete user file and folder

            try
            {
                string ProgramFiles = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
                string path = Path.Combine(ProgramFiles, "TimeTag");

                if(outz_Log.tw != null)
                {
                    outz_Log.tw.Close();
                    outz_Log.tw.Dispose();
                }                

                System.GC.Collect();
                System.GC.WaitForPendingFinalizers();

                DirectoryInfo dir = new DirectoryInfo(path);
                HelperSetting.SaveLog(path, HelperSetting.StatusLogPath);

                foreach (FileInfo file in dir.GetFiles())
                {
                    try
                    {
                        file.Delete();
                    }
                    catch { }                    
                }

                Directory.Delete(path);
            }
            catch { }            

            #endregion
        }
    }
}
