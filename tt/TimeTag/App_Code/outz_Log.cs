using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace TimeTag
{
    public class outz_Log
    {
        private static readonly TextWriter tw = TextWriter.Synchronized(File.AppendText(System.AppDomain.CurrentDomain.BaseDirectory + "Log.txt"));
        private static readonly object _syncObject = new object();
        public static void LogError(string message)
        {
            try
            {
                HelperSetting.WriteWithTime(message, Properties.Settings.Default.ErrorLog, true);
            }
            catch
            {               
            }
        }

        public static void LogStatus(string message)
        {
            try
            {
                HelperSetting.WriteWithTime(message, Properties.Settings.Default.StatusLog, true);
            }
            catch
            {
            }
        }


        public static void LogToFile(string logMessage)
        {
            try
            {
                Log(logMessage, tw);
            }
            catch (IOException e)
            {
                tw.Close();
            }
        }

        private static void Log(string logMessage, TextWriter w)
        {
            lock (_syncObject)
            {
                w.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                w.WriteLine("  :{0}", logMessage);
                w.WriteLine("-------------------------------");
                // Update the underlying file.
                w.Flush();
            }
        }
    }
}
