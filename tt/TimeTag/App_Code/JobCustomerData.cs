using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;

namespace TimeTag
{
    /// <summary>
    /// Job customer data factory
    /// </summary>
    public class JobCustomerData
    {
         private string _connectionString;

         public JobCustomerData(string lto, bool isNewDb)
         {
             //lto = "glad";
             _connectionString = DBHandler.Initialize(lto, isNewDb);
         }

         /// <summary>
         /// Select all jobCustomers
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum jobCustomers</param>
         /// <param name="maxRecords">maximum jobCustomers</param>
         /// <returns></returns>
         public List<outz_JobCustomerOrigin> SelectAllJobCustomers(string mid, string sortColumns, int startRecord, int maxRecords)
         {
             if (string.IsNullOrWhiteSpace(mid))
             {
                 mid = "1";
             }
             //string sqlCmd = "SELECT j.id, j.jobnavn, j.jobnr, j.jobstatus, k.kkundenavn, k.kkundenr, k.kinit FROM timereg_usejob AS tu" +
             //    " LEFT JOIN job AS j ON (j.id = tu.jobid) LEFT JOIN kunder AS k ON (k.kid = j.jobknr)"+
             //    " WHERE tu.medarb = " + mid + " AND (j.jobstatus = 1) "+
             //    " AND kkundenavn <> '' GROUP BY j.id ";

            string sqlCmd = "SELECT r.jobid, j.jobnavn, j.jobnr, j.jobstatus, k.kkundenavn, k.kkundenr, k.kinit, SUM(r.timer) AS restimer FROM ressourcer_md AS r " +
                            " LEFT JOIN job AS j ON (j.id = r.jobid) " +
                            " LEFT JOIN kunder AS k ON (k.kid = j.jobknr)" +
                            " WHERE r.medid = " + mid + " AND r.aktid <> 0 AND k.kkundenavn <> '' AND j.jobstatus = 1 GROUP BY r.jobid ";

            //OR j.jobstatus = 3

            if (sortColumns.Trim() == "")
                 sqlCmd += "ORDER BY kkundenavn, jobnavn";
             else
                 sqlCmd += "ORDER BY " + sortColumns;


             //sqlCmd += " LIMIT 10 ";


             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_JobCustomerOrigin> jobCustomers = new List<outz_JobCustomerOrigin>();
             int count = 0;

             try
             {
                 conn.Open();

                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         //if (jobCustomers.Count < maxRecords)
                             jobCustomers.Add(GetJobCustomerFromReader(reader));
                         //else
                         //    cmd.Cancel();
                     }

                     count++;
                 }

             }
             catch (Exception ex)
             {
                 throw new Exception("Problems in jobCustomer data factory => select all jobCustomers: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }

             return jobCustomers;
         }

         /// <summary>
         /// Select all jobCustomers
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum jobCustomers</param>
         /// <param name="maxRecords">maximum jobCustomers</param>
         /// <returns></returns>
         public List<outz_JobCustomerOrigin> SelectAllJobCustomersOnlyActiveJob(string mid, string sortColumns, int startRecord, int maxRecords)
         {
             if (string.IsNullOrWhiteSpace(mid))
             {
                 mid = "1";
             }
             //string sqlCmd = "SELECT j.id, j.jobnavn, j.jobnr, j.jobstatus, k.kkundenavn, k.kkundenr, k.kinit FROM timereg_usejob AS tu" +
             //    " LEFT JOIN job AS j ON (j.id = tu.jobid) LEFT JOIN kunder AS k ON (k.kid = j.jobknr)" +
             //    " WHERE tu.medarb = " + mid + " AND (j.jobstatus = 1) " +
             //    " AND kkundenavn <> '' AND tu.forvalgt='1' GROUP BY j.id ";

            string sqlCmd = "SELECT r.jobid, j.jobnavn, j.jobnr, j.jobstatus, k.kkundenavn, k.kkundenr, k.kinit, SUM(r.timer) AS restimer FROM ressourcer_md AS r " +
                            " LEFT JOIN job AS j ON (j.id = r.jobid) " +
                            " LEFT JOIN aktiviteter AS a ON (a.id = r.aktid) " +
                            " LEFT JOIN kunder AS k ON (k.kid = j.jobknr)" +
                            " WHERE r.medid = " + mid + " AND r.aktid <> 0 AND k.kkundenavn <> '' AND a.navn IS NOT NULL AND j.jobstatus = 1 GROUP BY r.jobid ";


            //OR j.jobstatus = 3

            if (sortColumns.Trim() == "")
                 sqlCmd += "ORDER BY kkundenavn, jobnavn";
             else
                 sqlCmd += "ORDER BY " + sortColumns;


             //sqlCmd += " LIMIT 10 ";

          

             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_JobCustomerOrigin> jobCustomers = new List<outz_JobCustomerOrigin>();
             int count = 0;

             try
             {
                 conn.Open();
                 
                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         //if (jobCustomers.Count < maxRecords)
                         //{
                             jobCustomers.Add(GetJobCustomerFromReader(reader));
                         //}
                         //else
                         //    cmd.Cancel();
                     }

                     count++;
                 }

             }
             catch (OdbcException ex)
             {     
                     throw new Exception("odbcException: "+ex.Message);                 
             }
             catch (Exception ex)
             {
                 throw new Exception("Problems in jobCustomer data factry => select all active jobCustomers: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }

             return jobCustomers;
         }

         private outz_JobCustomerOrigin GetJobCustomerFromReader(OdbcDataReader reader)
         {
             outz_JobCustomerOrigin jobCustomer = new outz_JobCustomerOrigin();

             jobCustomer.JobId = reader.GetInt32(0);

             if (reader.GetValue(1) != DBNull.Value)
                 jobCustomer.JobName = reader.GetString(1);

             if (reader.GetValue(2) != DBNull.Value)
                 jobCustomer.JobNo = reader.GetString(2);

            if (reader.GetValue(3) != DBNull.Value)
                jobCustomer.JobStatus = Int32.Parse(reader.GetValue(3).ToString());

            if (reader.GetValue(4) != DBNull.Value)
                jobCustomer.CustomerName = reader.GetString(4);

            if (reader.GetValue(5) != DBNull.Value)
                jobCustomer.CustomerNo = reader.GetString(5);

            if (reader.GetValue(6) != DBNull.Value)
                jobCustomer.CustomerInit = reader.GetString(6);

            return jobCustomer;
         }
    }
}
