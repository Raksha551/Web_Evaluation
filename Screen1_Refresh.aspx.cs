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
        private static int currentRowIndex = 0;
        private static Random random = new Random();

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            Response.Cache.SetNoStore();

            // Bind data on every request (including refresh)
            if (!IsPostBack)
            {
                BindData();
            }
        }



        public void BindData()
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

            //    List<Dictionary<string, object>> hardcodedRows = new List<Dictionary<string, object>>()
            //{
            //    new Dictionary<string, object> { { "Cumulative Target Qty", 3300 }, { "Cumulative Production Qty", 100 }, { "Shortfall Qty", 29 }, { "Rejection Qty", 45.67 }, { "Rework Qty", 286 },{"work",12 } },
            //    new Dictionary<string, object> { { "Cumulative Target Qty", 44000 }, { "Cumulative Production Qty", 300 }, { "Shortfall Qty", 412 }, { "Rejection Qty", 71 }, { "Rework Qty", 322 } },
            //    new Dictionary<string, object> { { "Cumulative Target Qty", 5500 }, { "Cumulative Production Qty", 400 }, { "Shortfall Qty", 77 }, { "Rejection Qty", 46 }, { "Rework Qty", 427 } },
            //    new Dictionary<string, object> { { "Cumulative Target Qty", 6600 }, { "Cumulative Production Qty", 800 }, { "Shortfall Qty", 99 }, { "Rejection Qty", 88 }, { "Rework Qty", 987 } }
            //};

            //    // Ensure index wraps around
            //    currentRowIndex = (currentRowIndex + 1) % hardcodedRows.Count;

            //    // Select the next hardcoded row
            //    var selectedRow = hardcodedRows[currentRowIndex];
            //    // System.Diagnostics.Debug.WriteLine($"Adding hardcoded row: {string.Join(", ", selectedRow.Select(kvp => $"{kvp.Key}: {kvp.Value}"))}");
            //    rows.Add(new Dictionary<string, object>(selectedRow));


            rptAndonData.DataSource = rows;
            rptAndonData.DataBind();
        }

        public string RenderDynamicBoxes(object dataItem)
        {
            var row = (Dictionary<string, object>)dataItem;
            StringBuilder html = new StringBuilder();

            Dictionary<string, string> globalColumnName = new Dictionary<string, string>()
            {
                { "TargetQty","Cumulative Target Qty."},
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

                    // Log data being rendered
                    //  System.Diagnostics.Debug.WriteLine($"Rendering: {columnName} = {columnValue}");

                    html.Append($@"
                <div class='andon-box'>
                    <h2>{columnValue}</h2>
                    <p>{columnName}</p>
                </div>");
                }
            }

            return html.ToString();
        }



    }
}