using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Web.Configuration;
using System.Web.UI.WebControls;

namespace ASP_Evaluation_Task
{
    public class DatabaseHelper
    {
        public class ColumnNames_111
        {
            public string Date { get; set; }
            public string Shift { get; set; } = string.Empty;
            public string ComponentId { get; set; }
            public string SerialNo { get; set; }
            public List<childClass> listviewdata { get; set; } = new List<childClass>();
            public string SpindleLoad { get; set; }
            public string Result { get; set; }
            public string Remarks { get; set; }
            public int tdColSpan { get; set; } = 1;
            public int RowSpan { get; set; } = 1;
            public bool tdVisible { get; set; } = true;
        }

        public class childClass
        {
            public string CharacteristicValue { get; set; }
            public string backColor { get; set; }
            public int tdColSpan { get; set; } = 1;
        }
        private static string connectionString;
        private static string ConnectionString;
        DataTable dt = new DataTable();

        public DatabaseHelper()
        {
            connectionString = WebConfigurationManager.ConnectionStrings["DBCS2"].ConnectionString;
            ConnectionString = WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        }
        public DataTable GetDowntimeData(string startTime, string endTime, string machineID = "", string downID = "", string matrixType = "DLoss_By_Catagory", string plantID = "", string exclude = "", string groupID = "")
        {
            
            SqlConnection conn = new SqlConnection(connectionString);
            try
            {
                using (SqlCommand cmd = new SqlCommand("s_GetSONA_ShiftAgg_DowntimeMatrix", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@StartTime", startTime);
                    cmd.Parameters.AddWithValue("@EndTime", endTime);
                    cmd.Parameters.AddWithValue("@MachineID", machineID);
                    cmd.Parameters.AddWithValue("@DownID", downID);
                    cmd.Parameters.AddWithValue("@MatrixType", matrixType);
                    cmd.Parameters.AddWithValue("@PlantID", plantID);
                    cmd.Parameters.AddWithValue("@Exclude", exclude);
                    cmd.Parameters.AddWithValue("@Groupid", groupID);


                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error retrieving downtime data: " + ex.Message);
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return dt;
        }
        public DataTable GetRandomDataScreen(string dateTime = "", string param = "")
        {
            SqlConnection con = new SqlConnection(connectionString);
            try
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
            catch (Exception ex)
            {
                throw new Exception("Error retrieving downtime data: " + ex.Message);
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
            }
            return dt;
        }
        public static List<string> Populatemachineid()
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            List<string> list = new List<string>();
            SqlDataReader reader = null;
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT MachineID FROM Machineinformation", con))
                {
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["MachineID"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) { reader.Close(); reader.Dispose(); }
            }
            return list;
        }
        public static List<string> Populatecomponent(string machineID)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataReader reader = null;
            List<string> list = new List<string>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT ComponentID FROM componentoperationpricing WHERE MachineID = @MachineID", con))
                {
                    cmd.Parameters.AddWithValue("@MachineID", machineID);
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["ComponentID"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if (reader != null) { reader.Close(); reader.Dispose(); }
            }
            return list;
        }
        public static  List<string> PopulateCharacteristicdropdown(string machineID, string componentID, string operationNo)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataReader reader = null;
            List<string> list = new List<string>();
            try
            {  
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT CharacteristicCode FROM SPC_Characteristic WHERE MachineID = @MachineID AND ComponentID = @ComponentID AND OperationNo = @OperationNo", con))
                {
                    cmd.Parameters.AddWithValue("@MachineID", machineID);
                    cmd.Parameters.AddWithValue("@ComponentID", componentID);
                    cmd.Parameters.AddWithValue("@OperationNo", operationNo);
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                    {


                        while (reader.Read())
                        {
                            list.Add(reader["CharacteristicCode"].ToString());
                        }
                    }
                }  
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PopulateCharacteristicDropDown: {ex.Message}");
            }
            finally
            {
                if(con!= null) { con.Close();con.Dispose(); }
                if(reader != null) { reader.Close(); reader.Dispose(); }
            }
            return list;
        }

        public static List<string> PopulateOperationdropdown(string componentID)
        {
            SqlConnection con = new SqlConnection(ConnectionString);
            SqlDataReader reader = null;
            List<string> list = new List<string>();
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT OperationNo FROM componentoperationpricing WHERE ComponentID = @ComponentID", con))
                {
                    cmd.Parameters.AddWithValue("@ComponentID", componentID);
                    con.Open();
                    using (reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(reader["OperationNo"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PopulateCharacteristicDropDown: {ex.Message}");
            }
            finally
            {
                if (con != null) { con.Close(); con.Dispose(); }
                if(reader != null) { reader.Close();reader.Dispose(); }
            }
                return list;
          
        }

        public DataTable fetchDBdata(string machineID, string componentID, string operationNo, string serialNo, string[] status, string fromDate, string toDate, string[] dimension)
        {
            SqlConnection conn = new SqlConnection(ConnectionString);

            try
            {
                using (SqlCommand cmd = new SqlCommand("S_GetSlnoWiseSPCReport_Bajaj", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Use parameters for all user inputs, including dates
                    cmd.Parameters.AddWithValue("@MachineID", machineID);
                    cmd.Parameters.AddWithValue("@ComponentID", componentID);
                    cmd.Parameters.AddWithValue("@OperationNo", operationNo);
                    cmd.Parameters.AddWithValue("@SerialNo", serialNo);
                    cmd.Parameters.AddWithValue("@Dimension", string.Join(",", dimension));
                    cmd.Parameters.AddWithValue("@Status", string.Join(",", status));
                    cmd.Parameters.AddWithValue("@StartDate", fromDate);
                    cmd.Parameters.AddWithValue("@EndDate", toDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PopulateCharacteristicDropDown: {ex.Message}");
            }
            finally
            {
                if (conn != null) { conn.Close(); conn.Dispose(); }
            }
            return dt;
        }
    }
}
