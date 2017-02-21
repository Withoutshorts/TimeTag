using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TimeTag
{
    public class DBHandler
    {
        public DBHandler()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        /// <summary>
        /// Get connection string by name
        /// </summary>
        /// <param name="connName">Name of connection string, default: 'ParkXManagerConnectionString'</param>
        /// <returns>connection string</returns>
        public static string Initialize(string lto, bool isNewDb)
        {
            string connectionString;

            if (lto.ToLower() == "outz")
                lto = "intranet";

            if (isNewDb)
            {
                connectionString = "Driver={MySQL ODBC 3.51 Driver};Server=194.150.108.154;User=to_outzource2;Password=SKba200473;Database=timeout_" + lto + ";";
            }
            else
            {
                connectionString = "driver={MySQL ODBC 3.51 Driver};server=195.189.130.210;Port=3306;uid=outzource;pwd=SKba200473;database=timeout_" + lto + ";";
            }



            //connectionString = "driver={MySQL ODBC 3.51 Driver};server=localhost; Port=3306; " +
            //    "uid=root;pwd=;database=timeout_intranet;";
            //connectionString = "Driver={MySQL ODBC 3.51 Driver};Server=194.150.108.154;User=to_outzource2;Password=SKba200473;Database=timeout_bf;";
            //outz_Log.LogError("Conn SK: " + connectionString);

            return connectionString;
        }
    }
}
