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
            height: 80px;
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
           padding-left:272px;
            flex-grow: 1;
            color: white;
            line-height: 50px;
            margin: 0;
        }

        .header-right-section {
            display: flex;
            align-items: center;
            
            padding-right: 20px;
           
           
            background-color: #0056b3;
            height: 80px;
            width:330px;
        }

        .max {
            flex-direction: column;
        }

        .maximize {
        }

        .header-left{
            height:80px;
            width:90px;
        }

        .header-info {
            text-align: right;
        }

        #currentDate {
            font-size: 34px;
            font-weight: bold;
            padding-right:55px;
            padding-left:55px;
           
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
            padding: 10px 0;
            margin: 0;
            font-size: 26px;
            font-weight: bold;
        }

        /* Grid Styling for Andon Boxes */
        .andon-grid {
            display: flex; /* Use flexbox for layout */
            flex-wrap: wrap; /* Allow items to wrap to the next row */
            column-gap: 160px; /* Add spacing between the boxes */
            row-gap: 50px;
            align-content: center;
            align-items: center;
            justify-content: center; /* Center-align the boxes in the row */
            padding:44px;
            margin: 0 auto; /* Center the container itself */
            /* Centers items horizontally */
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
            height: 270px;
        }

            .andon-box:hover {
                transform: scale(1.05);
            }

            .andon-box h2 {
                font-size: 80px;
                font-weight: bold;
                margin: 0;
                color: #ffff00; /* Bright yellow font */
                background-color: #0056b3;
                border-radius: 10px;
                padding-top: 26px;
                text-align: center;
                height: 120px;
            }

            .andon-box p {
                font-size: 34px;
                font-weight: 620;
                text-align: center;
                padding-top: 35px;
                border-radius: 10px;
                margin: 5px 0 0;
                color: #1b1b6f; /* column-count name */
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
            height:42px;
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
             height:18px;
            padding-top:16px;
            padding-bottom:16px;
            font-size: 18px;
            
           box-shadow: 0px 3px 6px rgba(0, 0, 0, 0.3);
            width: 82%;
           
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
                margin-right:0px;
               
            }
    </style>

</head>
<body>
    <!-- Header -->
    <div class="head-container">
        <img src="./Images/AmiTLogo.jpg" class="header-left" alt="Left Icon" />
        <div class="head-title1">CUMULATIVE TARGET VS ACTUAL</div>
        <div class="header-right-section">
            <div class="header-info">
                <span id="currentDate"></span>
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

    <asp:Repeater ID="rptAndonData" runat="server">
        <ItemTemplate>
            <div class="andon-grid">

                <%# RenderDynamicBoxes(Container.DataItem) %>
            </div>
        </ItemTemplate>
    </asp:Repeater>

    <!-- Footer Section -->
    <div class="footer">
        <!-- Text at the left corner -->
        <div class="footer-left">Powered by TPM-Trak &copy;</div>

        <!-- Dark blue centered styled box with "Welcome to TPM-Trak" -->
        <div class="footer-center-container">Welcome To TPM-Trak</div>

        <!-- Right section with an image -->
        <div class="footer-right">
            <img src="./Images/AmiTLogo.jpg" alt="Footer Logo" width="100" height="100" />
        </div>
    </div>
    <script>
        document.addEventListener('DOMContentLoaded', function () {
            const currentDate = new Date();
            const formattedDate = currentDate.toLocaleDateString('en-US', {
                day: '2-digit',
                month: '2-digit',
                year: 'numeric',
            });
            document.getElementById('currentDate').textContent = formattedDate;

            setInterval(() => {
                location.reload(true); // Forces a reload without using the cache
            }, 2000);
        });
</script>


</body>
</html>
