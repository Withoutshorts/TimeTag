using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace TimeTag.App_Code
{
    public class ResourceDataProvider
    {
        private string _connectionString;

        public ResourceDataProvider(string lto, bool isNewDb)
        {
            _connectionString = DBHandler.Initialize(lto, isNewDb);
        }

        public decimal GetResourceHours(string mid, int jobId, int activityId, DateTime date)
        {
            var result = (decimal?)null;
            string sqlCmd = "SELECT SUM(timer) AS restimer " +
                            "FROM ressourcer_md " +
                            "WHERE (jobid = " + jobId.ToString() +
                            " AND aktid = " + activityId +
                            " AND medid = " + mid + ") " +
                            " AND (md = " + date.Month.ToString() + " AND aar = " + date.Year.ToString() + ")" +
                            " GROUP BY aktid, medid";

            var conn = new OdbcConnection(_connectionString);

            OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
            OdbcDataReader reader = null;

            try
            {
                conn.Open();

                var fieldValue = cmd.ExecuteScalar();
                if (fieldValue != null)
                {
                    result = Convert.ToDecimal(fieldValue);
                }
            }
            catch (OdbcException ex)
            {
                throw new Exception("odbcException: " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Problems in ResourceDataProvider > GetResourceHours: " + ex.Message);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            
            return result.HasValue ? result.Value : 0m;
        }
    }
}
