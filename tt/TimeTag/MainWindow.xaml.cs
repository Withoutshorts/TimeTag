using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Deployment.Application;
using Microsoft.Win32;
using System.Collections.Specialized;
using System.Web;
using TimeTag.Helper;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using TimeTag.Properties;

namespace TimeTag
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string INFO_SUBMIT = "Les heures sont envoyées au rapport hebdomadaire.";
        const string INFO_PROCESS_EXIST = "TimeTag déjà activé.";
        
        public MainWindow()
        {
            SelectCulture(Settings.Default.Culture);
            InitializeComponent();

            switch (ConfigHelper.TimeInputMode)
            {
                case Entity.TimeInputMode.Stopwatch:
                    Width = 700;
                    Height = 297;
                    break;
                case Entity.TimeInputMode.ManualInput:
                    Width = 467;
                    Height = 320;
                    break;
            }

            //Get url parameters and save to application setting
            this.Loaded += new RoutedEventHandler(MainWindow_Loaded);

            menuTimeOffline.Header = FindResource("Timeofflineheader").ToString();
            menuLogStatus.Header = FindResource("Upstatusheader").ToString();
            menuUserInfo.Header = FindResource("Userinfoheader").ToString();
            menuLogError.Header = FindResource("Errorlogfoheader").ToString();
            menuAbout.Header = FindResource("Aboutheader").ToString();
            menuDataSet.Header = FindResource("Datasetheader").ToString();
            menuExit.Header = FindResource("Exitheader").ToString();
        }

        #region private events

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Properties.Settings.Default.UserInfo != null && (Properties.Settings.Default.UserInfo.Count < 2 || string.IsNullOrWhiteSpace(Properties.Settings.Default.UserInfo[1].Split(new[] { ':' })[1])))
                {
                    UserInfo userInfo = new UserInfo();
                    //userInfo.Owner = Window.GetWindow(this);
                    userInfo.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                    userInfo.WindowState = WindowState.Normal;
                    userInfo.ResizeMode = ResizeMode.NoResize;
                    this.IsEnabled = false;
                    userInfo.Topmost = true;
                    this.Topmost = false;
                    userInfo.Activate();

                    userInfo.ShowDialog();
                    this.IsEnabled = true;
                    this.Topmost = true;
                }
                try
                {
                    MyInit();
                }
                catch (Exception ex)
                {
                    outz_Log.LogError("Init issue: " + ex.Message);
                }
                outz_User user = new outz_User(GetQueryStringParameters());
                user.SaveToFile();
                ShowLayout();
            }
            catch (Exception ex)
            {
                outz_Log.LogError(ex.Message);
            }
        }

        private void menuAbout_Click(object sender, RoutedEventArgs e)
        {
            AboutBox box = new AboutBox();
            box.TopMost = true;
            box.Show();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    MyInit();
            //}
            //catch (Exception ex)
            //{
            //    outz_Log.LogError("Init issue: " + ex.Message);
            //}
        }

        //private void btnGotoUgeSeddel_Click(object sender, RoutedEventArgs e)
        //{

        //    //public DateTime Date { get; }
        //    var idag = DateTime.Now.ToString("yyyy/MM/dd");

        //    try
        //    {
        //        HelperProcess.StartIE("https://outzource.dk/timeout_xp/wwwroot/ver2_14/timereg/ugeseddel_2011.asp?usemrn=1&varTjDatoUS_man=" + idag);  //+ "&varTjDatoUS_son=2014/1/19");
        //    }
        //    catch (Exception ex)
        //    {
        //        outz_Log.LogError(ex.Message);
        //    }
        //}
        #endregion

        #region Private methods

        private NameValueCollection GetQueryStringParameters()
        {
            NameValueCollection nameValueTable = new NameValueCollection();

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Uri activateUri = ApplicationDeployment.CurrentDeployment.ActivationUri;
                if (activateUri != null)
                {
                    string queryString = activateUri.Query;
                    nameValueTable = HttpUtility.ParseQueryString(queryString);
                }
                else
                    outz_Log.LogStatus("ActivationUri is null. My crystal ball tells me you are running on chrome!");
            }

            return (nameValueTable);
        }

        private void MyInit()
        {
            //Show window at right bottom of the window
            ShowRightBottom();

            //hide on start
            myHide();

            //Only start one process
            CheckProcess();

            //Set to true in production to add app in windows start up
            startup(true);

            //Empty offline time records if any, upload them to server
            //EmptyOfflineTime();
            Title = string.Format("Outzource - TimeTag S {0}", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
        }

        private void ShowRightBottom()
        {
            var desktopWorkingArea = System.Windows.SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Bottom - this.Height;
        }

        /// <summary>
        /// Do not start this MxDownloader twice
        /// </summary>
        private void CheckProcess()
        {
            try
            {
                if (HelperProcess.CheckOpen())
                {
                    if (System.Windows.MessageBox.Show(INFO_PROCESS_EXIST, "Astuce", MessageBoxButton.OK) == MessageBoxResult.OK)
                        System.Windows.Application.Current.Shutdown();
                }
            }
            catch (Exception ex)
            {
                outz_Log.LogError(ex.Message);
            }
        }

        #region Windows start up
        private void AddDelete(bool add, RegistryKey key, string name)
        {
            if (add)
            {
                key.SetValue(name, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
            else
            {
                key.DeleteValue(name, false);
            }
        }

        private void AddKey(bool add, RegistryKey key, string name)
        {
            if (add)
            {
                key.SetValue(name, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        private void DeleteAdd(bool add, RegistryKey key, string name)
        {
            key.DeleteValue(name, false);
            if (add)
            {
                key.SetValue(name, System.Reflection.Assembly.GetExecutingAssembly().Location);
            }
        }

        private void startup(bool add)
        {
            try
            {
                // isinstartup = add;
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    string name = "TimeTag";

                    if (key == null)
                    {
                        AddKey(add, key, name);
                    }
                    else
                    {
                        string[] names = key.GetValueNames();
                        if (!names.Contains(name))
                        {
                            AddDelete(add, key, name);
                        }
                        else
                        {
                            DeleteAdd(add, key, name);
                        }
                    }

                    key.Close();
                }
            }
            catch (Exception ex)
            {
                outz_Log.LogError("Ajout de TimeTag au démarrage de Windows échoué: " + ex.Message);
            }
        }
        #endregion // Windows start up
        #endregion // Private methods

        #region Service show and hide

        private void myHide()
        {
            this.WindowState = System.Windows.WindowState.Minimized;
            Hide();
        }

        private void notificationAreaIcon1_MouseClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Show();
                this.WindowState = System.Windows.WindowState.Normal;
            }
            else if (e.ChangedButton == MouseButton.Right)
            {
                var menu = (System.Windows.Controls.ContextMenu)this.notificationAreaIcon1.ContextMenu;
                menu.IsOpen = true;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            myHide();
            e.Cancel = true;
        }
        #endregion              

        #region Debug purpose
        private void ShowWindow(object win)
        {
            ((Window)win).Topmost = true;
            ((Window)win).Show();
        }

        private void menuTimeOffline_Click(object sender, RoutedEventArgs e)
        {
            TimeOffline win = new TimeOffline();
            ShowWindow(win);
        }


        private void menuLogStatus_Click(object sender, RoutedEventArgs e)
        {
            StatusLog win = new StatusLog();
            ShowWindow(win);
        }

        private void menuUserInfo_Click(object sender, RoutedEventArgs e)
        {
            UserInfo win = new UserInfo();
            win.ResizeMode = ResizeMode.NoResize;
            ShowWindow(win);
        }

        private void menuLogError_Click(object sender, RoutedEventArgs e)
        {
            ErrorLog win = new ErrorLog();
            ShowWindow(win);
        }

        private void menuDataSet_Click(object sender, RoutedEventArgs e)
        {
            DsSent win = new DsSent();
            ShowWindow(win);
            try
            {
                if (timeReportStopWatch.dsTimeTag.Tables.Count > 0)
                    win.dsTimeTag.DataContext = timeReportStopWatch.dsTimeTag.Tables[0];
            }
            catch (Exception ex)
            {
                outz_Log.LogError("Erreurs d'accès aux données: " + ex.Message);
            }
        }

        private void ShowLayout()
        {
            switch (ConfigHelper.TimeInputMode)
            {
                case Entity.TimeInputMode.Stopwatch:
                    timeReportStopWatch.Visibility = Visibility.Visible;
                    timeReportStopWatch.InitTimer();
                    timeReportManual.Visibility = Visibility.Hidden;
                    Width = 700;
                    Height = 297;
                    break;
                case Entity.TimeInputMode.ManualInput:
                    timeReportStopWatch.Visibility = Visibility.Hidden;
                    timeReportManual.Visibility = Visibility.Visible;
                    timeReportManual.InitTimer();
                    Width = 467;
                    Height = 320;
                    break;
            }
        }
        #endregion

        #region Static methods  

        public static void SelectCulture(string culture)
        {
            List<ResourceDictionary> source = new List<ResourceDictionary>();
            foreach (ResourceDictionary mergedDictionary in System.Windows.Application.Current.Resources.MergedDictionaries)
                source.Add(mergedDictionary);
            string requestedCulture = string.Format("Resources\\StringResources.{0}.xaml", (object)culture);
            ResourceDictionary resourceDictionary = source.FirstOrDefault<ResourceDictionary>((Func<ResourceDictionary, bool>)(d => d.Source.OriginalString == requestedCulture));
            if (resourceDictionary != null)
            {
                System.Windows.Application.Current.Resources.MergedDictionaries.Remove(resourceDictionary);
                System.Windows.Application.Current.Resources.MergedDictionaries.Add(resourceDictionary);
            }
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(culture);
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(culture);
        }

        #endregion // Static methods
    }
}
