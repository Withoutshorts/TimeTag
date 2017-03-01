using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TimeTag.App_Code;
using TimeTag.Entity;

namespace TimeTag.Helper
{
    public class TimeReportHelper
    {
        public const string INFO_NOT_SUBMIT = "Timer er ligger i kø og sendes når du er online.";

        public static InternetStatus RefreshInternetStatus()
        {
            try
            {
                bool isOnline = HelperInternet.IsOnline();
                
                if (isOnline)
                {
                    return InternetStatus.Online;
                }
                else
                {
                    return InternetStatus.Offline;
                }
            }
            catch (Exception ex)
            {
                outz_Log.LogError(ex.Message);
            }
            return InternetStatus.Notset;
        }

        public static void EmptyOfflineTime(List<outz_JobCustomer> jobs)
        {
            try
            {
                string[] timeLines = HelperSetting.ReadLines(Properties.Settings.Default.TimeOffline, 10);

                foreach (string timeLine in timeLines)
                {
                    if (timeLine != null && timeLine.Trim() != string.Empty)
                    {
                        outz_TimeTag tt = new outz_TimeTag(timeLine, jobs);
                        tt.InitDataSet();
                        string status = tt.UploadTimeService(tt.IsNewDb);
                        //Log time upload status
                        outz_Log.LogStatus("Empty offline time status: " + status);
                    }
                }
            }
            catch (Exception ex)
            {
                outz_Log.LogError("Empty offline time issue: " + ex.Message);
            }
        }

        public static List<outz_JobCustomer> GetCustomerJobs(DateTime selectedDate)
        {
            try
            {
                outz_TimeTag tt = new outz_TimeTag();
                outz_JobCustomer jc = new outz_JobCustomer();
                var sw = new Stopwatch();
                sw.Start();
                jc.GetAllNames(tt.PA == "1", tt.LTO, tt.MID, tt.IsNewDb);
                if (tt.PA == "2")
                {
                    foreach (var job in jc.ListAllJobCustomer)
                    {
                        outz_Activity activity = new outz_Activity();
                        activity.GetAllNames(true, tt.MID, job.JobId.ToString(), tt.LTO, tt.IsNewDb);
                        string jobid = outz_JobCustomer.GetId(job.IsCustomer ? job.CustomerName : job.JobName, jc.ListAllJobCustomer);
                        foreach (var act in activity.ListAllActivities)
                        {
                            var rdp = new ResourceDataProvider(UserInfoProvider.LTO, UserInfoProvider.IsNewDb);
                            act.ResourceHours = rdp.GetResourceHours(UserInfoProvider.MID, int.Parse(jobid), act.Id, selectedDate);

                            var hoursService = new HoursService(tt.LTO, tt.IsNewDb);
                            act.ReportedHours = hoursService.GetReportedHoursByActivity(UserInfoProvider.MID, act.Id, selectedDate);
                        }
                        job.Activities = activity.ListAllActivities;
                    }
                }
                sw.Stop();
                if (sw.ElapsedMilliseconds > 2000)
                {
                    outz_Log.LogToFile(string.Format("ValidateSubmittedData() has taken {0}ms", sw.ElapsedMilliseconds));
                }
                return jc.ListAllJobCustomer;
            }
            catch (Exception ex)
            {
                outz_Log.LogError("Init auto compelete customer job issue: " + ex.Message);
            }
            return new List<outz_JobCustomer>();
        }

        public static List<outz_Activity> GetJobActivities(string selectedCustomerJob, List<outz_JobCustomer> jobs, DateTime selectedDate)
        {
            try
            {
                List<outz_Activity> activities;
                try
                {
                    outz_TimeTag tt = new outz_TimeTag();
                    outz_Activity activity = new outz_Activity();
                    string jobid = outz_JobCustomer.GetId(selectedCustomerJob, jobs);
                    var positive = true;
                    activity.GetAllNames(positive, tt.MID, jobid, tt.LTO, tt.IsNewDb);
                    activities = activity.ListAllActivities;
                }
                catch
                {
                    activities = outz_JobCustomer.GetActivities(selectedCustomerJob, jobs);
                }

                return activities;
            }
            catch (Exception ex)
            {
                outz_Log.LogError("Init auto compelete activity issue: " + ex.Message);
            }
            return new List<outz_Activity>();
        }

        public static bool SearchCustomerJob(string search, object value)
        {
            return outz_JobCustomer.SearchCustomerJob(search, value);
        }

        public static bool SearchActivity(string search, object value)
        {
            return outz_Activity.SearchActivity(search, value);
        }

        public static bool RequireAllFields()
        {
            string[] customers = HelperSetting.ReadLines(Properties.Settings.Default.UserInfo, 3);
            string[] requireAllFields = HelperSetting.ReadLines(Properties.Settings.Default.RequireAllFields, 10);
            var currentCustomer = customers[0].Split(':').Skip(1).ToArray()[0];
            return string.IsNullOrEmpty(currentCustomer) || !requireAllFields.Any(s => currentCustomer.Equals(s, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
