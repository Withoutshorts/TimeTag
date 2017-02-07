using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class ProGroupRelationsData
    {
        private string _connectionString;

        public ProGroupRelationsData(string lto, bool isNewDb)
        {
             _connectionString = DBHandler.Initialize(lto, isNewDb);
         }

         /// <summary>
         /// Select all ProGroupRelations
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum ProGroupRelations</param>
         /// <param name="maxRecords">maximum ProGroupRelations</param>
         /// <returns></returns>
         public List<outz_ProGroupRelations> SelectAllProGroupRelations(string sortColumns, int startRecord, int maxRecords)
         {
             string sqlCmd = "select ProjektgruppeId, MedarbejderId from progrupperelationer GROUP BY ProjektgruppeId";

             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_ProGroupRelations> ProGroupRelations = new List<outz_ProGroupRelations>();
             int count = 0;

             try
             {
                 conn.Open();

                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         if (ProGroupRelations.Count < maxRecords)
                             ProGroupRelations.Add(GetProGroupRelationFromReader(reader));
                         else
                             cmd.Cancel();
                     }

                     count++;
                 }

             }
             catch (Exception ex)
             {
                 throw new Exception("Problems in ProGroupRelation data factory => select all ProGroupRelations: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }

             return ProGroupRelations;
         }

         /// <summary>
         /// Select all ProGroupRelations
         /// </summary>
         /// <param name="sortColumns">column names or column names plus desc and LIMIT 50</param>
         /// <param name="startRecord">minimum ProGroupRelations</param>
         /// <param name="maxRecords">maximum ProGroupRelations</param>
         /// <returns></returns>
         public List<outz_ProGroupRelations> GetProGroupRelationsByMid(int startRecord, int maxRecords, int mid)
         {
             string sqlCmd = "select ProjektgruppeId, MedarbejderId from progrupperelationer where MedarbejderId = " + mid + " GROUP BY ProjektgruppeId";

             OdbcConnection conn = new OdbcConnection(_connectionString);

             OdbcCommand cmd = new OdbcCommand(sqlCmd, conn);
             OdbcDataReader reader = null;

             List<outz_ProGroupRelations> ProGroupRelations = new List<outz_ProGroupRelations>();
             int count = 0;

             try
             {
                 conn.Open();
                 
                 reader = cmd.ExecuteReader();

                 while (reader.Read())
                 {
                     if (count >= startRecord)
                     {
                         if (ProGroupRelations.Count < maxRecords)
                             ProGroupRelations.Add(GetProGroupRelationFromReader(reader));
                         else
                             cmd.Cancel();
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
                 throw new Exception("Problems in ProGroupRelation data factry => select all active ProGroupRelations: " + ex.Message);
             }
             finally
             {
                 if (reader != null) { reader.Close(); }
                 conn.Close();
             }

             return ProGroupRelations;
         }

         private outz_ProGroupRelations GetProGroupRelationFromReader(OdbcDataReader reader)
         {
             outz_ProGroupRelations ProGroupRelation = new outz_ProGroupRelations();

             if(reader.GetValue(0) != DBNull.Value)
                 ProGroupRelation.ProjectGroupId = int.Parse(reader.GetValue(0).ToString());

             if (reader.GetValue(1) != DBNull.Value)
                 ProGroupRelation.MedId = reader.GetValue(0).ToString();
                        
             return ProGroupRelation;
         }
    }
}
