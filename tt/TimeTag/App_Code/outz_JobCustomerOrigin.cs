using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    /// <summary>
    /// Job Customer class for holding origin data from database
    /// </summary>
    public class outz_JobCustomerOrigin
    {
        public int JobId { get; set; }
        public string JobName { get; set; }
        public string JobNo { get; set; }
        public int JobStatus { get; set; }
        public string CustomerName { get; set; }
        public string CustomerNo { get; set; }
        public string CustomerInit { get; set; }

        /// <summary>
        /// List of job customer from database
        /// </summary>
        public List<outz_JobCustomerOrigin> ListAllJobCustomers = new List<outz_JobCustomerOrigin>();

        public outz_JobCustomerOrigin()
        {
        }

        /// <summary>
        /// Get all job and customer items from database and init customer list with customers and joblist under each customer
        /// </summary>
        /// <param name="onlyActiveJoblist"></param>
        public void GetAll(bool onlyActiveJoblist, string lto, string mid, bool isNewDb)
        {
            JobCustomerData jc = new JobCustomerData(lto, isNewDb);
            if (onlyActiveJoblist)
                this.ListAllJobCustomers = jc.SelectAllJobCustomersOnlyActiveJob(mid, "kkundenavn, jobnavn", 0, 50);
            else
                this.ListAllJobCustomers = jc.SelectAllJobCustomers(mid, "kkundenavn, jobnavn", 0, 50);
        }
    }
}
