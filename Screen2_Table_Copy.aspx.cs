

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
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style.XmlAccess;
using System.IO;
using System.Xml.Xsl;
using System.Collections;
using static ASP_Evaluation_Task.Screen2_Table_Copy;
using static OfficeOpenXml.ExcelErrorValue;


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
        DatabaseHelper dbHelper= new DatabaseHelper();

        DataTable dt = null;
        private string ConnectionString => WebConfigurationManager.ConnectionStrings["DBCS"].ConnectionString;

      



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
            try
            {
                machineDroplist.Items.Clear();
                List<string> list = new List<string>();
                list = DatabaseHelper.Populatemachineid();
                machineDroplist.DataSource = list;
                machineDroplist.DataBind();
                if (machineDroplist.Items.Count > 0)
                {
                    machineDroplist.SelectedIndex = 0;
                    MachineDroplist_SelectedIndexChanged(machineDroplist, EventArgs.Empty);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in PopulateMachineDropDown: {ex.Message}");
            }
        }

        protected void MachineDroplist_SelectedIndexChanged(object sender, EventArgs e)
        {
            string selectedMachine = machineDroplist.SelectedValue;
            PopulateComponentDropDown(selectedMachine);
        }

        private void PopulateComponentDropDown(string machineID)
        {
            List<string> list = new List<string>();
            list=DatabaseHelper.Populatecomponent(machineID);
          
                componentDroplist.DataSource = list;
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
            List<string> list=new List<string>();
          list=DatabaseHelper.PopulateOperationdropdown(componentID);
            OpDroplist.DataSource= list;
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
            List<string> list = new List<string>();
               list= DatabaseHelper.PopulateCharacteristicdropdown(machineID, componentID, operationNo);

            lbxCharacteristic.DataSource = list;
            lbxCharacteristic.DataBind();
            foreach (var item in lbxCharacteristic.Items.Cast<ListItem>().ToList())
            {
                item.Selected = true;
            }
        }
        private void SetDefaultPageloadData()
        {
            try
            {
                lbxStatus.Items.Clear();
                lbxStatus.Items.Add("Ok");
                lbxStatus.Items.Add("Rework");
                lbxStatus.Items.Add("Rejected");
                lbxStatus.Items.Add("Empty");
              //  txtFromDate.Text = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd HH:mm:ss");
               // txtToDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in SetDefaultPageloadData: {ex.Message}");
            }
        }
        protected void ViewButton_Click(object sender, EventArgs e)
        {
            string machineID = machineDroplist.SelectedValue;
            string componentID = componentDroplist.SelectedValue;
            string operationNo = OpDroplist.SelectedValue;
            string serialNo = txtSerialNo.Text;
            string fromDate = txtFromDate.Text;
            string toDate = txtToDate.Text;
            string status = Request.Form[lbxStatus.UniqueID]; // Comma-separated values
            string[] selectedStatuses = status?.Split(',') ?? new string[] { };
           string characteristic = Request.Form[lbxCharacteristic.UniqueID];
            string[] selectedCharacteristics = characteristic?.Split(',') ?? new string[] { };
            if (string.IsNullOrEmpty(machineID) || string.IsNullOrEmpty(componentID))
            {
                return;
            }

            string formattedFromDate =Helper.ConvertToCustomDateFormat(fromDate);
            string formattedToDate = Helper.ConvertToCustomDateFormat(toDate);

            System.Diagnostics.Debug.WriteLine($"Formatted From Date: {formattedFromDate}, Formatted To Date: {formattedToDate}");
            DataTable resultTable = FetchReportData(machineID, componentID, operationNo, serialNo, selectedStatuses, formattedFromDate, formattedToDate, selectedCharacteristics);
            System.Diagnostics.Debug.WriteLine($"Fetched data: {resultTable.Rows.Count} rows");
            if (resultTable.Rows.Count > 0)
            {
                bool hasData = false;
                if (resultTable.Columns.Contains("SaveFlag"))
                {
                    if (resultTable.Rows[0]["SaveFlag"].ToString() == "No records found!")
                    {
                        listview1.DataSource = null; 
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
                    List<DatabaseHelper.ColumnNames_111> list = new List<DatabaseHelper.ColumnNames_111>();
                    List<DatabaseHelper.childClass> dynValues = new List<DatabaseHelper.childClass>();

                    DatabaseHelper.ColumnNames_111 data = null;
                    DatabaseHelper.childClass ColumnNames_111data = null;

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (i == 0)
                            {
                                DatabaseHelper.ColumnNames_111 Row1 = new DatabaseHelper.ColumnNames_111(); // Header
                                DatabaseHelper.ColumnNames_111 Row2 =new DatabaseHelper.ColumnNames_111(); // Sub header

                                Row1.Date = "Date";
                                Row1.Shift = "Shift";
                                Row1.ComponentId = "ComponentID";
                                Row1.SerialNo = "SerialNo";

                                Row1.SpindleLoad = "SpindleLoad";
                                Row1.Result = "Result";
                                Row1.Remarks = "Remarks";
                                Row1.RowSpan = 2;
                                Row1.tdColSpan = 1;


                                Row2.tdVisible = false;

                                List<DatabaseHelper.childClass> dynValues_Row1 = new List<DatabaseHelper.childClass>();
                                List<DatabaseHelper.childClass> dynValues_Row2 = new List<DatabaseHelper.childClass>();

                                Dictionary<string, List<string>> headerGroups = new Dictionary<string, List<string>>();                             

                                for (int j = 10; j < dt.Columns.Count; j++)
                                {
                                    string colName = dt.Columns[j].ColumnName;
                                    var keyParts = colName.Split('$');
                                  //  var mainHeader = keyParts.Length > 0 ? keyParts[0] + keyParts[1].Split('@')[0] : colName;
                                    var mainHeader = keyParts.Length > 0
     ? keyParts[0].Split(new[] { " by " }, StringSplitOptions.None)[0]
     : colName;
                                    var subHeader = keyParts.Length > 1 && keyParts[1].Contains('@')
                                        ? keyParts[1].Split('@').Last()
                                        : string.Empty;

                                    //    dynValues_Row1.Add(new childClass { CharacteristicValue = mainHeader });
                                    //    dynValues_Row2.Add(new childClass { CharacteristicValue = subHeader });

                                    //}

                                    if (!headerGroups.ContainsKey(mainHeader))
                                    {
                                        headerGroups[mainHeader] = new List<string>();
                                    }

                                    headerGroups[mainHeader].Add(subHeader);
                                }

                          
                                dynValues_Row1.Clear();
                                dynValues_Row2.Clear();

                                foreach (var mainHeader in headerGroups)
                                {
                                    var subHeaders = mainHeader.Value.Where(s => !string.IsNullOrEmpty(s)).ToList();
                                    int subheaderCount = subHeaders.Count;

                                    // Add main header to dynValues_Row1 only once
                                    dynValues_Row1.Add(new DatabaseHelper.childClass
                                    {
                                        CharacteristicValue = mainHeader.Key,
                                        tdColSpan = mainHeader.Key == "Mechanical Size" ? 1 : Math.Max(subheaderCount, 1) // For Mechanical Size, colspan is fixed to 1
                                    });

                                    // Add subheaders or a single blank subheader if none exist
                                    if (subheaderCount > 0)
                                    {
                                        foreach (var subHeader in subHeaders)
                                        {
                                            dynValues_Row2.Add(new DatabaseHelper.childClass
                                            {
                                                CharacteristicValue = subHeader,
                                                tdColSpan = 1
                                            });
                                        }
                                    }
                                    else
                                    {
                                        // For headers without subheaders, add a single blank subheader row
                                        dynValues_Row2.Add(new DatabaseHelper.childClass
                                        {
                                            CharacteristicValue = "&nbsp;",
                                            tdColSpan = 1
                                        });
                                    }
                                }

                                Row1.listviewdata = dynValues_Row1;
                                Row2.listviewdata = dynValues_Row2;

                                list.Add(Row1);
                                list.Add(Row2);
                            }
                            data = new DatabaseHelper.ColumnNames_111();
                            data.Date = dt.Rows[i]["Date"].ToString();  // Assuming the column names match
                            data.Shift = dt.Rows[i]["Shift"].ToString();
                            data.ComponentId = dt.Rows[i]["ComponentID"].ToString();
                            data.SerialNo = dt.Rows[i]["SerialNo"].ToString();
                            data.SpindleLoad = dt.Rows[i]["SpindleLoad"].ToString();
                            data.Result = dt.Rows[i]["Result"].ToString();
                            data.Remarks = dt.Rows[i]["Remarks"].ToString();
                            int v=dt.Rows.Count;

                            dynValues = new List<DatabaseHelper.childClass>();
                            for (int j = 10; j < dt.Columns.Count; j++)
                            {
                                ColumnNames_111data = new DatabaseHelper.childClass();
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
                                }
                           }
                            data.listviewdata = dynValues;
                            list.Add(data);
                        }
                        Session["list"] = list;
                        listview1.DataSource = list;
                            listview1.DataBind();
                        }
                    }
                    else
                    {

                        listview1.DataSource = null;
                        listview1.DataBind();
                    }
                }
            }
        private DataTable FetchReportData(string machineID, string componentID, string operationNo, string serialNo, string[] status, string fromDate, string toDate, string[] dimension)
        { 
            dt=new DataTable();
            serialNo = "%" + serialNo + "%"; 
            serialNo = serialNo.Trim(); 
            machineID = machineID.Trim();
            componentID = componentID.Trim();

            dt = dbHelper.fetchDBdata(machineID, componentID, operationNo, serialNo, status, fromDate, toDate, dimension);
           
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
            try
            {
                List<DatabaseHelper.ColumnNames_111> listData = Session["list"] as List<DatabaseHelper.ColumnNames_111>;
                string appPath = HttpContext.Current.Server.MapPath("~");
                string destinationPath = Path.Combine(appPath, "GeneratedReports", "DynamicTableReport.xlsx");
                if (listData != null && listData.Count > 0)
                {
                    ExportTableToExcel(listData, destinationPath);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("No data available to export.");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Write("Export_ButtonClick :"+ex.Message);
            }
           
        }
       public void ExportTableToExcel(List<DatabaseHelper.ColumnNames_111> tableData, string filePath)
        {
            try
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using (ExcelPackage excel = new ExcelPackage(filePath))
                {
                    var Sheet = excel.Workbook.Worksheets["TableData"];
                    if (Sheet != null)
                    {                       
                        excel.Workbook.Worksheets.Delete("TableData");
                    }
                    var sheet = excel.Workbook.Worksheets.Add("TableData");
                    int row = 1;
                    int col = 1;

                    var staticHeaders = tableData[0]; //static and Dynamic Main headers (Row1)
                    var subHeaders = tableData[1]; // Row2 for dynamic subheaders

                    sheet.Cells[row, col++].Value = staticHeaders.Date;
                    sheet.Cells[row, col++].Value = staticHeaders.Shift;
                    sheet.Cells[row, col++].Value = staticHeaders.ComponentId;
                    sheet.Cells[row, col++].Value = staticHeaders.SerialNo;

                    sheet.Cells[row, col-4, row + 1, col - 4].Merge = true; 
                    sheet.Cells[row, col - 3, row + 1, col - 3].Merge = true; 
                    sheet.Cells[row, col-2, row + 1, col-2].Merge = true;
                    sheet.Cells[row, col - 1, row + 1, col - 1].Merge = true;
                 
                    //for (int i = 0; i < staticHeaders.listviewdata.Count; i++)
                    //{
                    //    var mainHeader = staticHeaders.listviewdata[i];
                    //    var subHeader = subHeaders.listviewdata[i]; // Get the corresponding subheader

                    //    if (subHeader != null) // Check if the subheader is not null
                    //    {
                    //        // Place the main header in the current cell
                    //        sheet.Cells[row, col].Value = mainHeader.CharacteristicValue; // Main header
                    //        sheet.Cells[row + 1, col].Value = subHeader.CharacteristicValue;
                    //        sheet.Cells[row, col, row + 1, col].Merge = false; // No merge for two rows
                    //    }
                    //    sheet.Column(col).Width = 35;
                    //    col++; // Move to the next column for the next subheader
                    //}

                  
                    for (int i = 0; i < staticHeaders.listviewdata.Count; i++)
                    {
                        var mainHeader = staticHeaders.listviewdata[i];

                       
                        int colspan = Math.Max(mainHeader.tdColSpan, 1); // Ensure colspan is at least 1
                        sheet.Cells[row, col, row, col + colspan - 1].Merge = true; 
                        sheet.Cells[row, col].Value = mainHeader.CharacteristicValue; // Main header text
                        sheet.Cells[row, col, row, col + colspan - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col, row, col + colspan - 1].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        // Add subheaders
                        for (int j = 0; j < colspan; j++)
                        {
                            // Ensure the subheader index does not exceed the available list
                            if (i + j < subHeaders.listviewdata.Count)
                            {
                                var subHeader = subHeaders.listviewdata[i + j];
                                if (mainHeader.CharacteristicValue == "Mechanical Size")
                                {
                                    sheet.Cells[row + 1, col + j].Value = ""; // Set the subheader cell to blank
                                }
                                else
                                {
                                    sheet.Cells[row + 1, col + j].Value = subHeader.CharacteristicValue;
                                }
                                sheet.Cells[row + 1, col + j].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            }
                        }

                        for (int j = 0; j < colspan; j++)
                        {
                            sheet.Column(col + j).Width = 20;
                        }
                        col += colspan; // Move to the next starting column based on colspan
                    }

                    // Adding the rest of the static headers
                    sheet.Cells[row, col].Value = staticHeaders.SpindleLoad;
                    sheet.Cells[row, ++col].Value = staticHeaders.Result;
                    sheet.Cells[row, ++col].Value = staticHeaders.Remarks;

                    sheet.Cells[row, col - 2, row + 1, col - 2].Merge = true; // SpindleLoad
                    sheet.Cells[row, col - 1, row + 1, col - 1].Merge = true; // Result
                    sheet.Cells[row, col , row + 1, col ].Merge = true;
                  
                    using (var headerRange = sheet.Cells[row, 1, row + 1, col])
                    {
                        // Styling for font, background color, etc.
                        headerRange.Style.Font.Bold = true;
                        headerRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        headerRange.Style.Fill.BackgroundColor.SetColor(Color.DarkBlue);
                        headerRange.Style.Font.Color.SetColor(Color.White);
                        headerRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        headerRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        headerRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        headerRange.Style.Border.Top.Color.SetColor(Color.White);
                        headerRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        headerRange.Style.Border.Left.Color.SetColor(Color.White);
                        headerRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        headerRange.Style.Border.Right.Color.SetColor(Color.White);
                        headerRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        headerRange.Style.Border.Bottom.Color.SetColor(Color.White);
                    }
                    
                    row = 3; // Move to data rows

                    // Populate table data
                    foreach (var record in tableData.Skip(2)) // Skip first two rows (headers)
                    {
                        col = 1;

                        sheet.Cells[row, col++].Value = record.Date;
                        sheet.Cells[row, col++].Value = record.Shift;
                        sheet.Cells[row, col++].Value = record.ComponentId;
                        sheet.Cells[row, col++].Value = record.SerialNo;

                        sheet.Column(col - 4).Width = 22;
                        sheet.Column(col - 3).Width = 10;
                        sheet.Column(col - 2).Width = 25;
                        sheet.Column(col - 1).Width = 15;

                        sheet.Cells[row, col - 4].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col - 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col - 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col - 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                   
                        sheet.Cells[row, col + 5].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col + 6].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col + 7].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                        foreach (var child in record.listviewdata)
                        {
                            var cell = sheet.Cells[row, col++];                          
                            
                            cell.Value = child.CharacteristicValue;
                            cell.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                            cell.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                            // Apply dynamic background color
                            if (!string.IsNullOrEmpty(child.backColor))
                            {
                                //Color bgColor = child.backColor.Contains("green") ? Color.LightGreen : Color.LightCoral;
                                //cell.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                //cell.Style.Fill.BackgroundColor.SetColor(bgColor);
                                Color fontColor = child.backColor.Contains("green") ? Color.Green : Color.Red;

                                // Set the font color
                                cell.Style.Font.Color.SetColor(fontColor);
                          }
                        }
                        sheet.Cells[row, col].Value = record.SpindleLoad;
                        sheet.Cells[row, col +1].Value = record.Result;
                        sheet.Cells[row, col + 2].Value = record.Remarks;

                        sheet.Column(col).Width = 12;
                        sheet.Column(col).Style.WrapText = true; // wrap Spindle Load

                        sheet.Column(col + 1).Width = 10;
                        sheet.Column(col + 2).Width = 45;
                        // Alignments for result columns
                        sheet.Cells[row, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[row, col + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


                        row++;
                    }

                    // Autofit columns and save
                   // sheet.Cells[sheet.Dimension.Address].AutoFitColumns();
                    excel.Save();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Downloading_Excel_Sheet : "+ex.Message);
            }
          
        }
    }
}



