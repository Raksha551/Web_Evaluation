

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
using System.Diagnostics;
using static ASP_Evaluation_Task.Screen2_Table;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.IO;


namespace ASP_Evaluation_Task
{
    public partial class Screen2_Table_Copy : System.Web.UI.Page
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

            public int RowSpan { get; set; } = 1;
            public bool tdVisible { get; set; } = true;
        }

        public class childClass
        {
            public string CharacteristicValue { get; set; }
           public string backColor { get; set; }
            public int tdColSpan { get; set; } = 1;
        }




        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopulateMachineDropDown();
                SetDefaultPageloadData();


            }

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

            if (machineDroplist.Items.Count > 0)
            {
                machineDroplist.SelectedIndex = 0;
                MachineDroplist_SelectedIndexChanged(machineDroplist, EventArgs.Empty);
            }
        }

        protected void MachineDroplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMachine = machineDroplist.SelectedValue;
            PopulateComponentDropDown(selectedMachine);
            //  OpDroplist.Items.Clear(); 
            // ddlCharacteristic.Items.Clear();
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
            componentDroplist.DataBind();

            if (componentDroplist.Items.Count > 0)
            {
                componentDroplist.SelectedIndex = 0;
                ComponentDroplist_SelectedIndexChanged(componentDroplist, EventArgs.Empty);
            }
        }



        protected void ComponentDroplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedComponent = componentDroplist.SelectedValue;
            PopulateOperationDropDown(selectedComponent);

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
            OpDroplist.DataBind();

            if (OpDroplist.Items.Count > 0)
            {
                OpDroplist.SelectedIndex = 0;
                Operation_SelectedIndexChanged(OpDroplist, EventArgs.Empty);
            }
        }

        protected void Operation_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMachine = machineDroplist.SelectedValue;
            string OperationNo = OpDroplist.SelectedValue;
            string selectedComponent = componentDroplist.SelectedValue;
            PopulateCharacteristicDropDown(selectedMachine, selectedComponent, OperationNo);
        }

        private void PopulateCharacteristicDropDown(string machineID, string componentID, string operationNo)
        {
            lbxCharacteristic.Items.Clear();
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                SqlCommand cmd = new SqlCommand("SELECT DISTINCT CharacteristicCode FROM SPC_Characteristic WHERE MachineID = @MachineID AND ComponentID = @ComponentID AND OperationNo = @OperationNo", con);
                cmd.Parameters.AddWithValue("@MachineID", machineID);
                cmd.Parameters.AddWithValue("@ComponentID", componentID);
                cmd.Parameters.AddWithValue("@OperationNo", operationNo);
                con.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lbxCharacteristic.Items.Add(reader["CharacteristicCode"].ToString());
                }
            }
            System.Diagnostics.Debug.WriteLine($"Characteristic Items Count: {lbxCharacteristic.Items.Count}");
            lbxCharacteristic.DataBind();

            foreach (var item in lbxCharacteristic.Items.Cast<ListItem>().ToList())
            {
                item.Selected = true;
            };
        }
        private void SetDefaultPageloadData()
        {
            lbxStatus.Items.Clear();
            lbxStatus.Items.Add("Ok");
            lbxStatus.Items.Add("Rework");
            lbxStatus.Items.Add("Rejected");
            lbxStatus.Items.Add("Empty");

            // txtFromDate.Text = "";

            // txtToDate.Text = "";
            System.Diagnostics.Debug.WriteLine($"status Items Count: {lbxStatus.Items.Count}");
            txtFromDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
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
            string status = Request.Form[lbxStatus.UniqueID]; // Comma-separated values
            string[] selectedStatuses = status?.Split(',') ?? new string[] { };

            string characteristic = Request.Form[lbxCharacteristic.UniqueID]; // Comma-separated values
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
                    hasData = true;
                }
                if (hasData)
                {
                    listview1.DataSource = null;
                    listview1.DataBind();
                    List<ColumnNames_111> list = new List<ColumnNames_111>();
                    List<childClass> dynValues = new List<childClass>();

                    ColumnNames_111 data = null;
                    childClass ColumnNames_111data = null;

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i == 0)
                            {
                                ColumnNames_111 Row1 = new ColumnNames_111(); // Header
                                ColumnNames_111 Row2 = new ColumnNames_111(); // Sub header

                                Row1.Date = "Date";
                                Row1.Shift = "Shift";
                                Row1.ComponentId = "ComponentID";
                                Row1.SerialNo = "SerialNo";

                                Row1.SpindleLoad = "SpindleLoad";
                                Row1.Result = "Result";
                                Row1.Remarks = "Remarks";
                                Row1.RowSpan = 2;


                                Row2.tdVisible = false;

                                List<childClass> dynValues_Row1 = new List<childClass>();
                                List<childClass> dynValues_Row2 = new List<childClass>();


                                for (int j = 10; j < dt.Columns.Count; j++)
                                {
                                    string colName = dt.Columns[j].ColumnName;
                                    var keyParts = colName.Split('$');
                                    var mainHeader = keyParts.Length > 0 ? keyParts[0] + keyParts[1].Split('@')[0] : colName;
                                    var subHeader = keyParts.Length > 1 && keyParts[1].Contains('@')
                                        ? keyParts[1].Split('@').Last()
                                        : string.Empty;

                                    dynValues_Row1.Add(new childClass { CharacteristicValue = mainHeader });
                                    dynValues_Row2.Add(new childClass { CharacteristicValue = subHeader });

                                }
                                Row1.listviewdata = dynValues_Row1;
                                Row2.listviewdata = dynValues_Row2;

                                list.Add(Row1);
                                list.Add(Row2);
                            }


                            data = new ColumnNames_111();
                            data.Date = dt.Rows[i]["Date"].ToString();  // Assuming the column names match

                            data.Shift = dt.Rows[i]["Shift"].ToString();
                            data.ComponentId = dt.Rows[i]["ComponentID"].ToString();
                            data.SerialNo = dt.Rows[i]["SerialNo"].ToString();
                            data.SpindleLoad = dt.Rows[i]["SpindleLoad"].ToString();
                            data.Result = dt.Rows[i]["Result"].ToString();
                            data.Remarks = dt.Rows[i]["Remarks"].ToString();
                            int v=dt.Rows.Count;

                            dynValues = new List<childClass>();
                            for (int j = 10; j < dt.Columns.Count; j++)
                            {
                                ColumnNames_111data = new childClass();
                                ColumnNames_111data.CharacteristicValue = dt.Rows[i][j].ToString();
                                //  ColumnNames_111data.dynamicvalues = dt.Rows[i][j].ToString();
                                dynValues.Add(ColumnNames_111data);

                                string colName = dt.Columns[j].ColumnName;
                                var keyParts = colName.Split('$');
                                var mainHeader = keyParts.Length > 0 ? keyParts[0] + keyParts[1].Split('@')[0] : colName;
                                var subHeader = keyParts.Length > 1 && keyParts[1].Contains('@')
                                    ? keyParts[1].Split('@').Last()
                                    : string.Empty;
                            
                                string dvalue = ColumnNames_111data.CharacteristicValue;

                                if (!string.IsNullOrEmpty(dvalue) && double.TryParse(ColumnNames_111data.CharacteristicValue, out double value))
                                {

                                    ColumnNames_111data.backColor = "color : red;"; // Default to red

                                    // Apply conditions for specific columns
                                    if (mainHeader == "Fine Boring by Renishaw30.81_30.87" && value >= 30.81 && value <= 30.87)
                                    {
                                        ColumnNames_111data.backColor = "color : green;";
                                    }
                                    else if (mainHeader == "Fine Boring by Renishaw30.81_30.87" && value >= 30.81 && value <= 30.87)
                                    {
                                        ColumnNames_111data.backColor = "color : green;";
                                    }
                                    else if (mainHeader == "Input Size by Renishaw20.82_20.88" && value >= 20.82 && value <= 20.88)
                                    {
                                        ColumnNames_111data.backColor = "color : green;";
                                    }
                                    else if (mainHeader == "Input Size by Renishaw20.82_20.88" && value <= 20.87 && value <= 20.88)
                                    {
                                        ColumnNames_111data.backColor = "color : green;";
                                    }
                                    else if (mainHeader == "Mechanical Size by Versa19.81_19.87" && value >= 19.81 && value <= 19.87)
                                    {
                                        ColumnNames_111data.backColor = "color : green;";
                                    }


                                    // Find the Label control in the template and apply the style



                                }

                            }

                            data.listviewdata = dynValues; //RenderColumn.HeaderVisibility = false;
                            list.Add(data);
                        }
                       

                            listview1.DataSource = list;
                            listview1.DataBind();
                        }
                    }
                    else
                    {

                        listview1.DataSource = null; // Clear any previous data
                        listview1.DataBind();
                    }

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
        protected void btnExport_Click(object sender, EventArgs e)
{
    // Create Excel Package
    using (ExcelPackage excelPackage = new ExcelPackage())
    {
        // Add a worksheet
        ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("ExportedData");

        int row = 1; // Start writing from the first row
        int col = 1; // Start writing from the first column

        // Export Header Row
        worksheet.Cells[row, col].Value = "Date";
        worksheet.Cells[row, col + 1].Value = "Shift";
        worksheet.Cells[row, col + 2].Value = "ComponentID";
        worksheet.Cells[row, col + 3].Value = "SerialNo";
        worksheet.Cells[row, col + 4].Value = "SpindleLoad";
        worksheet.Cells[row, col + 5].Value = "Result";
        worksheet.Cells[row, col + 6].Value = "Remarks";

        // Apply header styles
        using (var range = worksheet.Cells[row, 1, row, 7])
        {
            range.Style.Font.Bold = true;
            range.Style.Fill.PatternType = ExcelFillStyle.Solid;
            range.Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
        }

        row++; // Move to the next row for data

               
        foreach (ListViewItem item in listview1.Items)
                {
                    // Export outer ListView data
                    worksheet.Cells[row, col].Value = ((Label)item.FindControl("Date")).Text;
                    worksheet.Cells[row, col + 1].Value = ((Label)item.FindControl("Shift")).Text;
                    worksheet.Cells[row, col + 2].Value = ((Label)item.FindControl("ComponentID")).Text;
                    worksheet.Cells[row, col + 3].Value = ((Label)item.FindControl("SerialNo")).Text;

                    // Export inner ListView data
                    ListView lvInner = (ListView)item.FindControl("lvInnerListView");
                    int innerRow = row;

                    foreach (ListViewItem innerItem in lvInner.Items)
                    {
                        var characteristicValueLabel = (Label)innerItem.FindControl("CharacteristicValueLabel");
                        var backColorStyle = innerItem.DataItem as dynamic;

                        // Write CharacteristicValue
                        worksheet.Cells[innerRow, col + 4].Value = characteristicValueLabel.Text;

                        // Apply Cell Style based on backColor
                        if (backColorStyle != null && backColorStyle.backColor != null)
                        {
                            Color cellColor = ColorTranslator.FromHtml(backColorStyle.backColor);
                            worksheet.Cells[innerRow, col + 4].Style.Fill.PatternType = ExcelFillStyle.Solid;
                            worksheet.Cells[innerRow, col + 4].Style.Fill.BackgroundColor.SetColor(cellColor);
                        }

                        innerRow++;
                    }

                    // Move to the next row
                    row = Math.Max(innerRow, row + 1);

                    // Export SpindleLoad, Result, and Remarks
                    worksheet.Cells[row - 1, col + 5].Value = ((Label)item.FindControl("SpindleLoad")).Text;
                    worksheet.Cells[row - 1, col + 6].Value = ((Label)item.FindControl("Result")).Text;
                    worksheet.Cells[row - 1, col + 7].Value = ((Label)item.FindControl("Remarks")).Text;
                }

                // Auto-fit columns for better readability
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Set Response Headers for Download
        Response.Clear();
        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
        Response.AddHeader("content-disposition", "attachment; filename=ExportedListViewData.xlsx");

        // Write Excel file to response
        using (MemoryStream stream = new MemoryStream())
        {
            excelPackage.SaveAs(stream);
            stream.WriteTo(Response.OutputStream);
            Response.Flush();
            Response.End();
        }
    }
            //using (ExcelPackage package1 = new ExcelPackage())
            //{
            //    ExcelWorksheet ws = package1.Workbook.Worksheets.Add("Screen2_Table");
            //    ws.Cells["A1"].LoadFromDataTable(listview1, true);
            //    package1.SaveAs(new FileInfo(@"C:\Users\devteam\Documents\Demo_C#\WEB_ASP.Net\EPPlus_ExcelScreen2.xlsx"));
            //}
        }
        public class DimensionColumn
        {
            public string Key { get; set; }
            public string DisplayName { get; set; }
        }


        //protected void listview1_DataBound(object sender, EventArgs e)
        //{
        //    if (listview1.DataSource is DataTable resultTable)
        //    {
        //        // Filter dynamic headers that exist in the resultTable
        //        var dynamicHeaders = dimensionColumns
        //            .Where(columnName => resultTable.Columns.Contains(columnName))
        //            .ToList();

        //        // Find the dynamic headers ListView
        //        var dynamicHeadersListView = listview1.FindControl("lvInnerListView") as ListView;
        //        if (dynamicHeadersListView != null)
        //        {
        //            dynamicHeadersListView.DataSource = dynamicHeaders;
        //            dynamicHeadersListView.DataBind();
        //        }
        //    }
        //}



        //protected void phDynamicHeaders_ItemDataBound(object sender, ListViewItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListViewItemType.DataItem)
        //    {
        //        var groupedHeader = (KeyValuePair<string, List<string>>)e.Item.DataItem;

        //        if (!string.IsNullOrEmpty(groupedHeader.Key))
        //        {
        //            var mainheader = groupedHeader.Key;
        //            var subheader = groupedHeader.Value;

        //            var literalHeader = e.Item.FindControl("headerPlaceHolder") as Literal;
        //            if (literalHeader != null)
        //            {
        //                int colspan = subheader.Count > 1 ? 2 : 1;
        //                literalHeader.Text = $"<th colspan='{colspan}'>{mainheader}</th>";  // Main header
        //            }

        //            // Bind only the filtered subheaders
        //            var subHeadersListView = e.Item.FindControl("phDynamicSubHeaders") as ListView;
        //            if (subHeadersListView != null)
        //            {
        //                subHeadersListView.DataSource = subheader; // Subheaders
        //                subHeadersListView.DataBind();
        //            }
        //        }
        //    }

        //}
        //protected void phDynamicSubHeaders_ItemDataBound(object sender, ListViewItemEventArgs e)
        //{
        //    if (e.Item.ItemType == ListViewItemType.DataItem)
        //    {
        //        var subHeader = (string)e.Item.DataItem;
        //        var literalSubHeader = e.Item.FindControl("subHeaderPlaceHolder") as Literal;
        //        if (literalSubHeader != null)
        //        {
        //            literalSubHeader.Text = subHeader; // Set the subheader text
        //        }
        //    }
        //}


     
    }
}





//using System;
//using System.Collections.Generic;
//using System.Data.SqlClient;
//using System.Data;
//using System.Linq;
//using System.Web;
//using System.Web.Configuration;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
//using static System.Net.Mime.MediaTypeNames;
//using System.Data.Common;
//using System.Drawing;
//using System.Globalization;
//using System.Runtime.Remoting.Messaging;
//using System.ComponentModel;
//using System.Diagnostics;



//namespace ASP_Evaluation_Task
//{
//    public partial class Screen2_Table : System.Web.UI.Page
//    {// A dictionary to hold the dynamic headers and their corresponding values
//        private Dictionary<string, List<string>> dynamicHeaders = new Dictionary<string, List<string>>();
//        private List<string> columnOrder;
//        private Dictionary<string, string> dimensionColumns1 = new Dictionary<string, string>
//{
//    { "Fine Boring by Renishaw$30.81_30.87@Top", "Top" },
//    { "Fine Boring by Renishaw$30.81_30.87@Bottom", "Bottom" },
//    { "Input Size by Renishaw$20.82_20.88@Top", "Top" },
//    { "Input Size by Renishaw$20.81_20.87@Bottom", "Bottom" },
//    { "Mechanical Size by Versa$19.81_19.87", "" }
//};
//        private List<string> dimensionColumns = new List<string>
//{
//    "Fine Boring by Renishaw$30.81_30.87@Top",
//    "Fine Boring by Renishaw$30.81_30.87@Bottom",
//    "Input Size by Renishaw$20.82_20.88@Top",
//    "Input Size by Renishaw$20.81_20.87@Bottom",
//    "Mechanical Size by Versa$19.81_19.87"
//};

//        private string ConnectionString => WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

//        public class ColumnNames_111
//        {
//            public DateTime Date { get; set; }
//            public string Shift { get; set; } = string.Empty;
//            public string ComponentId { get; set; }
//            public string SerialNo { get; set; }
//            public List<ColumnNames_111> listviewdata { get; set; } = new List<ColumnNames_111>();
//            public string RenderData { get; set; }
//            public string SpindleLoad { get; set; }
//            public string Result { get; set; }
//            public string Remarks { get; set; }
//            public bool headervisible { get; set; }

//        }
//        protected void Page_Load(object sender, EventArgs e)
//        {

//            if (!IsPostBack)
//            {
//                PopulateMachineDropDown();
//                SetDefaultPageloadData();


//            }

//        }
//        private void PopulateMachineDropDown()
//        {
//            machineDroplist.Items.Clear();
//            List<string> list = new List<string>();
//            using (SqlConnection con = new SqlConnection(ConnectionString))
//            {
//                SqlCommand cmd = new SqlCommand("SELECT DISTINCT MachineID FROM Machineinformation", con);
//                con.Open();
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    list.Add(reader["MachineID"].ToString());
//                }
//            }
//            machineDroplist.DataSource = list;
//            machineDroplist.DataBind();

//            if (machineDroplist.Items.Count > 0)
//            {
//                machineDroplist.SelectedIndex = 0;
//                MachineDroplist_SelectedIndexChanged(machineDroplist, EventArgs.Empty);
//            }
//        }

//        protected void MachineDroplist_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            string selectedMachine = machineDroplist.SelectedValue;
//            PopulateComponentDropDown(selectedMachine);
//            //  OpDroplist.Items.Clear(); 
//            // ddlCharacteristic.Items.Clear();
//        }

//        private void PopulateComponentDropDown(string machineID)
//        {
//            componentDroplist.Items.Clear();
//            using (SqlConnection con = new SqlConnection(ConnectionString))
//            {
//                SqlCommand cmd = new SqlCommand("SELECT DISTINCT ComponentID FROM componentoperationpricing WHERE MachineID = @MachineID", con);
//                cmd.Parameters.AddWithValue("@MachineID", machineID);
//                con.Open();
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    componentDroplist.Items.Add(reader["ComponentID"].ToString());
//                }
//            }
//            componentDroplist.DataBind();

//            if (componentDroplist.Items.Count > 0)
//            {
//                componentDroplist.SelectedIndex = 0;
//                ComponentDroplist_SelectedIndexChanged(componentDroplist, EventArgs.Empty);
//            }
//        }



//        protected void ComponentDroplist_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            string selectedComponent = componentDroplist.SelectedValue;
//            PopulateOperationDropDown(selectedComponent);

//        }
//        private void PopulateOperationDropDown(string componentID)
//        {
//            OpDroplist.Items.Clear();
//            using (SqlConnection con = new SqlConnection(ConnectionString))
//            {
//                SqlCommand cmd = new SqlCommand("SELECT DISTINCT OperationNo FROM componentoperationpricing WHERE ComponentID = @ComponentID", con);
//                cmd.Parameters.AddWithValue("@ComponentID", componentID);
//                con.Open();
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    OpDroplist.Items.Add(reader["OperationNo"].ToString());
//                }
//            }
//            OpDroplist.DataBind();

//            if (OpDroplist.Items.Count > 0)
//            {
//                OpDroplist.SelectedIndex = 0;
//                Operation_SelectedIndexChanged(OpDroplist, EventArgs.Empty);
//            }
//        }

//        protected void Operation_SelectedIndexChanged(object sender, EventArgs e)
//        {
//            string selectedMachine = machineDroplist.SelectedValue;
//            string OperationNo = OpDroplist.SelectedValue;
//            string selectedComponent = componentDroplist.SelectedValue;
//            PopulateCharacteristicDropDown(selectedMachine, selectedComponent, OperationNo);
//        }

//        private void PopulateCharacteristicDropDown(string machineID, string componentID, string operationNo)
//        {
//            lbxCharacteristic.Items.Clear();
//            using (SqlConnection con = new SqlConnection(ConnectionString))
//            {
//                SqlCommand cmd = new SqlCommand("SELECT DISTINCT CharacteristicCode FROM SPC_Characteristic WHERE MachineID = @MachineID AND ComponentID = @ComponentID AND OperationNo = @OperationNo", con);
//                cmd.Parameters.AddWithValue("@MachineID", machineID);
//                cmd.Parameters.AddWithValue("@ComponentID", componentID);
//                cmd.Parameters.AddWithValue("@OperationNo", operationNo);
//                con.Open();
//                SqlDataReader reader = cmd.ExecuteReader();
//                while (reader.Read())
//                {
//                    lbxCharacteristic.Items.Add(reader["CharacteristicCode"].ToString());
//                }
//            }
//            System.Diagnostics.Debug.WriteLine($"Characteristic Items Count: {lbxCharacteristic.Items.Count}");
//            lbxCharacteristic.DataBind();

//            foreach (var item in lbxCharacteristic.Items.Cast<ListItem>().ToList())
//            {
//                item.Selected = true;
//            };
//        }
//        private void SetDefaultPageloadData()
//        {
//            lbxStatus.Items.Clear();
//            lbxStatus.Items.Add("Ok");
//            lbxStatus.Items.Add("Rework");
//            lbxStatus.Items.Add("Rejected");
//            lbxStatus.Items.Add("Empty");

//           // txtFromDate.Text = "";

//           // txtToDate.Text = "";
//           System.Diagnostics.Debug.WriteLine($"status Items Count: {lbxStatus.Items.Count}");
//            txtFromDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
//            txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
//        }
//        protected void ViewButton_Click(object sender, EventArgs e)
//        {
//            string machineID = machineDroplist.SelectedValue;
//            string componentID = componentDroplist.SelectedValue;
//            string operationNo = OpDroplist.SelectedValue;
//            string serialNo = txtSerialNo.Text;
//            //  string status = ddlStatus.SelectedValue;
//            string fromDate = txtFromDate.Text;
//            string toDate = txtToDate.Text;
//            //  string dimension = ddlCharacteristic.SelectedValue;
//            string status = Request.Form[lbxStatus.UniqueID]; // Comma-separated values
//            string[] selectedStatuses = status?.Split(',') ?? new string[] { };

//            string characteristic = Request.Form[lbxCharacteristic.UniqueID]; // Comma-separated values
//            string[] selectedCharacteristics = characteristic?.Split(',') ?? new string[] { };

//            // Validate inputs
//            if (string.IsNullOrEmpty(machineID) || string.IsNullOrEmpty(componentID))
//            {
//                //  lblMessage.Text = "Please select a valid Machine and Component.";
//                //  lblMessage.Visible = true;
//                return;
//            }

//            // Ensure the fromDate and toDate are in the correct format
//            string formattedFromDate = ConvertToCustomDateFormat(fromDate);
//            string formattedToDate = ConvertToCustomDateFormat(toDate);

//            // Log the formatted dates for debugging
//            System.Diagnostics.Debug.WriteLine($"Formatted From Date: {formattedFromDate}, Formatted To Date: {formattedToDate}");

//            // Fetch the filtered data
//            DataTable resultTable = FetchReportData(machineID, componentID, operationNo, serialNo, selectedStatuses, formattedFromDate, formattedToDate, selectedCharacteristics);
//            System.Diagnostics.Debug.WriteLine($"Fetched data: {resultTable.Rows.Count} rows");
//            if (resultTable.Rows.Count > 0)
//            {
//                bool hasData = false;


//                if (resultTable.Columns.Contains("SaveFlag"))
//                {
//                    // If the 'SaveFlag' column exists, check its value
//                    if (resultTable.Rows[0]["SaveFlag"].ToString() == "No records found!")
//                    {
//                        //  lblMessage.Text = "No data found for the given search criteria.";
//                        //  lblMessage.Visible = true;
//                        listview1.DataSource = null; // Clear any previous data
//                        listview1.DataBind();
//                    }
//                    else
//                    {
//                        hasData = true;
//                    }
//                }
//                else
//                {
//                    hasData = true;
//                }
//                if (hasData)
//                {
//                    listview1.DataSource = null;
//                    listview1.DataBind();
//                    List<ColumnNames_111> list = new List<ColumnNames_111>();
//                    List<ColumnNames_111> dynValues = new List<ColumnNames_111>();
//                    ColumnNames_111 data = null;
//                    ColumnNames_111 ColumnNames_111data = null;

//                    if (dt != null && dt.Rows.Count > 0)
//                    {
//                        for (int i = 0; i < dt.Rows.Count; i++)
//                        {
//                            if (i == 0)
//                            {
//                                data = new ColumnNames_111();
//                                data.Date = Convert.ToDateTime(dt.Rows[i]["Date"]);  // Assuming the column names match
//                                data.Shift = dt.Rows[i]["Shift"].ToString();
//                                data.ComponentId = dt.Rows[i]["ComponentID"].ToString();
//                                data.SerialNo = dt.Rows[i]["SerialNo"].ToString();
//                                data.SpindleLoad = dt.Rows[i]["SpindleLoad"].ToString();
//                                data.Result = dt.Rows[i]["Result"].ToString();
//                                data.Remarks = dt.Rows[i]["Remarks"].ToString();
//                                data.headervisible = true;
//                                dynValues = new List<ColumnNames_111>();
//                                for (int j = 10; j < dt.Columns.Count; j++)
//                                {
//                                    ColumnNames_111data = new ColumnNames_111();
//                                    ColumnNames_111data.RenderData = dt.Columns[j].ColumnName;
//                                    dynValues.Add(ColumnNames_111data);
//                                }                                //RenderColumn.HeaderVisibility = false;
//                                list.Add(data);                               //RenderData.HeaderVisibility = false;

//                            }
//                           // list.Add(data);
//                        }

//                    }
//                    // lblMessage.Visible = false;
//                    // Assuming you already have the DataTable 'dt'
//                    listview1.DataSource = resultTable;
//                    listview1.DataBind();
//                }
//            }
//            else
//            {
//                // lblMessage.Text = "No data found for the given search criteria.";
//                //  lblMessage.Visible = true;
//                listview1.DataSource = null; // Clear any previous data
//                listview1.DataBind();
//            }

//        }

//        private string ConvertToCustomDateFormat(string dateString)
//        {
//            // Log the incoming date string to verify its format
//            System.Diagnostics.Debug.WriteLine($"ConvertToCustomDateFormat called with dateString: {dateString}");
//            if (dateString.Length == 16) // Format yyyy-MM-ddTHH:mm (no seconds)
//            {
//                // Append ":00" to include seconds
//                dateString += ":00";
//            }

//            DateTime parsedDate;

//            // Try parsing the date in the format received from the DateTimeLocal input (yyyy-MM-ddTHH:mm:ss)
//            if (DateTime.TryParseExact(dateString, "yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
//            {
//                // Log the successfully parsed date
//                System.Diagnostics.Debug.WriteLine($"Parsed Date: {parsedDate.ToString()}");

//                // Return the date in dd:MM:yyyy HH:mm:ss format
//                return parsedDate.ToString("dd:MM:yyyy HH:mm:ss");
//            }
//            else
//            {
//                // Log if the parsing fails
//                System.Diagnostics.Debug.WriteLine("Date parsing failed.");

//                // If the date format is invalid, return an empty string (or handle error as needed)
//                return string.Empty;
//            }
//        }
//        DataTable dt = new DataTable();
//        private DataTable FetchReportData(string machineID, string componentID, string operationNo, string serialNo, string[] status, string fromDate, string toDate, string[] dimension)
//        {


//            // Ensure the dates are parsed to correct format
//            DateTime startDate;
//            DateTime endDate;

//            if (!DateTime.TryParseExact(fromDate, "dd:MM:yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
//            {
//                // lblMessage.Text = "Invalid From Date format. Please use the format dd:MM:yyyy HH:mm:ss.";
//                //  lblMessage.Visible = true;
//                return dt;
//            }

//            if (!DateTime.TryParseExact(toDate, "dd:MM:yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
//            {
//                // lblMessage.Text = "Invalid To Date format. Please use the format dd:MM:yyyy HH:mm:ss.";
//                //  lblMessage.Visible = true;
//                return dt;
//            }
//            serialNo = "%" + serialNo + "%";   // Add '%' for partial match in SQL


//            serialNo = serialNo.Trim(); // Remove wildcard for testing
//            machineID = machineID.Trim();
//            componentID = componentID.Trim();
//            // status = status.Trim();
//            //dimension = dimension.Trim();

//            System.Diagnostics.Debug.WriteLine("binding starts");


//            using (SqlConnection con = new SqlConnection(ConnectionString))
//            {
//                using (SqlCommand cmd = new SqlCommand("S_GetSlnoWiseSPCReport_Bajaj", con))
//                {
//                    cmd.CommandType = CommandType.StoredProcedure;

//                    // Use parameters for all user inputs, including dates
//                    cmd.Parameters.AddWithValue("@MachineID", machineID);
//                    cmd.Parameters.AddWithValue("@ComponentID", componentID);
//                    cmd.Parameters.AddWithValue("@OperationNo", operationNo);
//                    cmd.Parameters.AddWithValue("@SerialNo", serialNo);
//                    cmd.Parameters.AddWithValue("@Dimension", string.Join(",", dimension));
//                    cmd.Parameters.AddWithValue("@Status", string.Join(",", status));
//                    cmd.Parameters.AddWithValue("@StartDate", startDate);
//                    cmd.Parameters.AddWithValue("@EndDate", endDate);

//                    SqlDataAdapter da = new SqlDataAdapter(cmd);
//                    da.Fill(dt);
//                }
//            }
//            System.Diagnostics.Debug.WriteLine("Rows in DataTable: " + dt.Rows.Count);
//            foreach (DataRow row in dt.Rows)
//            {
//                foreach (DataColumn column in dt.Columns)
//                {
//                    System.Diagnostics.Debug.WriteLine($"{column.ColumnName}: {row[column]}");
//                }
//            }

//            return dt;
//        }
//        public class DimensionColumn
//        {
//            public string Key { get; set; }
//            public string DisplayName { get; set; }
//        }
//        protected void listview1_DataBound(object sender, EventArgs e)
//        {
//            if (listview1.DataSource is DataTable resultTable)
//            {
//                // Filter dynamic headers that exist in the resultTable
//                var dynamicHeaders = dimensionColumns
//                    .Where(columnName => resultTable.Columns.Contains(columnName))
//                    .ToList();

//                // Find the dynamic headers ListView
//                var dynamicHeadersListView = listview1.FindControl("phDynamicHeaders") as ListView;
//                if (dynamicHeadersListView != null)
//                {
//                    dynamicHeadersListView.DataSource = dynamicHeaders;
//                    dynamicHeadersListView.DataBind();
//                }
//            }
//        }
//        protected void listview1_ItemDataBound(object sender, ListViewItemEventArgs e)
//        {
//            if (e.Item.ItemType == ListViewItemType.DataItem)
//            {
//                var rowView = e.Item.DataItem as DataRowView;
//                if (rowView != null)
//                {
//                    // Extract dynamic data for this row
//                    var dynamicData = dimensionColumns
//                        .Where(columnName => rowView.DataView.Table.Columns.Contains(columnName))
//                        .Select(columnName => new { Value = rowView[columnName]?.ToString() ?? string.Empty })
//                        .ToList();

//                    // Bind dynamic data to the ListView for this row
//                    var dynamicDataListView = e.Item.FindControl("phDynamicData") as ListView;
//                    if (dynamicDataListView != null)
//                    {
//                        dynamicDataListView.DataSource = dynamicData;
//                        dynamicDataListView.DataBind();
//                    }
//                }
//            }
//            if (e.Item.ItemType == ListViewItemType.DataItem)
//            {
//                var dataItem1 = e.Item.DataItem as DataRowView;
//                if (dataItem1 != null)
//                {
//                    foreach (DataColumn column in dataItem1.DataView.Table.Columns)
//                    {
//                        string columnName = column.ColumnName;
//                        if (dimensionColumns.Contains(columnName))
//                        {
//                            var numericvalue = dataItem1[columnName]; // Get the value from the DataRowView
//                            if (numericvalue != DBNull.Value)
//                            {
//                                try
//                                {
//                                    double value = Convert.ToDouble(numericvalue);
//                                    string style = "color: red;"; // Default to red
//                                    if (columnName == "Fine Boring by Renishaw$30.81_30.87@Top" && value >= 30.81 && value <= 30.87)
//                                    {
//                                        style = "color: green;";
//                                    }
//                                    else if (columnName == "Fine Boring by Renishaw$30.81_30.87@Bottom" && value >= 30.81 && value <= 30.87)
//                                    {
//                                        style = "color: green;";
//                                    }
//                                    else if (columnName == "Input Size by Renishaw$20.82_20.88@Top" && value >= 20.82 && value <= 20.88)
//                                    {
//                                        style = "color: green;";
//                                    }
//                                    else if (columnName == "Input Size by Renishaw$20.81_20.87@Bottom" && value <= 20.87 && value <= 20.88)
//                                    {
//                                        style = "color: green;";
//                                    }
//                                    else if (columnName == "Mechanical Size by Versa$19.81_19.87" && value >= 19.81 && value <= 19.87)
//                                    {
//                                        style = "color: green;";
//                                    }

//                                    var dynamicDataListView = e.Item.FindControl("phDynamicData") as ListView;
//                                    if (dynamicDataListView != null)
//                                    {
//                                        foreach (ListViewDataItem nestedItem in dynamicDataListView.Items)
//                                        {
//                                            var dataPlaceHolder = nestedItem.FindControl("DataPlaceHolder") as Literal;
//                                            if (dataPlaceHolder != null)
//                                            {
//                                                // Apply your logic to style and display the data
//                                                dataPlaceHolder.Text = $"<span style='{style}'>{value}</span>";
//                                            }
//                                            else
//                                            {
//                                                // Debugging for missing control
//                                                Debug.WriteLine("DataPlaceHolder is null in nested item");
//                                            }
//                                        }
//                                    }
//                                }
//                                catch (Exception ex)
//                                {
//                                    Response.Write(ex);
//                                }
//                            }
//                        }
//                    }
//                }

//            }
//        }

//        protected void phDynamicHeaders_ItemDataBound(object sender, ListViewItemEventArgs e)
//        {
//            if (e.Item.ItemType == ListViewItemType.DataItem)
//            {
//                var columnName = e.Item.DataItem as string;
//                if (!string.IsNullOrEmpty(columnName))
//                {
//                    var literalHeader = e.Item.FindControl("headerPlaceHolder") as Literal;
//                    if (literalHeader != null)
//                    {
//                        literalHeader.Text = columnName; // Set the column name as the header
//                    }
//                }
//            }
//        }

//        protected void phDynamicData_ItemDataBound(object sender, ListViewItemEventArgs e)
//        {
//            if (e.Item.ItemType == ListViewItemType.DataItem)
//            {
//                var dataItem = e.Item.DataItem as dynamic;
//                if (dataItem != null)
//                {
//                    var literalData = e.Item.FindControl("DataPlaceHolder") as Literal;
//                    if (literalData != null)
//                    {

//                        literalData.Text = dataItem.Value; // Set the dynamic cell value

//                    }
//                }


//            }
//        }

//    }
//}


