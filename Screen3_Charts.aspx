<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Screen3_Charts.aspx.cs" Inherits="ASP_Evaluation_Task.Screen3" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <script src="https://code.highcharts.com/highcharts.js"></script>
    <script src="https://code.highcharts.com/modules/pareto.js"></script>
    <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>
    <title>Highcharts Visualization</title>
    <style>
        body {
            margin: 0;
            padding: 0;
        }

        .header-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: blue;
            color: white;
            height: 60px;
        }

        .header-left,
        .header-right {
            width: 80px;
            height: 70px;
            display: flex;
        }

        .header-title {
            padding-left: 260px;
            font-size: 30px;
            font-weight: bold;
            text-align: center;
            line-height: 40px;
            flex-grow: 1;
        }

        .header-right-section {
            display: flex;
            align-items: end;
            gap: 10px;
            justify-items: end;
            padding-right: 28px;
        }

        .header-info {
            display: flex;
            flex-direction: column;
            align-items: flex-end;
            text-align: right;
            justify-content: end;
            justify-items: flex-end
        }

        #currentDate {
            font-size: 20px;
            font-weight: bold;
        }

        .header-text {
            font-size: 20px;
            padding-right: 42px;
        }

        .header-settings-icon {
            width: 40px;
            height: 40px;
            cursor: pointer;
            padding-left: 26px;
            padding-bottom: 4px;
        }
    </style>
</head>
<body>
    <form runat="server">

        <div class="header-container">
            <img src=".\Images\AmiTLogo.jpg" class="header-left" alt="Left Icon" />
            <div class="header-title">
                Andon
            </div>
            <div class="header-right-section">
                <div class="header-info">
                    <span id="currentDate"></span>
                    <div class="header-text">SHIFT-B</div>
                </div>
                <img src=".\Images\settings.png" class="header-settings-icon" alt="Settings Icon" />
            </div>
            <img src=".\Images\AmiTLogo.jpg" class="header-right" alt="Left Icon" />
        </div>

        <div>
            <%--  <asp:ScriptManager ID="ScriptManager1" runat="server" EnablePageMethods="true"></asp:ScriptManager>--%>
            <div id="pieContainer" style="width: 100%; height: 380px;"></div>
            <div id="paretoContainer" style="width: 100%; height: 350px;"></div>
        </div>

        <script>
            document.addEventListener('DOMContentLoaded', function () {
                const currentDate = new Date();
                const formattedDate = currentDate.toLocaleDateString('en-IN', {
                    day: '2-digit',
                    month: 'short',
                    year: 'numeric',
                    hour: '2-digit',
                    minute: '2-digit',

                    hour12: false,
                });
                document.getElementById('currentDate').textContent = formattedDate;
            });
            document.addEventListener('DOMContentLoaded', function () {
                $(document).ready(function () {
                    $.ajax({
                        type: "POST",
                        url: "Screen3_Charts.aspx/sendToFrontEnd",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            console.log("Raw response:", response);

                            // Extract data
                            let plotData = response.d;
                            let formattedTime = plotData.DowntimeFormatted;
                            let categories = plotData.DownIds;
                            let percentages = plotData.CumulativePercentages;

                            // Prepare chart data
                            let chartData = [];
                            for (let i = 0; i < categories.length; i++) {
                                chartData.push({
                                    name: categories[i],
                                    y: parseFloat(percentages[i]),
                                    downtimeFormatted: formattedTime[i],
                                    downtimeRaw: parseInt(percentages[i] * 3600)
                                });
                            }
                            console.log("Chart Data:", chartData);
                            console.log("Percentages Array:", percentages);
                            console.log("Pie Chart Data:", chartData.map(item => ({
                                name: item.name,
                                y: item.y,
                                downtimeFormatted: item.downtimeFormatted,
                                downtimeRaw: item.downtimeRaw
                            })));
                            // Pie Chart


                            Highcharts.chart('pieContainer', {
                                chart: { type: 'pie' },
                                credits: { enabled: false },
                                title: { text: '' },
                                series: [{
                                    name: 'Downtime',
                                    data: chartData.map(item => ({
                                        name: item.name,               // Category name
                                        y: 100 - item.y,                     // Percentage value as a number
                                        downtimeFormatted: item.downtimeFormatted, // Formatted downtime
                                        downtimeRaw: item.downtimeRaw   // Raw downtime value
                                    })),
                                    dataLabels: {
                                        enabled: true,
                                        formatter: function () {
                                            return `${this.point.name}: ${this.point.downtimeFormatted} (${this.percentage.toFixed(2)}%)`;
                                        }
                                    }
                                }],
                                tooltip: {
                                    formatter: function () {
                                        return `<b>${this.key}</b><br>Downtime (HH:MM): ${this.point.downtimeFormatted}`;
                                    }
                                },
                                plotOptions: {
                                    pie: {
                                       // minSize: 100, // Minimum size of the pie chart
                                        size: '80%' //  overall size of the pie chart
                                    }
                                }
                            });


                            // Pareto Chart
                            Highcharts.chart('paretoContainer', {
                                chart: { type: 'column' },
                                credits: { enabled: false },
                                title: { text: '' },

                                xAxis: {
                                    categories: categories, // Use categories directly
                                    title: { text: 'DownID' },


                                },
                                yAxis: [
                                    {
                                        title: { text: 'Downtime (HH:MM)' },

                                        labels: {
                                            formatter: function () {
                                                const hours = Math.floor(this.value / 3600);
                                                const minutes = Math.floor((this.value % 3600) / 60);
                                                return `${hours}:${minutes.toString().padStart(2, '0')}`;
                                            }
                                        },

                                    },
                                    {
                                        title: { text: 'Cumulative Percentage' },
                                        opposite: true,
                                        max: 100,
                                        min: 0,
                                        tickPositions: [0, 100],
                                        endOnTick: false,   // Prevents going beyond the max value
                                        startOnTick: false
                                    },

                                ],
                                series: [
                                    {
                                        type: 'pareto',
                                        name: 'Cumulative %',
                                        yAxis: 1,
                                        data: percentages, // Use percentages directly
                                        marker: { enabled: true },
                                    },
                                    {
                                        type: 'column',
                                        name: 'Downtime (HH:MM)',
                                        reversed: 'true',
                                        data: chartData.map(item => ({
                                            y: Math.max(...chartData.map(c => c.downtimeRaw)) - item.downtimeRaw, //reversing the order
                                            downtimeFormatted: item.downtimeFormatted,

                                        })),
                                            dataLabels: {
                                            enabled: true,
                                            formatter: function () {
                                                return this.point.downtimeFormatted;
                                            }
                                        }
                                    }
                                ],
                                tooltip: {
                                    formatter: function () {
                                        if (this.series.type === 'column') {
                                            return `<b>${this.key}</b><br>Downtime: ${this.point.downtimeFormatted}`;
                                        } else {
                                            return `<b>${this.key}</b><br>Cumulative %: ${this.y.toFixed(1)}%`;
                                        }
                                    }
                                }
                            });
                        },
                        error: function (xhr, status, error) {
                            console.error("AJAX Error:", error);
                            console.error("Status:", status);
                            console.error("Response:", xhr.responseText);
                        }

                    });
                });
            });

         <%-- Highcharts.chart('paretoContainer', {
                chart: { type: 'column' },
                credits: {
                    enabled: false
                },
                title: { text: ' ' },
                ////xAxis: { categories: <%= categories %>, title: { text: 'DownID' } },
                yAxis: [{
                    title: { text: 'Downtime (HH:MM)' },
                    labels: {
                        formatter: function () {
                            // No need to convert here; the formatted data is already passed from the backend.
                            // return this.value;
                            const hours = Math.floor(this.value / 3600);
                            const minutes = Math.floor((this.value % 3600) / 60);
                            return `${hours}:${minutes.toString().padStart(2, '0')}`;
                        }
                    },
                    opposite: false
                }, {
                    title: { text: 'Cumulative Percentage' },
                    opposite: true,
                    max: 100
                }],
                series: [
                    {
                        type: 'column',
                        name: 'Downtime (HH:MM)',
                  //  //    data: <%= paretoChartData %>.map(function (item) { return { y: item.downtimeRaw, downtimeFormatted: item.downtimeFormatted }; }),
                        dataLabels: {
                            enabled: true,
                            formatter: function () {
                                // Show formatted downtime in HH:MM format
                                return this.point.downtimeFormatted;
                            }
                        }
                    },
                    {
                        type: 'spline',
                        name: 'Cumulative %',
                        yAxis: 1,
                //  //      data: <%= paretoChartData %>.map(function (item) { return item.cumulativePercentage; }),
                        marker: { enabled: true }
                    }
                ],
                tooltip: {
                    formatter: function () {
                        if (this.series.type === 'column') {
                            return `<b>${this.key}</b><br>Downtime: ${this.point.downtimeFormatted}`;
                        } else {
                            return `<b>${this.key}</b><br>Cumulative %: ${this.y.toFixed(1)}%`;
                        }
                    }
                }
            });--%>

</script>
    </form>
</body>
</html>
