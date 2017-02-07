using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;

namespace TimeTag
{
    public class HelperSetting
    {
        /// <summary>
        /// Clear collection and write all info in
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userInfo"></param>
        public static void WriteUserInfo(StringCollection collection, outz_User userInfo)
        {
            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    collection.Clear();

                    collection.Add(string.Format("lto:{0}", userInfo.Lto));
                    collection.Add(string.Format("mid:{0}", userInfo.Mid));
                    collection.Add(string.Format("pa:{0}", userInfo.Pa));
                    collection.Add(string.Format("db:{0}", userInfo.IsNewDb ? "new" : ""));

                    Properties.Settings.Default.Save();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add message to collection
        /// </summary>
        /// <param name="message"></param>
        /// <param name="collection"></param>
        /// <param name="append">clear collection if append is false</param>
        public static void WriteWithTime(string message, StringCollection collection, bool append)
        {
            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    if (!append)
                        collection.Clear();

                    collection.Add(string.Format("{0} @ {1}", message, DateTime.Now.ToString()));

                    Properties.Settings.Default.Save();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Add message to collection
        /// </summary>
        /// <param name="message"></param>
        /// <param name="collection"></param>
        /// <param name="append">clear collection if append is false</param>
        public static void Write(string message, StringCollection collection, bool append)
        {
            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    if (!append)
                        collection.Clear();

                    collection.Add(message);

                    Properties.Settings.Default.Save();
                }
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Read collection items
        /// </summary>
        /// <param name="collection">collection to read</param>
        /// <param name="lineConunt">max lines to read</param>
        /// <returns></returns>
        public static string[] ReadLines(StringCollection collection, int lineConunt)
        {
            string[] lines = new string[lineConunt];

            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    if (collection != null && collection.Count > 0)
                    {
                        int i = 0;

                        foreach (string line in collection)
                        {
                            lines[i] = line;
                            i++;
                            if (i == lineConunt)
                                break;
                        }
                    }
                }
            }
            catch
            {
                throw;
            }

            return lines;
        }
            
    }
}
