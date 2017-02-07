using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class outz_Job : IComparable<outz_Job>  
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string JobNo { get; set; }
        public int JobStatus { get; set; }

        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerInit { get; set; }

        public outz_Job(int jobid, string jobname, string jobno, int jobstatus)
        {
            this.JobId = jobid;
            this.JobName = jobname;
            this.JobNo = jobno;
            this.JobStatus = jobstatus;
        }

        public outz_Job(int jobid, string jobname, string jobno, int jobstatus, string customerno, string customername, string customerinit)
        {
            this.JobId = jobid;
            this.JobName = jobname;
            this.JobNo = jobno;
            this.JobStatus = jobstatus;
            this.CustomerNo = customerno;
            this.CustomerName = customername;
            this.CustomerInit = customerinit;
        }

        public int CompareTo(outz_Job job)
        {
            return this.JobId.CompareTo(job.JobId);
        }
    }
}
