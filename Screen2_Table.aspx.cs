using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using static System.Net.Mime.MediaTypeNames;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Runtime.Remoting.Messaging;
using System.ComponentModel;



namespace ASP_Evaluation_Task
{



    public partial class Screen2_Table : System.Web.UI.Page
    {// A dictionary to hold the dynamic headers and their corresponding values
        private Dictionary<string, List<string>> dynamicHeaders = new Dictionary<string, List<string>>();
        private List<string> columnOrder;
        private Dictionary<string, string> dimensionColumns1 = new Dictionary<string, string>
{
    { "Fine Boring by Renishaw$30.81_30.87@Top", "Top" },
    { "Fine Boring by Renishaw$30.81_30.87@Bottom", "Bottom" },
    { "Input Size by Renishaw$20.82_20.88@Top", "Top" },
    { "Input Size by Renishaw$20.81_20.87@Bottom", "Bottom" },
    { "Mechanical Size by Versa$19.81_19.87", "" }
};
        private List<string> dimensionColumns = new List<string>
{
    "Fine Boring by Renishaw$30.81_30.87@Top",
    "Fine Boring by Renishaw$30.81_30.87@Bottom",
    "Input Size by Renishaw$20.82_20.88@Top",
    "Input Size by Renishaw$20.81_20.87@Bottom",
    "Mechanical Size by Versa$19.81_19.87"
};

        private string ConnectionString => WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

        public class ColumnNames
        {
            public DateTime Date { get; set; }
            public string Shift { get; set; } = string.Empty;
            public string ComponentId { get; set; }
            public string SerialNo { get; set; }
            public List<ColumnNames> listviewdata { get; set; } = new List<ColumnNames>();
            public string RenderData { get; set; }
            public string SpindleLoad { get; set; }
            public string Result { get; set; }
            public string Remarks { get; set; }
            public bool headervisible { get; set; }

        }
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopulateMachineDropDown();
                SetDefaultPageloadData();
                PopulateMachineDropDown();
                // PopulateCharacteristicDropDown();
                // SetDynamicHeaders();
            }
            //SetDynamicHeaders();
        }
        private void PopulateMachineDropDown()
        {
            machineDroplist.Items.Clear();
            List<string> list = new List<string>();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT MachineID FROM Machineinformation", con);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    list.Add(reader["MachineID"].ToString());
                }
            }
            machineDroplist.DataSource = list;
            machineDroplist.DataBind();
        }

        protected void MachineDroplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMachine = machineDroplist.SelectedValue;
            PopulateComponentDropDown(selectedMachine);
            OpDroplist.Items.Clear(); // Clear operations as machine changes.
            ddlCharacteristic.Items.Clear(); // Clear characteristics as machine changes.
        }

        private void PopulateComponentDropDown(string machineID)
        {
            componentDroplist.Items.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT ComponentID FROM componentoperationpricing WHERE MachineID = @MachineID", con);
                cmd.Parameters.AddWithValue("@MachineID", machineID);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    componentDroplist.Items.Add(reader["ComponentID"].ToString());
                }
            }
        }

        protected void ComponentDroplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedComponent = componentDroplist.SelectedValue;
            PopulateOperationDropDown(selectedComponent);
            string selectedMachine = machineDroplist.SelectedValue;
            PopulateCharacteristicDropDown(selectedMachine, selectedComponent);
        }

        private void PopulateOperationDropDown(string componentID)
        {
            OpDroplist.Items.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT OperationNo FROM componentoperationpricing WHERE ComponentID = @ComponentID", con);
                cmd.Parameters.AddWithValue("@ComponentID", componentID);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    OpDroplist.Items.Add(reader["OperationNo"].ToString());
                }
            }
        }

        private void PopulateCharacteristicDropDown(string machineID, string componentID)
        {
            ddlCharacteristic.Items.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT CharacteristicCode FROM SPC_Characteristic WHERE MachineID = @MachineID AND ComponentID = @ComponentID", con);
                cmd.Parameters.AddWithValue("@MachineID", machineID);
                cmd.Parameters.AddWithValue("@ComponentID", componentID);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    ddlCharacteristic.Items.Add(reader["CharacteristicCode"].ToString());
                }
            }
        }
        private void SetDefaultPageloadData()
        {
            ddlStatus.Items.Clear();
            ddlStatus.Items.Add("Ok");
            ddlStatus.Items.Add("Rework");
            ddlStatus.Items.Add("Rejected");
            ddlStatus.Items.Add("Empty");

            txtFromDate.Text = "";
            txtToDate.Text = "";
            //txtFromDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
            //txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        protected void ViewButton_Click(object sender, EventArgs e)
        {
            string machineID = machineDroplist.SelectedValue;
            string componentID = componentDroplist.SelectedValue;
            string operationNo = OpDroplist.SelectedValue;
            string serialNo = txtSerialNo.Text;
            //  string status = ddlStatus.SelectedValue;
            string fromDate = txtFromDate.Text;
            string toDate = txtToDate.Text;
            //  string dimension = ddlCharacteristic.SelectedValue;
            string status = Request.Form[ddlStatus.UniqueID]; // Comma-separated values
            string[] selectedStatuses = status?.Split(',') ?? new string[] { };

            string characteristic = Request.Form[ddlCharacteristic.UniqueID]; // Comma-separated values
            string[] selectedCharacteristics = characteristic?.Split(',') ?? new string[] { };

            // Validate inputs
            if (string.IsNullOrEmpty(machineID) || string.IsNullOrEmpty(componentID))
            {
              //  lblMessage.Text = "Please select a valid Machine and Component.";
              //  lblMessage.Visible = true;
                return;
            }

            // Ensure the fromDate and toDate are in the correct format
            string formattedFromDate = ConvertToCustomDateFormat(fromDate);
            string formattedToDate = ConvertToCustomDateFormat(toDate);

            // Log the formatted dates for debugging
            System.Diagnostics.Debug.WriteLine($"Formatted From Date: {formattedFromDate}, Formatted To Date: {formattedToDate}");

            // Fetch the filtered data
            DataTable resultTable = FetchReportData(machineID, componentID, operationNo, serialNo, selectedStatuses, formattedFromDate, formattedToDate, selectedCharacteristics);
            System.Diagnostics.Debug.WriteLine($"Fetched data: {resultTable.Rows.Count} rows");
            if (resultTable.Rows.Count > 0)
            {
                bool hasData = false;

                // Check if the DataTable has the 'SaveFlag' column
                if (resultTable.Columns.Contains("SaveFlag"))
                {
                    // If the 'SaveFlag' column exists, check its value
                    if (resultTable.Rows[0]["SaveFlag"].ToString() == "No records found!")
                    {
                      //  lblMessage.Text = "No data found for the given search criteria.";
                      //  lblMessage.Visible = true;
                        listview1.DataSource = null; // Clear any previous data
                        listview1.DataBind();
                    }
                    else
                    {
                        hasData = true;
                    }
                }
                else
                {
                    // If there's no 'SaveFlag' column, proceed with the data normally
                    hasData = true;
                }

                if (hasData)
                {

                    listview1.DataSource = null;
                    listview1.DataBind();
                    List<ColumnNames> list = new List<ColumnNames>();
                    List<ColumnNames> dynValues = new List<ColumnNames>();
                    ColumnNames data = null;
                    ColumnNames ColumnNamesdata = null;

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i == 0)
                            {
                                data = new ColumnNames();
                                data.Date = Convert.ToDateTime(dt.Rows[i]["Date"]);  // Assuming the column names match
                                data.Shift = dt.Rows[i]["Shift"].ToString();
                                data.ComponentId = dt.Rows[i]["ComponentID"].ToString();
                                data.SerialNo = dt.Rows[i]["SerialNo"].ToString();
                                data.SpindleLoad = dt.Rows[i]["SpindleLoad"].ToString();
                                data.Result = dt.Rows[i]["Result"].ToString();
                                data.Remarks = dt.Rows[i]["Remarks"].ToString();
                                data.headervisible = true;
                                dynValues = new List<ColumnNames>();
                                for (int j = 10; j < dt.Columns.Count; j++)
                                {
                                    ColumnNamesdata = new ColumnNames();
                                    ColumnNamesdata.RenderData = dt.Columns[j].ColumnName;
                                    dynValues.Add(ColumnNamesdata);
                                }                                //RenderColumn.HeaderVisibility = false;
                                                                 //RenderData.HeaderVisibility = false;

                            }
                            list.Add(data);
                        }

                    }
                   // lblMessage.Visible = false;
                    // Assuming you already have the DataTable 'dt'
                    listview1.DataSource = resultTable;
                    listview1.DataBind();
                }
            }
            else
            {
               // lblMessage.Text = "No data found for the given search criteria.";
              //  lblMessage.Visible = true;
                listview1.DataSource = null; // Clear any previous data
                listview1.DataBind();
            }

        }

        private string ConvertToCustomDateFormat(string dateString)
        {
            // Log the incoming date string to verify its format
            System.Diagnostics.Debug.WriteLine($"ConvertToCustomDateFormat called with dateString: {dateString}");
            if (dateString.Length == 16) // Format yyyy-MM-ddTHH:mm (no seconds)
            {
                // Append ":00" to include seconds
                dateString += ":00";
            }

            DateTime parsedDate;

            // Try parsing the date in the format received from the DateTimeLocal input (yyyy-MM-ddTHH:mm:ss)
            if (DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                // Log the successfully parsed date
                System.Diagnostics.Debug.WriteLine($"Parsed Date: {parsedDate.ToString()}");

                // Return the date in dd:MM:yyyy HH:mm:ss format
                return parsedDate.ToString("dd:MM:yyyy HH:mm:ss");
            }
            else
            {
                // Log if the parsing fails
                System.Diagnostics.Debug.WriteLine("Date parsing failed.");

                // If the date format is invalid, return an empty string (or handle error as needed)
                return string.Empty;
            }
        }
        DataTable dt = new DataTable();
        private DataTable FetchReportData(string machineID, string componentID, string operationNo, string serialNo, string[] status, string fromDate, string toDate, string[] dimension)
        {


            // Ensure the dates are parsed to correct format
            DateTime startDate;
            DateTime endDate;

            if (!DateTime.TryParseExact(fromDate, "dd:MM:yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
               // lblMessage.Text = "Invalid From Date format. Please use the format dd:MM:yyyy HH:mm:ss.";
              //  lblMessage.Visible = true;
                return dt;
            }

            if (!DateTime.TryParseExact(toDate, "dd:MM:yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
               // lblMessage.Text = "Invalid To Date format. Please use the format dd:MM:yyyy HH:mm:ss.";
              //  lblMessage.Visible = true;
                return dt;
            }
            serialNo = "%" + serialNo + "%";   // Add '%' for partial match in SQL


            serialNo = serialNo.Trim(); // Remove wildcard for testing
            machineID = machineID.Trim();
            componentID = componentID.Trim();
            // status = status.Trim();
            //dimension = dimension.Trim();

            System.Diagnostics.Debug.WriteLine("binding starts");


            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("S_GetSlnoWiseSPCReport_Bajaj", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Use parameters for all user inputs, including dates
                    cmd.Parameters.AddWithValue("@MachineID", machineID);
                    cmd.Parameters.AddWithValue("@ComponentID", componentID);
                    cmd.Parameters.AddWithValue("@OperationNo", operationNo);
                    cmd.Parameters.AddWithValue("@SerialNo", serialNo);
                    cmd.Parameters.AddWithValue("@Dimension", string.Join(",", dimension));
                    cmd.Parameters.AddWithValue("@Status", string.Join(",", status));
                    cmd.Parameters.AddWithValue("@StartDate", startDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            System.Diagnostics.Debug.WriteLine("Rows in DataTable: " + dt.Rows.Count);
            foreach (DataRow row in dt.Rows)
            {
                foreach (DataColumn column in dt.Columns)
                {
                    System.Diagnostics.Debug.WriteLine($"{column.ColumnName}: {row[column]}");
                }
            }

            return dt;
        }
        public class DimensionColumn
        {
            public string Key { get; set; }
            public string DisplayName { get; set; }
        }
        protected void listview1_DataBound(object sender, EventArgs e)
        {
            if (listview1.DataSource is DataTable resultTable)
            {
                // Filter dynamic headers that exist in the resultTable
                var dynamicHeaders = dimensionColumns
                    .Where(columnName => resultTable.Columns.Contains(columnName))
                    .ToList();

                // Find the dynamic headers ListView
                var dynamicHeadersListView = listview1.FindControl("phDynamicHeaders") as ListView;
                if (dynamicHeadersListView != null)
                {
                    dynamicHeadersListView.DataSource = dynamicHeaders;
                    dynamicHeadersListView.DataBind();
                }
            }
        }


        protected void listview1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var rowView = e.Item.DataItem as DataRowView;
                if (rowView != null)
                {
                    // Extract dynamic data for this row
                    var dynamicData = dimensionColumns
                        .Where(columnName => rowView.DataView.Table.Columns.Contains(columnName))
                        .Select(columnName => new { Value = rowView[columnName]?.ToString() ?? string.Empty })
                        .ToList();

                    // Bind dynamic data to the ListView for this row
                    var dynamicDataListView = e.Item.FindControl("phDynamicData") as ListView;
                    if (dynamicDataListView != null)
                    {
                        dynamicDataListView.DataSource = dynamicData;
                        dynamicDataListView.DataBind();
                    }
                }
            }
        }

        protected void phDynamicHeaders_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var columnName = e.Item.DataItem as string;
                if (!string.IsNullOrEmpty(columnName))
                {
                    var literalHeader = e.Item.FindControl("headerPlaceHolder") as Literal;
                    if (literalHeader != null)
                    {
                        literalHeader.Text = columnName; // Set the column name as the header
                    }
                }
            }
        }






        protected void phDynamicData_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = e.Item.DataItem as dynamic;
                if (dataItem != null)
                {
                    var literalData = e.Item.FindControl("DataPlaceHolder") as Literal;
                    if (literalData != null)
                    {
                        literalData.Text = dataItem.Value; // Set the dynamic cell value
                    }
                }
            }


         if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                var dataItem = ((ListViewDataItem)e.Item).DataItem;
                var properties = dataItem.GetType().GetProperties();

                foreach (var property in properties)
                {
                    string columnName = property.Name;
                    if (dimensionColumns.Contains(columnName))
                    {
                        // Get the value dynamically
                        var valueObj = property.GetValue(dataItem);
                        if (valueObj == null) continue;

                        double value;
                        if (!double.TryParse(valueObj.ToString(), out value)) continue;

                        // Determine the color based on column rules
                        string style = "color: red;"; // Default to red
                        if (columnName == "Fine Boring by Renishaw$30.81_30.87@Top" && value >= 30.81)
                        {
                            style = "color: green;";
                        }
                        else if (columnName == "Fine Boring by Renishaw$30.81_30.87@Bottom" && value <= 30.87)
                        {
                            style = "color: green;";
                        }
                        else if (columnName == "Input Size by Renishaw$20.82_20.88@Top" && value >= 20.82)
                        {
                            style = "color: green;";
                        }
                        else if (columnName == "Input Size by Renishaw$20.81_20.87@Bottom" && value <= 20.87)
                        {
                            style = "color: green;";
                        }
                        else if (columnName == "Mechanical Size by Versa$19.81_19.87" && value >= 19.81 && value <= 19.87)
                        {
                            style = "color: green;";
                        }

                        // Apply the style to the data placeholder
                        var dataPlaceHolder = (Literal)e.Item.FindControl("DataPlaceHolder");
                        if (dataPlaceHolder != null)
                        {
                            dataPlaceHolder.Text = $"<span style='{style}'>{value}</span>";
                        }
                    }
                }
            }
        }






        //    private void AddDynamicHeaders(DataTable dt)
        //    {
        //        // Locate the placeholder for dynamic headers
        //        PlaceHolder phHeaders = (PlaceHolder)listview1.FindControl("phDynamicHeaders");
        //        if (phHeaders == null) return;

        //        phHeaders.Controls.Clear();

        //        // Group columns by main header
        //        var groupedHeaders = new Dictionary<string, List<string>>()
        //{
        //    { "Fine Boring by Renishaw", new List<string> { "Fine Boring by Renishaw$30.81_30.87@Top", "Fine Boring by Renishaw$30.81_30.87@Bottom" } },
        //    { "Input Size by Renishaw", new List<string> { "Input Size by Renishaw$20.82_20.88@Top", "Input Size by Renishaw$20.81_20.87@Bottom" } },
        //    { "Mechanical Size by Versa", new List<string> { "Mechanical Size by Versa$19.81_19.87" } }
        //};

        //        // Add the main headers with colspan
        //        foreach (var group in groupedHeaders)
        //        {
        //            var mainHeader = group.Key;
        //            var subHeaders = group.Value;

        //            // Only add a main header if at least one of the subheader columns exists in the DataTable
        //            if (subHeaders.Any(subHeader => dt.Columns.Contains(subHeader)))
        //            {
        //                // Add a main header (with colspan equal to the number of subheaders)
        //                TableHeaderCell mainHeaderCell = new TableHeaderCell
        //                {
        //                    Text = $"{mainHeader} ({string.Join("/", subHeaders.Select(sub => sub.Split('$')[1].Split('@')[0]))})",
        //                    ColumnSpan = subHeaders.Count
        //                };
        //                phHeaders.Controls.Add(mainHeaderCell);

        //                // Add subheaders
        //                foreach (string columnName in subHeaders)
        //                {
        //                    if (dt.Columns.Contains(columnName))
        //                    {
        //                        TableHeaderCell subHeaderCell = new TableHeaderCell
        //                        {
        //                            Text = dimensionColumns[columnName] // Use the display name from the dictionary
        //                        };
        //                        phHeaders.Controls.Add(subHeaderCell);
        //                    }
        //                }
        //            }
        //        }
        //    }//recent working
        //    protected void listview1_ItemDataBound(object sender, ListViewItemEventArgs e)
        //    {
        //        if (e.Item.ItemType == ListViewItemType.DataItem)
        //        {
        //            // Find the placeholder for dynamic data cells
        //            var phDynamicData = (PlaceHolder)e.Item.FindControl("phDynamicData");
        //            if (phDynamicData != null && e.Item.DataItem is DataRowView rowView)
        //            {
        //                // Loop through only the dynamic columns defined in dimensionColumns
        //                foreach (var kvp in dimensionColumns)
        //                {
        //                    string columnName = kvp.Key;
        //                    string displayName = kvp.Value;

        //                    if (dt.Columns.Contains(columnName)) // Ensure the column exists in the DataTable
        //                    {
        //                        // Create a new TableCell for the dynamic column data
        //                        TableCell td = new TableCell
        //                        {
        //                            Text = rowView[columnName]?.ToString() // Use DataTable column data
        //                        };
        //                        phDynamicData.Controls.Add(td); // Add the cell to the placeholder
        //                    }
        //                }
        //            }
        //        }
        //    }// recent working

        //    protected void listview1_DataBound(object sender, EventArgs e)
        //    {
        //        var phDynamicHeaders = (PlaceHolder)listview1.FindControl("phDynamicHeaders");
        //        if (phDynamicHeaders == null) return;

        //        phDynamicHeaders.Controls.Clear();

        //        // Group columns by main header
        //        var groupedHeaders = new Dictionary<string, List<string>>()
        //{
        //    { "Fine Boring by Renishaw", new List<string> { "Fine Boring by Renishaw$30.81_30.87@Top", "Fine Boring by Renishaw$30.81_30.87@Bottom" } },
        //    { "Input Size by Renishaw", new List<string> { "Input Size by Renishaw$20.82_20.88@Top", "Input Size by Renishaw$20.81_20.87@Bottom" } },
        //    { "Mechanical Size by Versa", new List<string> { "Mechanical Size by Versa$19.81_19.87" } }
        //};

        //        // Create the rows for main headers and subheaders
        //        TableRow mainHeaderRow = new TableRow();
        //        TableRow subHeaderRow = new TableRow();

        //        // Add the main headers with colspan
        //        foreach (var group in groupedHeaders)
        //        {
        //            var mainHeader = group.Key;
        //            var subHeaders = group.Value;

        //            if (subHeaders.Any(subHeader => dt.Columns.Contains(subHeader))) // Ensure column exists
        //            {
        //                // Add a main header (with colspan equal to the number of subheaders)
        //                TableHeaderCell mainHeaderCell = new TableHeaderCell
        //                {
        //                    Text = $"{mainHeader} ({string.Join("/", subHeaders.Select(sub => sub.Split('$')[1].Split('@')[0]))})",
        //                    ColumnSpan = subHeaders.Count
        //                };
        //                mainHeaderRow.Cells.Add(mainHeaderCell);

        //                // Add subheaders (Top, Bottom, etc.)
        //                foreach (string columnName in subHeaders)
        //                {
        //                    if (dt.Columns.Contains(columnName)) // Ensure column exists
        //                    {
        //                        TableHeaderCell subHeaderCell = new TableHeaderCell
        //                        {
        //                            Text = dimensionColumns[columnName] // Use the display name from the dictionary
        //                        };
        //                        subHeaderRow.Cells.Add(subHeaderCell);
        //                    }
        //                }
        //            }
        //        }

        //        // Add both the main header row and subheader row to the placeholder
        //        phDynamicHeaders.Controls.Add(mainHeaderRow);
        //        phDynamicHeaders.Controls.Add(subHeaderRow);
        //    }// recent working




        //private void AddDynamicHeaders(DataTable dt)
        //{
        //    // Locate the placeholder for dynamic headers
        //    PlaceHolder phHeaders = (PlaceHolder)listview1.FindControl("phDynamicHeaders");
        //    if (phHeaders == null) return;

        //    phHeaders.Controls.Clear();

        //    // Create dynamic headers
        //    foreach (var kvp in dimensionColumns)
        //    {
        //        string columnName = kvp.Key;       // The actual column name in the DataTable
        //        string displayName = kvp.Value;   // The display name from the dictionary

        //        if (dt.Columns.Contains(columnName)) // Ensure the column exists in DataTable
        //        {
        //            Literal header = new Literal
        //            {
        //                Text = $"<th>{(string.IsNullOrEmpty(displayName) ? columnName : displayName)}</th>"
        //            };
        //            phHeaders.Controls.Add(header);
        //        }
        //    }
        //}
        //protected void listview1_ItemDataBound(object sender, ListViewItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListViewItemType.DataItem)
        //    {
        //        // Find the placeholder for dynamic data cells
        //        var phDynamicData = (PlaceHolder)e.Item.FindControl("phDynamicData");
        //        if (phDynamicData != null && e.Item.DataItem is DataRowView rowView)
        //        {
        //            // Loop through only the dynamic columns defined in dimensionColumns
        //            foreach (var kvp in dimensionColumns)
        //            {
        //                string columnName = kvp.Key;
        //                string displayName = kvp.Value;

        //                if (dt.Columns.Contains(columnName)) // Ensure the column exists in the DataTable
        //                {
        //                    // Create a new TableCell for the dynamic column data
        //                    TableCell td = new TableCell
        //                    {
        //                        Text = rowView[columnName]?.ToString() // Use DataTable column data
        //                    };
        //                    phDynamicData.Controls.Add(td); // Add the cell to the placeholder
        //                }
        //            }
        //        }
        //    }
        //}
        //protected void listview1_DataBound(object sender, EventArgs e)
        //{
        //    var phDynamicHeaders = (PlaceHolder)listview1.FindControl("phDynamicHeaders");
        //    if (phDynamicHeaders == null) return;

        //    phDynamicHeaders.Controls.Clear();

        //    foreach (var kvp in dimensionColumns)
        //    {
        //        string columnName = kvp.Key;       // The actual column name in the DataTable
        //        string displayName = kvp.Value;   // The display name from the dictionary

        //        if (dt.Columns.Contains(columnName)) // Ensure the column exists in DataTable
        //        {
        //            TableHeaderCell th = new TableHeaderCell
        //            {
        //                Text = displayName // Display the dictionary value
        //            };
        //            phDynamicHeaders.Controls.Add(th);
        //        }
        //    }
        //}








        private DataTable ConvertToDataTable(string[] values)
        {
            DataTable table = new DataTable();
            table.Columns.Add("Value", typeof(string));

            foreach (string value in values)
            {
                table.Rows.Add(value);
            }

            return table;
        }
    }
}
