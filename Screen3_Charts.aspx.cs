using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ASP_Evaluation_Task
{
    public partial class Screen3 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindData();
            }
        }

        protected void BindData()
        {

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

            JavaScriptSerializer serializer = new JavaScriptSerializer();

            pieChartData = serializer.Serialize(
                downIds.Zip(formattedDowntimes, (id, time) => new { name = id, y = downtimes[downIds.IndexOf(id)], downtimeFormatted = time }));

            var paretoData = downtimes.Zip(cumulativePercentages, (downtime, cumulativePercentage) => new
            {
                downtimeRaw = downtime, // Raw downtime in seconds
                downtimeFormatted = FormatTime(downtime),
                cumulativePercentage
            });
            paretoChartData = serializer.Serialize(paretoData);

            // Serialize categories
            categories = serializer.Serialize(downIds);
        }

        // Helper method to convert downtime in seconds to HH:MM format
        public string FormatTime(long seconds)
        {
            int hours = (int)(seconds / 3600);
            int minutes = (int)((seconds % 3600) / 60);
            return $"{hours:D2}:{minutes:D2}";  // Format as HH:MM
        }

        public string pieChartData { get; set; }
        public string paretoChartData { get; set; }
        public string categories { get; set; }

    }
}
