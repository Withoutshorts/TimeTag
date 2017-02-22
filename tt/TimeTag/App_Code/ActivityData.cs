using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace TimeTag
{
    //Activity data factory
    public class ActivityData
    {
        private string _connectionString;

        public ActivityData(string lto, bool isNewDb)
        {
             if (lto.ToLower() == "outz")
                 lto = "intranet";

             _connectionString = DBHandler.Initialize(lto, isNewDb);
         }

         /// <summary>
         /// Select all Activitys
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum Activitys</param>
         /// <param name="maxRecords">maximum Activitys</param>
         /// <returns></returns>
         public List<outz_Activity> SelectAllActivitys(string sortColumns, int startRecord, int maxRecords, bool positive)
         {
             string sqlCmd = "SELECT aktid FROM timereg_usejob";

             if (sortColumns.Trim() == "")
                 sqlCmd += " ORDER BY aktid";
             else
                 sqlCmd += " ORDER BY " + sortColumns;

             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_Activity> Activities = new List<outz_Activity>();
             int count = 0;

             try
             {
                 conn.Open();

                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         //if (Activities.Count < maxRecords)
                             Activities.Add(GetActivityFromReader(reader));
                         //else
                         //    cmd.Cancel();
                     }

                     count++;
                 }

             }
             catch (Exception ex)
             {
                 throw new Exception("Problems in activity data factory => select all activities: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }

             return Activities;
         }

         /// <summary>
         /// Select all Activitys
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum Activitys</param>
         /// <param name="maxRecords">maximum Activitys</param>
         /// <returns></returns>
         public List<outz_Activity> SelectAllActivitysOnlyPositive(string sortColumns, int startRecord, int maxRecords, int medid, int jobid)
         {

             string jobSQLkri = "";

             if (jobid != 0 && jobid != null)
             {
                 jobSQLkri = "tu.jobid = " + jobid + "";
             }
             else {

                 jobSQLkri = "tu.jobid = 0"; //Tom liste
             }

             string sqlCmd = "SELECT a.id, a.navn as aktnavn, " +
                                " a.projektgruppe1, a.projektgruppe2, a.projektgruppe3, a.projektgruppe4, a.projektgruppe5, " +
                                " a.projektgruppe6, a.projektgruppe7, a.projektgruppe8, a.projektgruppe9, a.projektgruppe10, a.beskrivelse, tu.jobid " +
                                " FROM timereg_usejob AS tu " +
                                " LEFT JOIN aktiviteter as a " +
                                " on (a.job = tu.jobid) and a.aktstatus = 1" +
                                " LEFT JOIN akt_typer AS at ON (at.aty_id = a.fakturerbar) " +
                                " where " + jobSQLkri + " and tu.medarb = " + medid + " and at.aty_on = 1 AND at.aty_on_realhours = 1 AND at.aty_hide_on_treg = 0 and a.aktstatus = 1"; // and tu.aktid <> 0";


             sqlCmd += " GROUP BY a.id ";

             if (sortColumns.Trim() == "")
                 sqlCmd += " ORDER BY sortorder";
             else
                 sqlCmd += " ORDER BY " + sortColumns;


             //sqlCmd += " LIMIT 20";

             //outz_Log.LogError("A: " + sqlCmd);

             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_Activity> Activities = new List<outz_Activity>();
             
          
             int count = 0;

             try
             {
                 conn.Open();

                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         //if (Activities.Count < maxRecords)
                             Activities.Add(GetActivityFromReader(reader));
                         //else
                         //    cmd.Cancel();
                     }

                     count++;
                 }

             }
             catch (OdbcException ex)
             {
                 throw new Exception("odbcException: " + ex.Message);
             }
             catch (Exception ex)
             {
                 throw new Exception("Problems in activity data factory => select all active activities positive only: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }


            
             return Activities;
         }

         /// <summary>
         /// Select all Activitys
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum Activitys</param>
         /// <param name="maxRecords">maximum Activitys</param>
         /// <returns></returns>
         public List<outz_Activity> SelectAllActivitysNotPositive(string sortColumns, int startRecord, int maxRecords, int medid, int jobid)
         {
             string sqlCmd = "SELECT DISTINCT a.id AS aid, navn AS aktnavn, projektgruppe1, "+
                 "projektgruppe2, projektgruppe3,projektgruppe4, projektgruppe5, projektgruppe6,"+
                 " projektgruppe7, projektgruppe8, projektgruppe9, projektgruppe10, a.beskrivelse FROM aktiviteter AS a" +
                 " LEFT JOIN akt_typer AS at ON (at.aty_id = a.fakturerbar) " +
                 " WHERE a.aktstatus = 1 and a.job = " + jobid + " and at.aty_on = 1 AND at.aty_on_realhours = 1 AND at.aty_hide_on_treg = 0";


             sqlCmd += " GROUP BY a.id ";

             if (sortColumns.Trim() == "")
                 sqlCmd += " ORDER BY sortorder";
             else
                 sqlCmd += " ORDER BY " + sortColumns;


             sqlCmd += " LIMIT 20";

             //outz_Log.LogError("B: " + sqlCmd);

             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_Activity> Activities = new List<outz_Activity>();
             int count = 0;

             try
             {
                 conn.Open();

                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         //if (Activities.Count < maxRecords)
                             Activities.Add(GetActivityFromReader(reader));
                         //else
                         //    cmd.Cancel();
                     }

                     count++;
                 }

             }
             catch (OdbcException ex)
             {
                 throw new Exception("odbcException: " + ex.Message);
             }
             catch (Exception ex)
             {
                 throw new Exception("Problems in activity data factory => select all active activities positive only: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }

             return Activities;
         }

         private outz_Activity GetActivityFromReader(OdbcDataReader reader)
         {
             outz_Activity activity = new outz_Activity();

             activity.Id = reader.GetInt32(0);

             if (reader.GetValue(1) != DBNull.Value)
                 activity.Name = reader.GetString(1);

             if (reader.GetValue(2) != DBNull.Value)
                 activity.ProjectGroup1 = reader.GetInt32(2);
             if (reader.GetValue(3) != DBNull.Value)
                 activity.ProjectGroup2 = reader.GetInt32(3);
             if (reader.GetValue(4) != DBNull.Value)
                 activity.ProjectGroup3 = reader.GetInt32(4);
             if (reader.GetValue(5) != DBNull.Value)
                 activity.ProjectGroup4 = reader.GetInt32(5);
             if (reader.GetValue(6) != DBNull.Value)
                 activity.ProjectGroup5 = reader.GetInt32(6);
             if (reader.GetValue(7) != DBNull.Value)
                 activity.ProjectGroup6 = reader.GetInt32(7);
             if (reader.GetValue(8) != DBNull.Value)
                 activity.ProjectGroup7 = reader.GetInt32(8);
             if (reader.GetValue(9) != DBNull.Value)
                 activity.ProjectGroup8 = reader.GetInt32(9);
             if (reader.GetValue(10) != DBNull.Value)
                 activity.ProjectGroup9 = reader.GetInt32(10);
             if (reader.GetValue(11) != DBNull.Value)
                 activity.ProjectGroup10 = reader.GetInt32(11);
            if (reader.GetValue(12) != DBNull.Value)
                activity.Description = System.Web.HttpUtility.HtmlDecode(reader.GetString(12));

            return activity;
         }
    }
}
