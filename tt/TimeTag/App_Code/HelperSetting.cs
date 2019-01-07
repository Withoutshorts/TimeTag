using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections.Specialized;
using System.Xml;

namespace TimeTag
{
    public class HelperSetting
    {
        /// <summary>
        /// Clear collection and write all info in
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="userInfo"></param>
        ////public static void WriteUserInfo(StringCollection collection, outz_User userInfo)
        ////{
        ////    try
        ////    {
        ////        object _locker = new object();
        ////        lock (_locker)
        ////        {
        ////            collection.Clear();

        ////            collection.Add(string.Format("lto:{0}", userInfo.Lto));
        ////            collection.Add(string.Format("mid:{0}", userInfo.Mid));
        ////            collection.Add(string.Format("pa:{0}", userInfo.Pa));
        ////            collection.Add(string.Format("db:{0}", userInfo.IsNewDb ? "new" : ""));

        ////            Properties.Settings.Default.Save();
        ////        }
        ////    }
        ////    catch
        ////    {
        ////        throw;
        ////    }
        ////}

        /// <summary>
        /// Add message to collection
        /// </summary>
        /// <param name="message"></param>
        /// <param name="collection"></param>
        /// <param name="append">clear collection if append is false</param>
        ////public static void WriteWithTime(string message, StringCollection collection, bool append)
        ////{
        ////    try
        ////    {
        ////        object _locker = new object();
        ////        lock (_locker)
        ////        {
        ////            if (!append)
        ////                collection.Clear();

        ////            collection.Add(string.Format("{0} @ {1}", message, DateTime.Now.ToString()));

        ////            Properties.Settings.Default.Save();
        ////        }
        ////    }
        ////    catch
        ////    {
        ////        throw;
        ////    }
        ////}

        /// <summary>
        /// Add message to collection
        /// </summary>
        /// <param name="message"></param>
        /// <param name="collection"></param>
        /// <param name="append">clear collection if append is false</param>
        ////public static void Write(string message, StringCollection collection, bool append)
        ////{
        ////    try
        ////    {
        ////        object _locker = new object();
        ////        lock (_locker)
        ////        {
        ////            if (!append)
        ////                collection.Clear();

        ////            collection.Add(message);

        ////            Properties.Settings.Default.Save();
        ////        }
        ////    }
        ////    catch
        ////    {
        ////        throw;
        ////    }
        ////}

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

        #region Custom Variables

        public static readonly string ErrorLogPath = "//ErrorLog";
        public static readonly string TimeOfflinePath = "//TimeOffline";
        public static readonly string StatusLogPath = "//StatusLog";
        public static readonly string UserInfoPath = "//UserInfo";
        public static readonly string SettingFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TimeTag\\user.config");
        public static readonly string LogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TimeTag\\Log.txt");
        public static XmlDocument SettingsDoc = new XmlDocument();
        private static string[] _UserInfo;

        public static string[] UserInfo
        {
            get {
                return _UserInfo;
            }

            set {
                _UserInfo = value;
            }
        }

        #endregion

        #region Custom Methods

        public static string[] ReadSettings(string path, int lineConunt = 0)
        {
            string[] lines = new string[lineConunt];

            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    string nodePath = path + "/string";
                    int i = 0;
                    XmlNodeList nodeList = SettingsDoc.SelectNodes(nodePath);
                    if(nodeList != null && nodeList.Count > 0)
                    {
                        if(lineConunt == 0)
                        {
                            lines = new string[nodeList.Count];
                        }                        

                        foreach (XmlNode node in nodeList)
                        {
                            lines[i] = node.InnerText;
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

        public static void SaveUserInfo(string path, string lto, string mid, string pa, bool isNewDb)
        {
            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    XmlNode node = SettingsDoc.SelectSingleNode(path);
                    if (node != null)
                    {
                        node.RemoveAll();

                        XmlElement strLog = SettingsDoc.CreateElement("string");
                        XmlText text = SettingsDoc.CreateTextNode(string.Format("lto:{0}", lto));
                        strLog.AppendChild(text);
                        node.AppendChild(strLog);

                        strLog = SettingsDoc.CreateElement("string");
                        text = SettingsDoc.CreateTextNode(string.Format("mid:{0}", mid));
                        strLog.AppendChild(text);
                        node.AppendChild(strLog);

                        strLog = SettingsDoc.CreateElement("string");
                        text = SettingsDoc.CreateTextNode(string.Format("pa:{0}", pa));
                        strLog.AppendChild(text);
                        node.AppendChild(strLog);

                        strLog = SettingsDoc.CreateElement("string");
                        text = SettingsDoc.CreateTextNode(string.Format("db:{0}", isNewDb ? "new" : ""));
                        strLog.AppendChild(text);
                        node.AppendChild(strLog);

                        SettingsDoc.Save(SettingFilePath);
                        UserInfo = ReadSettings(UserInfoPath);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void SaveLog(string message, string path)
        {
            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    XmlNode node = SettingsDoc.SelectSingleNode(path);
                    if (node != null)
                    {
                        XmlElement strLog = SettingsDoc.CreateElement("string");
                        XmlText text = SettingsDoc.CreateTextNode(message);
                        strLog.AppendChild(text);
                        node.AppendChild(strLog);

                        SettingsDoc.Save(SettingFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void LoadSettings()
        {
            SettingsDoc = new XmlDocument();

            if (!File.Exists(SettingFilePath))
            {
                XmlDeclaration xmlDeclaration = SettingsDoc.CreateXmlDeclaration("1.0", "UTF-8", null);
                XmlElement root = SettingsDoc.DocumentElement;
                SettingsDoc.InsertBefore(xmlDeclaration, root);

                XmlElement timeTag = SettingsDoc.CreateElement("TimeTag");
                SettingsDoc.AppendChild(timeTag);

                XmlElement userInfo = SettingsDoc.CreateElement(UserInfoPath.Replace("//", string.Empty));
                timeTag.AppendChild(userInfo);

                XmlElement timeOffline = SettingsDoc.CreateElement(TimeOfflinePath.Replace("//", string.Empty));
                timeTag.AppendChild(timeOffline);

                XmlElement statusLog = SettingsDoc.CreateElement(StatusLogPath.Replace("//", string.Empty));
                timeTag.AppendChild(statusLog);

                XmlElement errorLog = SettingsDoc.CreateElement(ErrorLogPath.Replace("//", string.Empty));
                timeTag.AppendChild(errorLog);

                string SettingDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "TimeTag");
                if (!Directory.Exists(SettingDirectory))
                {
                    Directory.CreateDirectory(SettingDirectory);
                }

                SettingsDoc.Save(SettingFilePath);

                File.Create(LogFilePath);
            }

            SettingsDoc.Load(SettingFilePath);
            UserInfo = ReadSettings(UserInfoPath);
        }

        public static void ClearLog(string path)
        {
            try
            {
                object _locker = new object();
                lock (_locker)
                {
                    XmlNode node = SettingsDoc.SelectSingleNode(path);
                    if (node != null)
                    {
                        node.RemoveAll();
                        SettingsDoc.Save(SettingFilePath);
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        #endregion
    }
}
