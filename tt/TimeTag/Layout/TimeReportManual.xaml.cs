using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
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
                string validationMessage;
                if (!ValidateSubmittedData(out validationMessage))
                {
                    ShowStatus(validationMessage);
                    outz_Log.LogError(validationMessage);
                    return;
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
                string status = tt.UploadTime();
                if (status.Contains(outz_TimeTag.Success_Web_Service))
                {
                    //txtSubmitStatus.Text = outz_TimeTag.Success_Web_Service;
                    //txtSubmitStatus.Foreground = Brushes.Green;
                    txtSubmitStatus.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    ShowStatus(status);
                }

                ReSet();
                //Log time upload status
                outz_Log.LogStatus(status);
                UpdateReportedHours();
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
                var activityName = ConfigHelper.ListActivities ? autoActivityList.Text : autoActivity.Text;
                if (string.IsNullOrEmpty(autoCustomerJob.Text))
                {
                    message = FindResource("CustomerJobRequired").ToString();
                    return false;
                }
                if (lstJobCustomerNames.All(cust => !cust.ToString().Equals(autoCustomerJob.Text, StringComparison.InvariantCultureIgnoreCase)))
                {
                    message = FindResource("CustomerJobWrong").ToString();
                    return false;
                }
                if (string.IsNullOrEmpty(activityName))
                {
                    message = FindResource("ActivityRequired").ToString();
                    return false;
                }
                if (lstActivities.All(act => !act.Name.ToString().Equals(activityName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    message = FindResource("ActivityWrong").ToString();
                    return false;
                }
            }
            if (UserInfoProvider.PA == "2" && 
                !string.IsNullOrEmpty(UserInfoProvider.LTO) && 
                "bf".Equals(UserInfoProvider.LTO, StringComparison.InvariantCultureIgnoreCase) && 
                SelectedCustomerId.HasValue && SelectedActivityId.HasValue)
            {
                var rdp = new ResourceDataProvider(UserInfoProvider.LTO, UserInfoProvider.IsNewDb);
                var resourceHours = rdp.GetResourceHours(UserInfoProvider.MID, SelectedCustomerId.Value, SelectedActivityId.Value, selectedDate.SelectedDate.Value);
                HoursService hoursService = new HoursService(UserInfoProvider.LTO, UserInfoProvider.IsNewDb);
                var hoursReportedBefore = hoursService.GetReportedHoursByActivity(UserInfoProvider.MID, SelectedActivityId.Value, selectedDate.SelectedDate.Value);
                if (decimal.Parse(txtHours.Text) + hoursReportedBefore > resourceHours)
                {
                    message = FindResource("ResourcesExceeded2") + " " + (resourceHours-(hoursReportedBefore + decimal.Parse(txtHours.Text))) + " " + FindResource("ResourcesExceeded3").ToString();
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        private void timerShort_Tick(object sender, EventArgs e)
        {
            SetInternetStatus();

            if (txtSubmitStatus.Visibility == System.Windows.Visibility.Visible)
                txtSubmitStatus.Visibility = System.Windows.Visibility.Collapsed;

            if (txtrefresh.Visibility == System.Windows.Visibility.Visible)
            {
                txtrefresh.Visibility = System.Windows.Visibility.Hidden;
            }
        }

        private void opdJobliste_Click(object sender, RoutedEventArgs e)
        {
            autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs(); 
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
                        activities = TimeReportHelper.GetJobActivities(autoCustomerJob.Text, (List<outz_JobCustomer>)autoCustomerJob.ItemsSource);
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


        public void settimer ()
        {
            timerShort.Interval = new TimeSpan(0, 0, 0, 4);
            timerShort.Tick += new EventHandler(timerShort_Tick);
            timerShort.Start();
        }

        /*  public void ShowStatusgreen ()
          {
              //txtinlast.Visibility = System.Windows.Visibility.Visible;
              //txtinlast.Text = i.ToString();
              txtinlast.Visibility = System.Windows.Visibility.Visible;
              System.Threading.Thread.Sleep(10000);
              txtinlast.Visibility = System.Windows.Visibility.Hidden;
          } */


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

        public void InitTimer()
        {
            timerShort.Interval = new TimeSpan(0, 0, 0, 10);
            timerShort.Tick += new EventHandler(timerShort_Tick);
            timerShort.Start();
            selectedDate.SelectedDate = DateTime.Today;
            //Init auto complete box of customer and job
            autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs();
            autoCustomerJob.ItemFilter += TimeReportHelper.SearchCustomerJob;
        }

        private void SetInternetStatus()
        {
            internetStatus = TimeReportHelper.RefreshInternetStatus(internetStatus, (List<outz_JobCustomer>)autoCustomerJob.ItemsSource);
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
            if (selectedDate.SelectedDate.HasValue)
            {
                outz_TimeTag tt = new outz_TimeTag();
                HoursService hoursService = new HoursService(tt.LTO, tt.IsNewDb);
                var hoursReported = hoursService.GetReportedHours(selectedDate.SelectedDate.Value, tt.MID);
                txtHoursReported.Text = hoursReported.ToString("N2");
            }
            else
            {
                txtHoursReported.Text = 0.ToString("N2");
            }
        }
    }
}
