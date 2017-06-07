using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using TimeTag.App_Code;
using TimeTag.Entity;
using TimeTag.Helper;

namespace TimeTag.Layout
{
    /// <summary>
    /// Interaction logic for TimeReportStopWatch.xaml
    /// </summary>
    public partial class TimeReportManual : UserControl
    {
        DispatcherTimer timerShort = new DispatcherTimer();
        InternetStatus internetStatus = InternetStatus.Notset;
        BackgroundWorker worker;
        public DataSet dsTimeTag = new DataSet();

        public List<outz_JobCustomer> lstJobCustomerNames
        {
            get
            {
                if (autoCustomerJob != null && autoCustomerJob.ItemsSource != null && autoCustomerJob.ItemsSource is List<outz_JobCustomer>)
                {
                    return (List<outz_JobCustomer>)autoCustomerJob.ItemsSource;
                }
                return new List<outz_JobCustomer>();
            }
        }

        public List<outz_Activity> lstActivities
        {
            get
            {
                if (autoActivity != null && autoActivity.ItemsSource != null && autoActivity.ItemsSource is List<outz_Activity>)
                {
                    return (List<outz_Activity>)autoActivity.ItemsSource;
                }
                return new List<outz_Activity>();
            }
        }

        public TimeReportManual()
        {
            InitializeComponent();

            //Check internet on/off line once
            SetInternetStatus();

            //Empty offline time records if any, upload them to server
            TimeReportHelper.EmptyOfflineTime((List<outz_JobCustomer>)autoCustomerJob.ItemsSource);
            if (ConfigHelper.ListActivities)
            {
                autoActivity.Visibility = Visibility.Hidden;
                autoActivityList.Visibility = Visibility.Visible;
            }
            else
            {
                autoActivity.Visibility = Visibility.Visible;
                autoActivityList.Visibility = Visibility.Hidden;
            }
        }

        public int? SelectedCustomerId
        {
            get
            {
                var selectedItem = autoCustomerJob.SelectedItem as outz_JobCustomer;
                if (selectedItem != null)
                {
                    return selectedItem.IsCustomer ? Convert.ToInt32(selectedItem.CustomerNo) : selectedItem.JobId;
                }
                return null;
            }
        }

        public int? SelectedActivityId
        {
            get
            {
                outz_Activity selectedItem = null;
                if (ConfigHelper.ListActivities)
                {
                    selectedItem = autoActivityList.SelectedValue as outz_Activity;
                }
                else
                {
                    selectedItem = autoActivity.SelectedItem as outz_Activity;
                }

                return selectedItem != null ? selectedItem.Id : (int?)null;
            }
        }

        #region Event handlers

        private void btnSubmit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var sw = new Stopwatch();
                string validationMessage;
                sw.Start();

                if (!ValidateSubmittedData(out validationMessage))
                {
                    ShowStatus(validationMessage);
                    outz_Log.LogError(validationMessage);

                    return;
                }

                sw.Stop();
                if (sw.ElapsedMilliseconds > 2000)
                {
                    outz_Log.LogToFile(string.Format("ValidateSubmittedData() has taken {0}ms", sw.ElapsedMilliseconds));
                }

                var jobName = string.Empty;
                if (autoCustomerJob.SelectedItem != null)
                {
                    jobName = ((outz_JobCustomer)autoCustomerJob.SelectedItem).GetName();
                }

                var activityName = ConfigHelper.ListActivities ? autoActivityList.Text : autoActivity.Text;

                outz_TimeTag tt = new outz_TimeTag(jobName, ((outz_JobCustomer)autoCustomerJob.SelectedItem).ToString(), activityName, ConvertHoursToHoursMinutes(float.Parse(txtHours.Text)), txtComments.Text, lstJobCustomerNames, lstActivities, string.Empty, string.Empty, selectedDate.SelectedDate.Value);
                tt.InitDataSet();
                dsTimeTag = tt.DsTimeTag;

                sw.Restart();
                string status = tt.UploadTime();
                sw.Stop();

                if (sw.ElapsedMilliseconds > 2000)
                {
                    outz_Log.LogToFile(string.Format("UploadTime() has taken {0}ms", sw.ElapsedMilliseconds));
                }
                if (status.Contains(outz_TimeTag.Success_Web_Service))
                {
                    txtSubmitStatus.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    ShowStatus(status);
                }

                var hoursReported = (autoActivityList.SelectedItem as outz_Activity).ReportedHours + Convert.ToDecimal(txtHours.Text.Trim());
                txtHoursReported.Text = hoursReported.ToString("N2");
                (autoActivityList.SelectedItem as outz_Activity).ReportedHours = hoursReported;

                ReSet();
                //Log time upload status
                outz_Log.LogStatus(status);
                sw.Restart();
                //UpdateReportedHours();                

                sw.Stop();
                if (sw.ElapsedMilliseconds > 2000)
                {
                    outz_Log.LogToFile(string.Format("UpdateReportedHours() has taken {0}ms", sw.ElapsedMilliseconds));
                }

                txtSubmitStatus.Foreground = Brushes.Green;
                txtSubmitStatus.Text = FindResource("Succeed").ToString();
                txtSubmitStatus.Visibility = System.Windows.Visibility.Visible;
                settimer();
            }
            catch (Exception ex)
            {
                //ShowStatus(ex.Message);

                outz_Log.LogError(ex.Message);
            }
        }

        private float ConvertHoursToHoursMinutes(float hours)
        {
            if (hours == 0)
            {
                return 0;
            }
            var integerPart = (float)Math.Truncate(hours);
            var decimalPart = hours - integerPart;
            return integerPart + (60 * decimalPart / 100);
        }

        private bool ValidateSubmittedData(out string message)
        {
            if (!selectedDate.SelectedDate.HasValue)
            {
                message = FindResource("DateRequired").ToString();
                return false;
            }

            if (string.IsNullOrEmpty(txtHours.Text))
            {
                message = FindResource("HoursRequired").ToString();
                return false;
            }

            if (TimeReportHelper.RequireAllFields())
            {
                if (autoCustomerJob.SelectedItem == null)
                {
                    message = FindResource("CustomerJobRequired").ToString();
                    return false;
                }

                if (autoActivityList.SelectedItem == null)
                {
                    message = FindResource("ActivityRequired").ToString();
                    return false;
                }

                if (UserInfoProvider.PA == "2" &&
                    !string.IsNullOrEmpty(UserInfoProvider.LTO) &&
                    "bf".Equals(UserInfoProvider.LTO, StringComparison.InvariantCultureIgnoreCase) &&
                    SelectedCustomerId.HasValue && SelectedActivityId.HasValue)
                {
                    var hoursReported = (autoActivityList.SelectedItem as outz_Activity).ReportedHours + Convert.ToDecimal(txtHours.Text.Trim());
                    var resourceHours = (autoActivityList.SelectedItem as outz_Activity).ResourceHours;

                    if (hoursReported > resourceHours)
                    {
                        message = FindResource("ResourcesExceeded2") + " " + (resourceHours - hoursReported) + " " + FindResource("ResourcesExceeded3").ToString();
                        return false;
                    }
                }
            }


            //if (TimeReportHelper.RequireAllFields())
            //{
            //    var activityName = ConfigHelper.ListActivities ? autoActivityList.Text : autoActivity.Text;
            //    if (string.IsNullOrEmpty(autoCustomerJob.Text))
            //    {
            //        message = FindResource("CustomerJobRequired").ToString();
            //        return false;
            //    }
            //    if (lstJobCustomerNames.All(cust => !cust.ToString().Equals(autoCustomerJob.Text, StringComparison.InvariantCultureIgnoreCase)))
            //    {
            //        message = FindResource("CustomerJobWrong").ToString();
            //        return false;
            //    }
            //    if (string.IsNullOrEmpty(activityName))
            //    {
            //        message = FindResource("ActivityRequired").ToString();
            //        return false;
            //    }
            //    if (lstActivities.All(act => !act.Name.ToString().Equals(activityName, StringComparison.InvariantCultureIgnoreCase)))
            //    {
            //        message = FindResource("ActivityWrong").ToString();
            //        return false;
            //    }
            //}

            //if (UserInfoProvider.PA == "2" &&
            //    !string.IsNullOrEmpty(UserInfoProvider.LTO) &&
            //    "bf".Equals(UserInfoProvider.LTO, StringComparison.InvariantCultureIgnoreCase) &&
            //    SelectedCustomerId.HasValue && SelectedActivityId.HasValue)
            //{
            //    var hoursReportedBefore = 0m;
            //    var resourceHours = 0m;

            //    bool isOnline = HelperInternet.IsOnline();
            //    if (isOnline) // LAVER KUN Ressourcetimer tjk IF online = true
            //    {

            //        //sqlstatus0.Text = "HEJ Der 8: resourceHours: " + resourceHours + " hoursReportedBefore: " + hoursReportedBefore + " isOnline: " + isOnline;


            //        try
            //        {
            //            var rdp = new ResourceDataProvider(UserInfoProvider.LTO, UserInfoProvider.IsNewDb);
            //            resourceHours = rdp.GetResourceHours(UserInfoProvider.MID, SelectedCustomerId.Value, SelectedActivityId.Value, selectedDate.SelectedDate.Value);

            //            HoursService hoursService = new HoursService(UserInfoProvider.LTO, UserInfoProvider.IsNewDb);
            //            hoursReportedBefore = hoursService.GetReportedHoursByActivity(UserInfoProvider.MID, SelectedActivityId.Value, selectedDate.SelectedDate.Value);

            //            var activities = outz_JobCustomer.GetActivities(autoCustomerJob.Text, lstJobCustomerNames);
            //            var activity = activities.FirstOrDefault(act => act.Id == SelectedActivityId.Value);
            //            if (activity != null)
            //            {

            //                activity.ResourceHours = resourceHours;
            //                activity.ReportedHours = hoursReportedBefore + decimal.Parse(txtHours.Text);

            //            }

            //            //bool isOnline = HelperInternet.IsOnline();
            //            //if (isOnline)
            //            //{

            //            //sqlstatus1.Text = "HEJ Der 9: resourceHours: " + resourceHours + " hoursBefore: " + hoursReportedBefore + " isOnline: " + isOnline;



            //        }
            //        catch
            //        {


            //            var activities = outz_JobCustomer.GetActivities(autoCustomerJob.Text, lstJobCustomerNames);
            //            var activity = activities.FirstOrDefault(act => act.Id == SelectedActivityId.Value);
            //            if (activity != null)
            //            {
            //                resourceHours = activity.ResourceHours;
            //                hoursReportedBefore = activity.ReportedHours;

            //                if (decimal.Parse(txtHours.Text) + hoursReportedBefore <= resourceHours)
            //                {
            //                    // activity.ReportedHours = 100;
            //                    activity.ReportedHours = hoursReportedBefore + decimal.Parse(txtHours.Text);
            //                    //activity.ResourceHours = activity.ResourceHours;
            //                }
            //            }
            //            //sqlstatus1.Text = "HEJ Der 9: Offline";

            //            //sqlstatus2.Text = "HEJ Der 10: resourceHours: " + resourceHours + " hoursReportedBefore: " + hoursReportedBefore + "  txtHours: " + decimal.Parse(txtHours.Text) + " isOnline: " + isOnline;

            //        }
            //        if (decimal.Parse(txtHours.Text) + hoursReportedBefore > resourceHours)
            //        {

            //            message = FindResource("ResourcesExceeded2") + " " + (resourceHours - (hoursReportedBefore + decimal.Parse(txtHours.Text))) + " " + FindResource("ResourcesExceeded3").ToString();
            //            return false;
            //        }
            //        //sqlstatus3.Text = "HEJ Der 11: resourceHours: " + resourceHours + " hoursReportedBefore: " + hoursReportedBefore + "txtHours: " + decimal.Parse(txtHours.Text);

            //    }

            //} // online

            message = string.Empty;
            return true;
        }

        private void timerShort_Tick(object sender, EventArgs e)
        {

            if (txtSubmitStatus.Visibility == System.Windows.Visibility.Visible)
                txtSubmitStatus.Visibility = System.Windows.Visibility.Collapsed;

            if (txtrefresh.Visibility == System.Windows.Visibility.Visible)
            {
                txtrefresh.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void opdJobliste_Click(object sender, RoutedEventArgs e)
        {
            autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs(selectedDate.SelectedDate.Value);
            autoCustomerJob.ItemFilter += TimeReportHelper.SearchCustomerJob;
            txtrefresh.Visibility = System.Windows.Visibility.Visible;
            settimer();
        }

        private void autoCustomerJob_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(autoCustomerJob.Text) && e.AddedItems.Count > 0)
                {
                    List<outz_Activity> activities = null;
                    if (((outz_JobCustomer)e.AddedItems[0]).IsCustomer)
                    {
                        e.Handled = true;
                        ((AutoCompleteBox)sender).SelectedItem = null;
                        activities = new List<outz_Activity>();
                    }
                    else
                    {
                        outz_TimeTag tt = new outz_TimeTag();
                        var sw = new Stopwatch();
                        sw.Start();
                        //activities = TimeReportHelper.GetJobActivities(autoCustomerJob.Text, (List<outz_JobCustomer>)autoCustomerJob.ItemsSource, selectedDate.SelectedDate.Value);
                        activities = ((outz_JobCustomer)e.AddedItems[0]).Activities;
                        sw.Stop();
                        if (sw.ElapsedMilliseconds > 2000)
                        {
                            outz_Log.LogToFile(string.Format("GetJobActivities() has taken {0}ms", sw.ElapsedMilliseconds));
                        }
                    }
                    autoActivity.ItemsSource = activities;
                    if (ConfigHelper.ListActivities)
                    {
                        autoActivityList.ItemsSource = activities;
                        autoActivityList.IsEnabled = true;
                        if (activities.Count == 1)
                        {
                            autoActivityList.SelectedIndex = 0;
                        }
                    }
                    else
                    {
                        autoActivity.ItemFilter += TimeReportHelper.SearchActivity;
                    }
                }
            }
            catch (Exception ex)
            {
                outz_Log.LogError("customer job selection changed issue: " + ex.Message);
            }
        }

        #endregion

        private void ShowStatus(string statusMsg)
        {
            txtSubmitStatus.Text = statusMsg;
            txtSubmitStatus.ToolTip = statusMsg;
            txtSubmitStatus.Foreground = Brushes.Red;
            txtSubmitStatus.Visibility = System.Windows.Visibility.Visible;
        }

        private void ReSet()
        {
            autoCustomerJob.Text = string.Empty;
            autoActivity.Text = string.Empty;
            autoActivityList.IsEnabled = false;
            autoActivityList.ItemsSource = null;
            autoActivityList.SelectedIndex = -1;
            txtHours.Text = string.Empty;
            txtComments.Text = string.Empty;
        }

        public void settimer()
        {
            timerShort.Interval = new TimeSpan(0, 0, 0, 8);
            timerShort.Tick += new EventHandler(timerShort_Tick);
            timerShort.Start();
        }

        public void InitTimer()
        {
            RunInternetStatusCheck();

            selectedDate.SelectedDate = DateTime.Today;
            selectedDate.SelectedDateChanged += selectedDate_SelectedDateChanged;
            //Init auto complete box of customer and job
            autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs(selectedDate.SelectedDate.Value);
            autoCustomerJob.ItemFilter += TimeReportHelper.SearchCustomerJob;
        }

        private void RunInternetStatusCheck()
        {
            worker = new BackgroundWorker();
            var oldStatus = internetStatus;
            var jobList = (List<outz_JobCustomer>)autoCustomerJob.ItemsSource;
            worker.DoWork += new DoWorkEventHandler((x, y) =>
            {
                internetStatus = TimeReportHelper.RefreshInternetStatus();
                if (internetStatus == InternetStatus.Online && oldStatus == InternetStatus.Offline)
                {
                    TimeReportHelper.EmptyOfflineTime(jobList);
                }
                Thread.Sleep(5000);
            });
            worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler((x, y) =>
            {
                var converter = new System.Windows.Media.BrushConverter();
                if (internetStatus == InternetStatus.Online)
                {
                    titleOnline.Text = FindResource("Online").ToString();
                    ellipseOnline.Fill = (Brush)converter.ConvertFromString("#FF6BAE58");
                    ellipseOnline.OpacityMask = (Brush)converter.ConvertFromString("#FF6BAE58");
                    ellipseOnline.Stroke = Brushes.Lime;
                }
                else
                {
                    titleOnline.Text = FindResource("Offline").ToString();
                    ellipseOnline.Fill = Brushes.Red;
                    ellipseOnline.OpacityMask = Brushes.Red;
                    ellipseOnline.Stroke = Brushes.Red;
                }
                RunInternetStatusCheck();
            });
            worker.RunWorkerAsync();
        }

        private void SetInternetStatus()
        {
            internetStatus = TimeReportHelper.RefreshInternetStatus();
            var converter = new System.Windows.Media.BrushConverter();
            if (internetStatus == InternetStatus.Online)
            {
                titleOnline.Text = FindResource("Online").ToString();
                ellipseOnline.Fill = (Brush)converter.ConvertFromString("#FF6BAE58");
                ellipseOnline.OpacityMask = (Brush)converter.ConvertFromString("#FF6BAE58");
                ellipseOnline.Stroke = Brushes.Lime;
            }
            else
            {
                titleOnline.Text = FindResource("Offline").ToString();
                ellipseOnline.Fill = Brushes.Red;
                ellipseOnline.OpacityMask = Brushes.Red;
                ellipseOnline.Stroke = Brushes.Red;
            }
        }

        private void selectedDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateReportedHours();
        }

        private void UpdateReportedHours()
        {
            try
            {
                if (selectedDate.SelectedDate.HasValue)
                {
                    outz_TimeTag tt = new outz_TimeTag();
                    HoursService hoursService = new HoursService(tt.LTO, tt.IsNewDb);
                    var hoursReported = hoursService.GetReportedHours(selectedDate.SelectedDate.Value, tt.MID);

                    txtHoursReported.Text = hoursReported.ToString("N2");
                    if (tt.PA == "2")
                    {
                        autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs(selectedDate.SelectedDate.Value);
                    }
                }
                else
                {
                    txtHoursReported.Text = 0.ToString("N2");
                }
            }
            catch
            {
                txtHoursReported.Text = 0.ToString("N2");
                outz_TimeTag tt = new outz_TimeTag();
                if (tt.PA == "2")
                {
                    var activities = outz_JobCustomer.GetActivities(autoCustomerJob.Text, lstJobCustomerNames);
                    if (activities != null)
                    {
                        foreach (var act in activities)
                        {
                            act.ResourceHours = 0;
                            act.ReportedHours = 0;
                        }
                    }
                }
            }
        }
    }
}
