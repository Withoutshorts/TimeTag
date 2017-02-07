using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class outz_Customer : IComparable<outz_Customer>
    {
        public string CustomerNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerInit { get; set; }

        //Job list under each customer
        public List<outz_Job> JobList { get; set; }

        public outz_Customer()
        {
            this.JobList = new List<outz_Job>();
        }

        public outz_Customer(string customerno, string customername, string customerinit)
        {
            this.CustomerNo = customerno;
            this.CustomerName = customername;
            this.CustomerInit = customerinit;

            this.JobList = new List<outz_Job>();
        }

        public static List<outz_Customer> Convert(List<outz_JobCustomerOrigin> jobCustomerList)
        {
            List<outz_Customer> lstCustomers = new List<outz_Customer>();

            foreach (outz_JobCustomerOrigin jc in jobCustomerList)
            {
                outz_Customer customer = new outz_Customer(jc.CustomerNo, jc.CustomerName, jc.CustomerInit);
                if (!lstCustomers.Exists(x => x.CustomerName == customer.CustomerName))
                    lstCustomers.Add(customer);
                else
                {
                    customer = lstCustomers.Where(x => x.CustomerName == customer.CustomerName).First();
                }

                outz_Job job = new outz_Job(jc.JobId, jc.JobName, jc.JobNo, 0, jc.CustomerNo, jc.CustomerName, jc.CustomerInit);
                if (!customer.JobList.Contains(job))
                    customer.JobList.Add(job);
            }

            return lstCustomers;
        }
    
        public int  CompareTo(outz_Customer other)
        {
            return this.CustomerNo.CompareTo(other.CustomerNo);
        }        
    }
}
