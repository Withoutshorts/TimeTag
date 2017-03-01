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
    public partial class TimeReportStopWatch : UserControl
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
                if (autoCustomerJob != null && autoCustomerJob.ItemsSource != null && autoActivity.ItemsSource is List<outz_Activity>)
                {
                    return (List<outz_Activity>)autoActivity.ItemsSource;
                }
                return new List<outz_Activity>();
            }
        }

        public TimeReportStopWatch()
        {
            InitializeComponent();

            //Check internet on/off line once
            SetInternetStatus();
 
            //Empty offline time records if any, upload them to server
            TimeReportHelper.EmptyOfflineTime((List<outz_JobCustomer>)autoCustomerJob.ItemsSource);
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
                outz_TimeTag tt = new outz_TimeTag(jobName, ((outz_JobCustomer)autoCustomerJob.SelectedItem).ToString(), autoActivity.Text, float.Parse(txtHours.Text), txtComments.Text, lstJobCustomerNames, lstActivities, txtHours_st.Text, txtHours_sl.Text, DateTime.Now);
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
            }
            catch (Exception ex)
            {
                //ShowStatus(ex.Message);

                outz_Log.LogError(ex.Message);
            }
        }

        private bool ValidateSubmittedData(out string message)
        {
            if (string.IsNullOrEmpty(txtHours.Text))
            {
                message = FindResource("StartEndDateRequired").ToString();
                return false;
            }
            if (TimeReportHelper.RequireAllFields())
            {
                if (string.IsNullOrEmpty(txtHours_st.Text))
                {
                    message = FindResource("StartDateRequired").ToString();
                    return false;
                }
                if (string.IsNullOrEmpty(txtHours_sl.Text))
                {
                    message = FindResource("EndDateRequired").ToString();
                    return false;
                }
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
                if (string.IsNullOrEmpty(autoActivity.Text))
                {
                    message = FindResource("ActivityRequired").ToString();
                    return false;
                }
                if (lstActivities.All(act => !act.Name.ToString().Equals(autoActivity.Text, StringComparison.InvariantCultureIgnoreCase)))
                {
                    message = FindResource("ActivityWrong").ToString();
                    return false;
                }
            }
            message = string.Empty;
            return true;
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var sttidHH = DateTime.Now.ToString("HH:mm");
            var TimerMin = 0;
            txtHours.Text = TimerMin.ToString();

            if (txtHours_st.Text == "")
            {
                txtHours_st.Text = sttidHH;
                //txtHours.Text = TimerMin.ToString();
                btnStart.Background = Brushes.Crimson;
            }
            else
            {
                txtHours_sl.Text = sttidHH;
                CalculateTimeDifference();
            }
        }

        private void CalculateTimeDifference()
        {
            txtHours_st.Background = Brushes.White;
            txtHours_sl.Background = Brushes.White;
            var isAnyInvalid = false;
            if (!txtHours_st.IsValidTime())
            {
                txtHours_st.Background = Brushes.Crimson;
                isAnyInvalid = true;
            }
            if (!txtHours_sl.IsValidTime())
            {
                txtHours_sl.Background = Brushes.Crimson;
                isAnyInvalid = true;
            }

            if (isAnyInvalid || string.IsNullOrWhiteSpace(txtHours_st.Text) || string.IsNullOrWhiteSpace(txtHours_sl.Text))
            {
                return;
            }

            DateTime startTime = Convert.ToDateTime(txtHours_st.Text);
            DateTime endtime = Convert.ToDateTime(txtHours_sl.Text);

            if (startTime > endtime)
            {
                txtHours_sl.Background = Brushes.Crimson;
                return;
            }
            btnStart.Background = Brushes.Green;

            TimeSpan duration = endtime - startTime;

            var timerval = duration.ToString();

            //txtHoursLbl.Text = timerval;

            timerval = timerval.Substring(0, 2);

            var minutval = duration.ToString();
            minutval = minutval.Substring(3, 2);

            //txtHoursLbl.Text = timerval + ":" + minutval; // duration.ToString();

            txtHours.Text = timerval + "," + minutval; //+ ":" + minutval; // duration.ToString();
        }

        private void timerShort_Tick(object sender, EventArgs e)
        {
            SetInternetStatus();

            if (txtSubmitStatus.Visibility == System.Windows.Visibility.Visible)
                txtSubmitStatus.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void opdJobliste_Click(object sender, RoutedEventArgs e)
        {
            autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs(DateTime.Now);
            autoCustomerJob.ItemFilter += TimeReportHelper.SearchCustomerJob;
        }

        private void autoCustomerJob_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(autoCustomerJob.Text) && e.AddedItems.Count > 0)
                {
                    if (((outz_JobCustomer)e.AddedItems[0]).IsCustomer)
                    {
                        e.Handled = true;
                        ((AutoCompleteBox)sender).SelectedItem = null;
                        autoActivity.ItemsSource = new List<outz_Activity>();
                        autoActivity.ItemFilter += TimeReportHelper.SearchActivity;
                        return;
                    }
                    outz_TimeTag tt = new outz_TimeTag();
                    autoActivity.ItemsSource = TimeReportHelper.GetJobActivities(autoCustomerJob.Text, (List<outz_JobCustomer>)autoCustomerJob.ItemsSource, DateTime.Now);
                    autoActivity.ItemFilter += TimeReportHelper.SearchActivity;
                }
            }
            catch (Exception ex)
            {
                outz_Log.LogError("customer job selection changed issue: " + ex.Message);
            }
        }

        private void txtHoursValueChanged(object sender, EventArgs e)
        {
            CalculateTimeDifference();
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
            txtHours.Text = string.Empty;
            txtComments.Text = string.Empty;
            txtHours_st.Text = string.Empty;
            txtHours_sl.Text = string.Empty;
        }

        public void InitTimer()
        {
            timerShort.Interval = new TimeSpan(0, 0, 0, 10);
            timerShort.Tick += new EventHandler(timerShort_Tick);
            timerShort.Start();
            //Init auto complete box of customer and job
            autoCustomerJob.ItemsSource = TimeReportHelper.GetCustomerJobs(DateTime.Now);
            autoCustomerJob.ItemFilter += TimeReportHelper.SearchCustomerJob;
            UpdateReportedHours();
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

        private void UpdateReportedHours()
        {
            outz_TimeTag tt = new outz_TimeTag();
            HoursService hoursService = new HoursService(tt.LTO, tt.IsNewDb);
            var hoursReported = hoursService.GetReportedHours(DateTime.Today, tt.MID);
            hoursToday.Text = FindResource("HoursToday").ToString() + " " + hoursReported.ToString("N2");
        }
    }
}
