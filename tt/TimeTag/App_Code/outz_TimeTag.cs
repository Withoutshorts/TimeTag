using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Data;

namespace TimeTag
{
    public class outz_TimeTag
    {
        /// <summary>
        /// Init to 0, assume to be signed by web service
        /// </summary>
        /// 

        public string jobnavnsog { get; set; }

        public double Id { get; set; }
        public string CustomerJob { get; set; }
        public int JobId { get { return _jobId; } }
        public string Activity { get; set; }
        public float Hours { get; set; }
        public string Comments { get; set; }
        public Int64 ActivityId { get; set; }
        public string JobName { get; set; }
        /// <summary>
        /// Status 0 if no customer or job matches, else Status 1
        /// </summary>
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public string LTO { get; set; }
        public string MID { get; set; }
        public string PA { get; set; }
        public bool IsNewDb { get; set; }
        public readonly int ORINGIN = 11;//DO NOT change the value
        public readonly string Editor = "TimeTag";

        public string stTid { get; set; }
        public string slTid { get; set; }

        public DataSet DsTimeTag { get { return _dsTimeTag; } }

        public static string Success_Web_Service { get { return SUCCESS_WEB_SERVICE; } }
        public static string TimeOfflineFile { get { return TIME_OFFLINE_FILE; } }
        public static string UserInfoFile { get { return USER_INFO_FILE; } }

        const string SUCCESS_WEB_SERVICE = "succes";
        const string TIME_OFFLINE_FILE = "TimeOffline.txt";
        const string USER_INFO_FILE = "UserInfo.txt";

        private DataSet _dsTimeTag;
        private int _jobId;
       
        //private List<outz_JobCustomer> lstJobCustomerNames;
        //private List<outz_Activity> lstActivities;
      

        public outz_TimeTag()
        {
            this.CreatedTime = DateTime.Now;
            GetUserInfo();
        }

        public outz_TimeTag(string timeRow, List<outz_JobCustomer> lstNames)
        {
            try
            {
                string[] times = timeRow.Split(',');
                if (times.Length == 6)
                {
                    this.Id = 0;
                    this.CustomerJob = times[0];
                    this.JobName = times[0] != null ? this.CustomerJob.Trim() : "";
                    this._jobId = int.Parse(outz_JobCustomer.GetId(this.CustomerJob, lstNames, false));
                    this.Activity = times[1];
                    float hours = 0.0f;
                    float.TryParse(times[2], out hours);
                    this.Hours = hours;
                    this.Comments = times[3];
                    this.Status = GetStatus(this.CustomerJob, lstNames);
                    this.CreatedTime = DateTime.Parse(times[5], CultureInfo.CurrentCulture);
                    this.stTid = "00:00:00";
                    this.slTid = "00:00:00";

                    GetUserInfo();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Construct from offline exception: " + ex.Message);
            }
        }

        public outz_TimeTag(string customerJob, string customerNameWithNo, string activity, float hours, string comments, List<outz_JobCustomer> lstNames, List<outz_Activity> lstActivities, string startTid, string slutTid, DateTime createdDate)
        {
            try
            {
                this.Id = 0;
                this.CustomerJob = customerJob;
                this.JobName = customerJob.Trim();
                this._jobId = int.Parse(outz_JobCustomer.GetId(customerNameWithNo, lstNames, true));
                this.Activity = activity;
                this.ActivityId = int.Parse(outz_Activity.GetId(this.Activity, lstActivities));
                this.Hours = hours;
                this.Comments = comments;
                this.Status = GetStatus(customerJob, lstNames);
                this.CreatedTime = createdDate;
                this.stTid = startTid;
                this.slTid = slutTid;

                GetUserInfo();
            }
            catch (Exception ex)
            {
                throw new Exception("Construct exception: " + ex.Message);
            }
        }

       /*  public outz_TimeTag(string p1, string p2, float p3, string p4, List<outz_JobCustomer> lstJobCustomerNames, List<outz_Activity> lstActivities, string p5, string p6)
        {
            // TODO: Complete member initialization
            this.p1 = p1;
            this.p2 = p2;
            this.p3 = p3;
            this.p4 = p4;
            this.lstJobCustomerNames = lstJobCustomerNames;
            this.lstActivities = lstActivities;
            this.p5 = p5;
            this.p6 = p6;
        } */

        public void InitDataSet()
        {
            this._dsTimeTag = new DataSet();

            // Create data table in runtime
            DataTable timeTagTable = new DataTable("TimeTag");

            if (timeTagTable != null)
            {
                DataColumn column1 = new DataColumn("id", typeof(double));
                timeTagTable.Columns.Add(column1);
                DataColumn column2 = new DataColumn("dato", typeof(DateTime));
                timeTagTable.Columns.Add(column2);
                DataColumn column3 = new DataColumn("editor", typeof(string));
                timeTagTable.Columns.Add(column3);
                DataColumn column4 = new DataColumn("origin", typeof(int));
                timeTagTable.Columns.Add(column4);
                DataColumn column5 = new DataColumn("medarbejderid", typeof(string));
                timeTagTable.Columns.Add(column5);
                DataColumn column6 = new DataColumn("jobid", typeof(int));
                timeTagTable.Columns.Add(column6);
                DataColumn column7 = new DataColumn("aktnavn", typeof(string));
                timeTagTable.Columns.Add(column7);
                DataColumn column8 = new DataColumn("timer", typeof(double));
                timeTagTable.Columns.Add(column8);
                DataColumn column9 = new DataColumn("tdato", typeof(DateTime));
                timeTagTable.Columns.Add(column9);
                DataColumn column10 = new DataColumn("lto", typeof(string));
                timeTagTable.Columns.Add(column10);
                DataColumn column11 = new DataColumn("timerkom", typeof(string));
                timeTagTable.Columns.Add(column11);
                DataColumn column12 = new DataColumn("akt_id", typeof(Int64));
                timeTagTable.Columns.Add(column12);
                DataColumn column13 = new DataColumn("jobnavn", typeof(string));
                timeTagTable.Columns.Add(column13);
                DataColumn column14 = new DataColumn("sttid", typeof(string));
                timeTagTable.Columns.Add(column14);
                DataColumn column15 = new DataColumn("sltid", typeof(string));
                timeTagTable.Columns.Add(column15);

                this._dsTimeTag.Tables.Add(timeTagTable);

                // Fill the data table from reader data
                DataRow dataRow = timeTagTable.NewRow();

                dataRow["id"] = this.Id;
                dataRow["dato"] = this.CreatedTime;
                dataRow["editor"] = this.Editor;
                dataRow["origin"] = this.ORINGIN;
                dataRow["medarbejderid"] = this.MID;
                dataRow["jobid"] = this.JobId;
                dataRow["aktnavn"] = this.Activity;
                dataRow["timer"] = this.Hours;
                dataRow["tdato"] = this.CreatedTime;
                dataRow["lto"] = this.LTO;
                dataRow["timerkom"] = this.Comments;
                dataRow["akt_id"] = this.ActivityId;
                dataRow["jobnavn"] = this.JobName;
                dataRow["sttid"] = this.stTid;
                dataRow["sltid"] = this.slTid;

                timeTagTable.Rows.Add(dataRow);
            }
        }

        private void GetUserInfo()
        {
            try
            {
                string[] infos = HelperSetting.ReadLines(Properties.Settings.Default.UserInfo, 4);
                this.LTO = infos[0].Split(':').Skip(1).ToArray()[0];
                this.MID = infos[1].Split(':').Skip(1).ToArray()[0];
                this.PA = infos[2].Split(':').Skip(1).ToArray()[0];
                this.IsNewDb = infos[3].Split(':').Skip(1).ToArray()[0] == "new";
            }
            catch (Exception ex)
            {
                throw new Exception("Get user info exception: " + ex.Message);
            }
        }

        private int GetStatus(string name, List<outz_JobCustomer> lstJobCustomers)
        {
            int status = 0;

            foreach (outz_JobCustomer jc in lstJobCustomers)
            {
                bool exist = outz_JobCustomer.SearchCustomerJob(name, jc);
                if (exist)
                {
                    status = 1;
                    break;
                }
            }

            return status;
        }

        public string UploadTime()
        {
            string uploadStatus = string.Empty;

            try
            {
                //Log offline
                string timeRow = string.Format("{0},{1},{2},{3},{4},{5}", this.CustomerJob, this.Activity, this.Hours, this.Comments, this.Status, this.CreatedTime);
                HelperSetting.Write(timeRow, Properties.Settings.Default.TimeOffline, true);

                uploadStatus = UploadTimeService(IsNewDb);

                if (uploadStatus.Contains(SUCCESS_WEB_SERVICE))
                    Properties.Settings.Default.TimeOffline.Clear();
            }
            catch (Exception ex)
            {
                throw new Exception("Upload time exception: " + ex.Message);
            }

            return uploadStatus;
        }

        public string UploadTimeService(bool isNewDb)
        {
            string uploadStatus = string.Empty;
            try
            {
                bool isOnline = HelperInternet.IsOnline();
                if (isOnline)
                {
                    //Call web service return upload status
                    //dk.outzource.to_import outz = new dk.outzource.to_import();                    
                    //uploadStatus = outz.timeout_importTimer(this.DsTimeTag);

                    if (isNewDb)
                    {
                        var serv = new dk.outzource2.to_import_timetagSoapClient();
                        uploadStatus = serv.timeout_importTimer_timetag(this.DsTimeTag);
                    }
                    else
                    {
                        var outz = new dk.outzource1.to_import_timetag();
                        uploadStatus = outz.timeout_importTimer_timetag(this.DsTimeTag);
                    }
                    

                    if (uploadStatus.Contains(SUCCESS_WEB_SERVICE))
                        Properties.Settings.Default.TimeOffline.Clear();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Upload web service exception: " + ex.Message);
            }

            return uploadStatus;
        }
    }
}
