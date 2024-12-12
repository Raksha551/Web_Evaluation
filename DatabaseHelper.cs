using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace ASP_Evaluation_Task
{
    public class DatabaseHelper
    {
        private string connectionString;

        public DatabaseHelper()
        {
            // Retrieve the connection string from Web.config
            connectionString = WebConfigurationManager.ConnectionStrings["DBCS2"].ConnectionString;

        }

        // Method to retrieve downtime data
        public DataTable GetDowntimeData(string startTime, string endTime, string machineID = "", string downID = "", string matrixType = "DLoss_By_Catagory", string plantID = "", string exclude = "", string groupID = "")
        {
            DataTable dt = new DataTable();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("s_GetSONA_ShiftAgg_DowntimeMatrix", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Add parameters to the command
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    cmd.Parameters.AddWithValue("@MachineID", machineID);
                    cmd.Parameters.AddWithValue("@DownID", downID);
                    cmd.Parameters.AddWithValue("@MatrixType", matrixType);
                    cmd.Parameters.AddWithValue("@PlantID", plantID);
                    cmd.Parameters.AddWithValue("@Exclude", exclude);
                    cmd.Parameters.AddWithValue("@Groupid", groupID);

                    try
                    {
                        conn.Open();
                        using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                        {
                            da.Fill(dt);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Handle or log exceptions
                        throw new Exception("Error retrieving downtime data: " + ex.Message);
                    }
                }
            }

            return dt;
        }

        public DataTable GetRandomDataScreen(string dateTime = "", string param = "")
        {
            DataTable dt = new DataTable();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("S_DayWiseTargetAndonSaveAndView_KTA", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Date", dateTime);
                    cmd.Parameters.AddWithValue("@Param", param);

                    con.Open();
                    SqlDataAdapter ad = new SqlDataAdapter(cmd);
                    ad.Fill(dt);
                }

            }
            return dt;
        }


    }
}
