using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace TimeTag.App_Code
{
    public class HoursService
    {
        private string _connectionString;

        public HoursService(string lto, bool isNewDb)
        {
            if (lto.ToLower() == "outz")
                lto = "intranet";

            _connectionString = DBHandler.Initialize(lto, isNewDb);
        }
        public decimal GetReportedHours(DateTime date, string mid)
        {
            string sqlCmd = string.Format("SELECT SUM(timer) FROM timer WHERE tmnr = '{0}' AND tdato = '{1}' AND (tfaktim = 1 OR tfaktim = 2 OR tfaktim = 6 OR tfaktim = 14 OR tfaktim = 20 OR tfaktim = 21)", mid, date.ToString("yyyy-MM-dd"));
            decimal result = 0m;
            OdbcConnection conn = new OdbcConnection(_connectionString);
            OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
            OdbcDataReader reader = null;
            try
            {
                conn.Open();
                var hours = cmd.ExecuteScalar();
                if (hours != DBNull.Value)
                {
                    result = Convert.ToDecimal(hours);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problems in hours data factory => select reported hours: " + ex.Message);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }

            sqlCmd = string.Format("SELECT SUM(timer) FROM timer_import_temp WHERE medarbejderid = '{0}' AND tdato = '{1}' AND overfort = 0", mid, date.ToString("yyyy-MM-dd"));
            conn = new OdbcConnection(_connectionString);
            cmd = new OdbcCommand(sqlCmd, conn);
            reader = null;
            try
            {
                conn.Open();
                var hours = cmd.ExecuteScalar();
                if (hours != DBNull.Value)
                {
                    result += Convert.ToDecimal(hours);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problems in hours data factory => select reported hours: " + ex.Message);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            return result;
        }

        public decimal GetReportedHoursByActivity(string mid, int activityId, DateTime reportDate)
        {
            string sqlCmd = string.Format("SELECT COALESCE(SUM(timer), 0) AS timerbrugt FROM timer WHERE taktivitetid = '{0}' AND tmnr = '{1}' AND MONTH(tdato) = '{2}' GROUP BY taktivitetid", activityId, mid, reportDate.Month);
            decimal result = 0m;
            OdbcConnection conn = new OdbcConnection(_connectionString);
            OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
            OdbcDataReader reader = null;
            try
            {
                conn.Open();
                var hours = cmd.ExecuteScalar();
                if (hours != DBNull.Value)
                {
                    result = Convert.ToDecimal(hours);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Problems in hours data factory => select reported hours: " + ex.Message);
            }
            finally
            {
                if (reader != null) { reader.Close(); }
                conn.Close();
            }
            return result;
        }
    }
}
