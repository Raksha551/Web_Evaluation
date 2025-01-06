<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Screen1_Refresh.aspx.cs" Inherits="ASP_Evaluation_Task.Screen1_Refresh" %>

<!DOCTYPE html>

<html>
<head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <title>Andon Dashboard</title>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            text-align: center;
            background-color: #f0f0f5;
        }

        /* Header Styling */
        .head-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: #486ab7;
            color: white;
            height: 70px;
            margin-bottom:0px;
            /* Remove padding */
        }

        .header-left {
            width: 60px;
            height: 70px;
            flex-shrink: 0;
            margin: 0;
        }

        .head-title1 {
            font-size: 34px;
            font-weight: bold;
            text-align: center;
            padding-left: 272px;
            flex-grow: 1;
            color: white;
            line-height: 45px;
            margin: 0;
        }

        .header-right-section {
            display: flex;
            align-items: center;
            padding-right: 20px;
            background-color: #0056b3;
            height: 70px;
            width: 330px;
        }

        .max {
            flex-direction: column;
        }

        .maximize {
        }

        .header-left {
            height: 70px;
            width: 90px;
        }

        .header-info {
            text-align: right;
        }

        #currentDate {
            font-size: 30px;
            font-weight: bold;
            padding-right: 60px;
            padding-left: 50px;
        }

        .header-icon1 {
            width: 20px;
            height: 20px;
            cursor: pointer;
            padding-left: 40px;
        }

        .header-icon {
            width: 28px;
            height: 28px;
            cursor: pointer;
        }
        /* Sub-header styling */
        h5 {
            background-color: #004080;
            color: white;
            padding: 6px 0;
            margin: 0;
            font-size: 24px;
            font-weight: bold;

        }

        /* Grid Styling for Andon Boxes */
        .andon-grid {
            display: flex; /* Use flexbox for layout */
            flex-wrap: wrap; /* Allow items to wrap to the next row */
            column-gap: 150px; /* Add spacing between the boxes */
            row-gap: 35px;
            align-content: center;
            align-items: center;
            justify-content: center; /* Center-align the boxes in the row */
            padding: 44px;
           margin-top:0px;
        }

        .maximize-icon {
            position: fixed;
            bottom: 10px; /* Position near the bottom */
            right: 10px; /* Position near the right */
            width: 20px;
            height: 20px;
            cursor: pointer;
        }

        .andon-box {
            background-color: #d7e0f3;
            color: white;
            border-radius: 12px;
            text-align: center;
            box-shadow: 0px 4px 6px rgba(0, 0, 0, 0.1);
            transition: transform 0.2s ease;
            width: 420px;
            height: 250px;
        }

/*            .andon-box:hover {
                transform: scale(1.05);
            }
*/
            .andon-box h2 {
                font-size: 84px;
                font-weight: bold;
                margin: 0;
                color: #ffff00; /* Bright yellow font */
                background-color: #0056b3;
                border-radius: 10px;
                padding-top: 22px;
                text-align: center;
                height: 120px;
                text-shadow: 2px 2px 4px yellow;
            }

            .andon-box p {
                font-size: 34px;
                font-weight: 620;
                text-align: center;
                padding-top: 24px;
                border-radius: 10px;
                margin: 5px 0 0;
                color: #1b1b6f; /* column-count name */
            }
            .andon-box:nth-child(2) > p{
                 padding-top: 16px;
            }
        /* Footer */
        .auto-refresh-message {
            font-size: 12px;
            color: gray;
            margin-top: 10px;
        }

        .footer {
            background-color: #486ab7;
            color: white;
            padding: 15px 20px;
            position: fixed;
            bottom: 0;
            width: 100%;
            height: 38px;
            display: flex;
            justify-content: space-between;
            align-items: center;
            box-shadow: 0px -2px 5px rgba(0, 0, 0, 0.2);
        }

        /* Left side "Powered by TPM-Trak" Text */
        .footer-left {
            font-size: 14px;
        }

        /* Centered dark blue styled box for "Welcome To TPM-Trak" */
        .footer-center-container {
            display: flex;
            justify-content: center;
            align-items: center;
            background-color: #004080;
            color: white;
            padding: 8px;
            height: 18px;
            padding-top: 16px;
            padding-bottom: 16px;
            font-size: 18px;
            box-shadow: 0px 3px 6px rgba(0, 0, 0, 0.3);
            width: 80%;
            margin: 10px auto;
            text-align: center;
        }


        .footer-right img {
            height: 50px;
            margin-left: 10px;
            margin-right: 35px;
        }

        .icon-container {
            display: grid;
            grid-template-columns: 1fr 1fr;
            grid-template-rows: repeat(2, auto);
            gap: 10px;
            align-items: center;
            margin-right: 0px;
        }
    </style>

</head>
<body>
   <form id="form1" runat="server">
    <div class="head-container">
        <img src="./Images/AmiTLogo.jpg" class="header-left" alt="Left Icon" />
        <div class="head-title1">CUMULATIVE TARGET VS ACTUAL</div>
        <div class="header-right-section">
            <div class="header-info">
                <asp:Label runat="server" id="currentDate"></asp:Label>
            </div>
            <div class="max">
                <div class="icon-container">
                    <img src="./Images/home.png" class="header-icon" alt="Home Icon" />
                    <img src="./Images/settings.png" class="header-icon" alt="Settings Icon" />
                    <div></div>
                    <img src="./Images/maximize.png" class="header-icon " alt="Help Icon" />
                </div>
            </div>
        </div>
    </div>

    <h5>KTA Spindle Tooling</h5>
<asp:ScriptManager ID="ScriptManager1" runat="server" />
<asp:UpdatePanel ID="upAndonData" runat="server" UpdateMode="Conditional">
    <ContentTemplate>
        <asp:Repeater ID="rptAndonData" runat="server">
            <ItemTemplate>
                <div class="andon-grid">
                    <%# RenderDynamicBoxes(Container.DataItem) %>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </ContentTemplate>

    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="timerUpdate" EventName="Tick" />
    </Triggers>
    </asp:UpdatePanel>

   
    <div class="footer">
       <div class="footer-left">Powered by TPM-Trak &copy;</div>
       <div class="footer-center-container">Welcome To TPM-Trak</div>
        <div class="footer-right">
            <img src="./Images/AmiTLogo.jpg" alt="Footer Logo" width="100" height="100" />
        </div>
    </div>
    <asp:Timer ID="timerUpdate" runat="server" Interval="5000" OnTick="timerUpdate_Tick" Enabled="True" />
       </form>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const currentDate = new Date();
            const formattedDate = currentDate.toLocaleDateString('en-IN', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
            });
            const [day, month, year] = formattedDate.split('/');
            const finalDate = `${day}-${month}-${year}`;
            document.getElementById('currentDate').textContent = finalDate;
        });
</script>


</body>
</html>
