using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace TimeTag
{
    public class outz_JobCustomer
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string JobNo { get; set; }
        public int JobStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public bool IsCustomer { get; set; }
        public string CustomerInit { get; set; }
        public List<outz_Activity> Activities { get; set; }

        /// <summary>
        /// List of job under customer if the instance is a customer
        /// </summary>
        public List<outz_Job> ListJob = new List<outz_Job>();

        /// <summary>
        /// List of job customer for auto compelete box with customized matching strategy and to string value define
        /// </summary>
        public List<outz_JobCustomer> ListAllJobCustomer = new List<outz_JobCustomer>();

        public Brush ItemColor
        {
            get
            {
                return this.IsCustomer ? Brushes.Gray : Brushes.Black;
            }
        }

        public outz_JobCustomer()
        {
        }

        /// <summary>
        /// Init customer item
        /// </summary>
        /// <param name="customerno"></param>
        /// <param name="customername"></param>
        /// <param name="lstJob"></param>
        public outz_JobCustomer(string customerno, string customername, string customerinit, List<outz_Job> lstJob)
        {
            this.CustomerNo = customerno;
            this.CustomerName = customername;
            this.CustomerInit = customerinit;
            this.ListJob = lstJob;

            this.IsCustomer = true;
        }

        /// <summary>
        /// Init job item
        /// </summary>
        /// <param name="customerno"></param>
        /// <param name="customername"></param>
        /// <param name="jobid"></param>
        /// <param name="jobname"></param>
        /// <param name="jobno"></param>
        public outz_JobCustomer(string customerno, string customername, int jobid, string jobname, string jobno, string customerinit)
        {
            this.CustomerNo = customerno;
            this.CustomerName = customername;
            this.JobId = jobid;
            this.JobName = jobname;
            this.JobNo = jobno;
            this.CustomerInit = customerinit;
          
            
            this.IsCustomer = false;    
        

        }

        /// <summary>
        /// Overwrite ToString to show hierarchecal. Customer as title with indent job list below
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (this.IsCustomer)
                return this.CustomerName;
            else
                // TT 23112014 return " " + this.JobName + " (" + this.JobNo + ")";
                return this.JobName + " (" + this.JobNo + ")";
        }

        public string GetName()
        {
            if (this.IsCustomer)
                return this.CustomerName;
            else
                return this.JobName;
        }

        /// <summary>
        /// Get all the job and customer items for auto compelete box with job customer list and joblist under each customer
        /// </summary>
        /// <param name="onlyActiveJobList"></param>
        public void GetAllNames(bool? onlyActiveJobList, string lto, string mid, bool isNewDb)
        {
            outz_JobCustomerOrigin origin = new outz_JobCustomerOrigin();
            origin.GetAll(HelperNullable.ConvertBoolean(onlyActiveJobList), lto, mid, isNewDb);

            List<outz_Customer> lstAllCustomers = outz_Customer.Convert(origin.ListAllJobCustomers);
            
            foreach (outz_Customer customer in lstAllCustomers)
            {
                this.ListAllJobCustomer.Add(new outz_JobCustomer(customer.CustomerNo, customer.CustomerName, customer.CustomerInit, customer.JobList));

                foreach (outz_Job job in customer.JobList)
                {
                    var customerJob = new outz_JobCustomer(job.CustomerNo, job.CustomerName, job.JobId, job.JobName, job.JobNo, job.CustomerInit);
                    outz_Activity activity = new outz_Activity();
                    activity.GetAllNames(true, mid, job.JobId.ToString(), lto, isNewDb);
                    customerJob.Activities = activity.ListAllActivities;
                    this.ListAllJobCustomer.Add(customerJob);
                    //this.ListAllJobCustomer.Add(new outz_JobCustomer(customer.CustomerNo, customer.CustomerName, customer.CustomerInit, customer.JobList));
                    //  this.ListAllJobCustomer.Add(new outz_JobCustomer(job.CustomerNo, "", job.JobId, job.JobName, job.JobNo, job.CustomerInit));
                }
            }
        }

        /// <summary>
        /// Define match strategy for each job customer object
        /// </summary>
        /// <param name="search"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SearchCustomerJob(string search, object value)
        {
            search = search.ToLower();
            outz_JobCustomer jc = (outz_JobCustomer)value;

            
            //if (jc.JobName != null) {
            if (String.IsNullOrEmpty(jc.JobName))
            {
                jobnavnsog = "";
            }
            else {
                jobnavnsog = jc.JobName.ToLower();
            }

            //outz_Log.LogError("Jobnavn: " + jobnavnsog.ToLower() + " Search: " + search.ToLower() + " Index: " + jobnavnsog.ToLower().IndexOf(search.ToLower()));

            if (jc != null) //&& search.ToLower() != null
            {
                if (String.IsNullOrEmpty(jc.CustomerInit)) // Job
                {
                    //Look for a match in jobs and customers
                    //outz_Log.LogError("A Jobnavn: " + jobnavnsog.ToLower() + " Search: " + search.ToLower() + " Index: " + jobnavnsog.ToLower().IndexOf(search.ToLower()));
                    //if (jc.ToString().ToLower().StartsWith(search.ToLower()))
                    if (jobnavnsog.StartsWith(search) || jobnavnsog.IndexOf(search) > 0 || (!string.IsNullOrEmpty(jc.JobNo) && jc.JobNo.Contains(search))) //Jobname
                        return true;
                }
                else
                {
                    //Look for a match in jobs and customers
                    if (jc.CustomerInit.ToString().ToLower().StartsWith(search) || jc.CustomerInit.ToLower() == search)
                    //outz_Log.LogError("B Jobnavn: " + jobnavnsog.ToLower() + " Search: " + search + " Index: " + jobnavnsog.ToLower().IndexOf(search));
                    //if (jobnavnsog.ToLower().IndexOf(search) > 0) //Jobname
                           return true;
                }

                //Look for a match in customers who has the matched job
                if (jc.IsCustomer)
                {
                    if ((!string.IsNullOrEmpty(jc.CustomerName) && jc.CustomerName.ToLower().Contains(search)) || (!string.IsNullOrEmpty(jc.JobNo) && jc.JobNo.Contains(search)))
                    {
                        return true;
                    }
                    foreach (outz_Job job in jc.ListJob)
                    {
                        if ((!string.IsNullOrEmpty(job.JobName) && job.JobName.ToLower().Contains(search)) || (!string.IsNullOrEmpty(job.JobNo) && job.JobNo.Contains(search)))
                        {
                            return true;
                        }
                        //if (String.IsNullOrEmpty(jc.CustomerInit))
                        //{
                        //    if (jc.JobName != null)
                        //    { 
                        //    //job.JobName.ToLower().StartsWith(search.ToLower())

                        //    //outz_Log.LogError("Jobnavn: " + jobnavnsog.ToLower() + " Search: " + search.ToLower() + " Index: " + jobnavnsog.ToLower().IndexOf(search.ToLower()));
                        //    if (jobnavnsog.ToLower().StartsWith(search.ToLower()) || jobnavnsog.ToLower().IndexOf(search.ToLower()) > 0) //Jobname
                        //        return true;

                        //    }
                        //}
                        //else
                        //{
                        //    //job.JobName.ToLower().StartsWith(search.ToLower())
                        //    //outz_Log.LogError("Jobnavn: " + jc.JobName.ToLower() + " Search: " + search.ToLower() + " Index: " + jc.JobName.ToLower().IndexOf(search.ToLower()));
                        //    if (job.JobName.ToLower().StartsWith(search.ToLower()) || jc.JobName.ToLower().IndexOf(search.ToLower()) > 0 || jc.CustomerInit.ToLower() == search.ToLower())
                        //        return true;
                        //}            
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(jc.CustomerName) && jc.CustomerName.ToLower().Contains(search))
                    {
                        return true;
                    }
                    if ((!string.IsNullOrEmpty(jc.JobName) && jc.JobName.ToLower().Contains(search)) || (!string.IsNullOrEmpty(jc.JobNo) && jc.JobNo.Contains(search)))
                    {
                        return true;
                    }

                    //if (String.IsNullOrEmpty(jc.CustomerInit))
                    //{
                    //    //if (jc.CustomerName.ToLower().StartsWith(search.ToLower()))
                    //    //outz_Log.LogError("Kundenavn: "+ jc.CustomerName.ToLower() + " Search: " + search.ToLower() +" Index: "+ jc.CustomerName.ToLower().IndexOf(search.ToLower()));
                    //    if (jc.CustomerName.ToLower().StartsWith(search.ToLower()) || jc.CustomerName.ToLower().IndexOf(search.ToLower()) > 0) //Customername

                    //        return true;
                    //}
                    //else
                    //{
                    //    //jc.CustomerName.ToLower().StartsWith(search.ToLower())
                    //    if (jc.CustomerName.ToLower().StartsWith(search.ToLower()) || jc.CustomerName.ToLower().IndexOf(search.ToLower()) > 0 || jc.CustomerInit.ToLower() == search.ToLower())
                    //        return true;
                    //}        
                }

            }

            return false;
        }

        /// <summary>
        /// Get job id or customer no by job name or customer name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>"0" for not found job</returns>
        public static string GetId(string name, List<outz_JobCustomer> listNames, bool jobNoIncluded = true)
        {
            string id = "0";
            string tmp = "";
            foreach (outz_JobCustomer jc in listNames)
            {
                tmp = jc.JobName + (jobNoIncluded ? " (" + jc.JobNo + ")" : "");
                if (jc.JobName != null && tmp.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    id = jc.JobId.ToString();
                    break;
                }
                else if (jc.CustomerName != null && jc.CustomerName.Equals(name, StringComparison.InvariantCultureIgnoreCase) && jc.ListJob.Count > 0)
                {
                    id = jc.ListJob[0].JobId.ToString();
                    break;
                }
            }
            return id;
        }

        public static void SetActivities(string name, List<outz_JobCustomer> listNames, List<outz_Activity> activities, bool jobNoIncluded = true)
        {
            string tmp = "";
            foreach (outz_JobCustomer jc in listNames)
            {
                tmp = jc.JobName + (jobNoIncluded ? " (" + jc.JobNo + ")" : "");
                if (jc.JobName != null && tmp.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    jc.Activities = activities;
                    break;
                }
                else if (jc.CustomerName != null && jc.CustomerName.Equals(name, StringComparison.InvariantCultureIgnoreCase) && jc.ListJob.Count > 0)
                {
                    jc.ListJob[0].Activities = activities;
                    break;
                }
            }
        }

        public static List<outz_Activity> GetActivities(string name, List<outz_JobCustomer> listNames, bool jobNoIncluded = true)
        {
            List<outz_Activity> activities = null;
            string tmp = "";
            foreach (outz_JobCustomer jc in listNames)
            {
                tmp = jc.JobName + (jobNoIncluded ? " (" + jc.JobNo + ")" : "");
                if (jc.JobName != null && tmp.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    activities = jc.Activities;
                    break;
                }
                else if (jc.CustomerName != null && jc.CustomerName.Equals(name, StringComparison.InvariantCultureIgnoreCase) && jc.ListJob.Count > 0)
                {
                    activities = jc.ListJob[0].Activities;
                    break;
                }
            }
            return activities;
        }

        public static string jobnavnsog { get; set; }
    }
}
