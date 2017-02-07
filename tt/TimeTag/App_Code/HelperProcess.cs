using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace TimeTag
{
    public class HelperProcess
    {
        public static void StartIE(string url)
        {
            try
            {
                Process.Start("iexplore.exe", url);
            }
            catch(Exception ex)
            {
                throw new Exception("IE could not start: "+ex.Message);
            }
        }

        public static bool CheckOpen()
        {
            bool isOpen = false;

            Process[] myProcess = Process.GetProcesses();

            int count = 0;
            foreach (Process pro in myProcess)
            {
                if (pro.ProcessName == "TimeTag")
                {
                    count++;
                    if (count == 2)
                    {
                        isOpen = true;
                        break;
                    }
                }
            }

            return isOpen;
        }

        public static void ProcessCheck()
        {
            Process[] myProcess = Process.GetProcesses();

            foreach (Process pro in myProcess)
            {
                if (pro.ProcessName == "TimeTag")
                {
                    pro.CloseMainWindow();
                }
            }
        }
    }
}
