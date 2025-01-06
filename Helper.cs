using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace ASP_Evaluation_Task
{
    public class Helper
    {
        public static string ConvertToCustomDateFormat(string dateString)
        {
            try
            {
                if (dateString.Length == 16)
                {
                    dateString += ":00";
                }
                DateTime parsedDate;
                if (DateTime.TryParseExact(dateString, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                {
                    return parsedDate.ToString("yyyy-MM-dd HH:mm:ss");
                }
                else
                {
                    return string.Empty;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error converting date format: {ex.Message}");
                return string.Empty;
            }
        }
    }
}