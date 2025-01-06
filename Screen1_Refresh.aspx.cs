using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASP_Evaluation_Task
{
    public partial class Screen1_Refresh : System.Web.UI.Page
    {
       // private static int currentRowIndex = 0;
        private static Random random = new Random();

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    BindData();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in Page_Load: {ex.Message}");
            }
        }
        public void BindData()
        {
            try
            {
            List<Dictionary<string, object>> rows = new List<Dictionary<string, object>>();
            DatabaseHelper dbHelper = new DatabaseHelper();
            DataTable dt = dbHelper.GetRandomDataScreen("2024-04-10 06:00:00", "DayWiseTargetAndonView");
            Dictionary<string, object> dict = new Dictionary<string, object>();
            foreach (DataRow row in dt.Rows)
            {
                dict["TargetQty"] = random.Next(1000, 10000);//row["TargetQty"];
                dict["ProdQty"] = random.Next(1, 1000); //row["ProdQty"];
                dict["ShortfallQty"] = random.Next(1, 1000); //row["ShortfallQty"];
                dict["Rejection_Qty"] = random.Next(1, 1000); //row["Rejection_Qty"];
                dict["Rework_Qty"] = random.Next(1, 1000);//row["Rework_Qty"];
                //dict["Rework_www"] = random.Next(1, 1000);
                rows.Add(dict);
            }
            rptAndonData.DataSource = rows;
            rptAndonData.DataBind();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in BindData: {ex.Message}");
            }       
        }

        protected void timerUpdate_Tick(object sender, EventArgs e)
        {
            try
            {
                BindData();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in TimerUpdate_click :  {ex.Message}");
            }
          
        }
        public string RenderDynamicBoxes(object dataItem)
        {
            try
            {
                var row = (Dictionary<string, object>)dataItem;
                StringBuilder html = new StringBuilder(); // mutable string
                Dictionary<string, string> globalColumnName = new Dictionary<string, string>()
                {
                    {"TargetQty","Cumulative Target Qty."},
                    {"ProdQty","Cumulative Production Qty." },
                    {"ShortfallQty","Shortfall Qty." },
                    {"Rejection_Qty","Rejection Qty." },
                    {"Rework_Qty","Rework Qty." }

                };
                foreach (var column in row)
                {
                    if (column.Key != "Plant" && column.Key != "CellId") // Exclude specific columns
                    {
                        string columnName = globalColumnName[column.Key];
                        string columnValue = column.Value.ToString();
                        html.Append($@"
                         <div class='andon-box'>
                             <h2>{columnValue}</h2>
                             <p>{columnName}</p>
                         </div>");
                    }
                }
                return html.ToString();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error in RenderDynamicBoxes: {ex.Message}");
                return string.Empty;
            }
        }
    }
}