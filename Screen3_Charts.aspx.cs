using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Policy;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;

namespace ASP_Evaluation_Task
{
    public partial class Screen3 : System.Web.UI.Page
    {
        public class ChartValues
        {
            public  List<string> DownIds { get; set; }
            public List<string> DowntimeFormatted { get; set; }
            public List<double> CumulativePercentages { get; set; }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!IsPostBack)
                {
                    sendToFrontEnd();
                }
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error in Page_Load: {ex.Message}");
            }
        }

        [WebMethod]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
        public static ChartValues sendToFrontEnd()
        {
            try
            {
                // Replace with actual database fetching logic
                DatabaseHelper dbHelper = new DatabaseHelper();
                DataTable dt = dbHelper.GetDowntimeData("2024-04-01", "2024-07-01");

                List<string> downIds = new List<string>();
                List<long> downtimes = new List<long>();
                List<string> formattedDowntimes = new List<string>();
                long totalDowntime = 0;

                foreach (DataRow row in dt.Rows)
                {
                    downIds.Add(row["DownID"].ToString());
                    long downtimeInSeconds = Convert.ToInt64(row["DowntimeInSeconds"]);
                    downtimes.Add(downtimeInSeconds);
                    formattedDowntimes.Add(FormatTime(downtimeInSeconds));
                    totalDowntime += downtimeInSeconds;
                }

                var combinedData = downIds.Zip(downtimes, (id, time) => new { DownID = id, DowntimeInSeconds = time })
                                          .OrderByDescending(x => x.DowntimeInSeconds)
                                          .ToList();

                downIds = combinedData.Select(x => x.DownID).ToList();
                downtimes = combinedData.Select(x => x.DowntimeInSeconds).ToList();

                formattedDowntimes.Clear();
                foreach (var downtime in downtimes)
                {
                    formattedDowntimes.Add(FormatTime(downtime));
                }

                List<double> cumulativePercentages = new List<double>();
                double cumulativeTotal = 0;

                foreach (var downtime in downtimes)
                {
                    cumulativeTotal += downtime;
                    cumulativePercentages.Add((cumulativeTotal / totalDowntime) * 100);
                }
                List<double> v = cumulativePercentages;
                return new ChartValues
                {
                    DownIds = downIds,
                    DowntimeFormatted = formattedDowntimes,
                    CumulativePercentages = cumulativePercentages
                };
            }
            catch (Exception ex)
            {
                // Log the error
                System.Diagnostics.Debug.WriteLine($"Error in sendToFrontEnd: {ex.Message}");
                throw new Exception("Failed to retrieve data.");
            }
        }


        // Helper method to convert downtime in seconds to HH:MM format
        public static string FormatTime(long seconds)
        {
            try
            {
                int hours = (int)(seconds / 3600);
                int minutes = (int)((seconds % 3600) / 60);
                return $"{hours:D2}:{minutes:D2}";  // Format as HH:MM
            }
            catch (Exception ex)
            {
                // Log the error and return default value
                System.Diagnostics.Debug.WriteLine($"Error in FormatTime: {ex.Message}");
                return "00:00";
            }
        }

        //public string pieChartData { get; set; }
        //public string paretoChartData { get; set; }
        //public string categories { get; set; }
    }
}

//AJAX Calls for getting pieChartData