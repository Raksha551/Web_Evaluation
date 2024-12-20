﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Screen2_Table.aspx.cs" Inherits="ASP_Evaluation_Task.Screen2_Table" %>

<!DOCTYPE html>

<html>
<head runat="server">
    <title>TPM-Trak Analytics</title>
    <%--<link href="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/css/select2.min.css" rel="stylesheet" />--%>
    <%-- <script src="https://code.jquery.com/jquery-3.6.0.min.js"></script>--%>
    <%-- <script src="https://cdn.jsdelivr.net/npm/select2@4.1.0-rc.0/dist/js/select2.min.js"></script>--%>
    <%--  <link href="c:\users\devteam\documents\demo_c#\web_asp.net\asp_training_project\asp_evaluation_task\lib\bootstrap\css\bootstrap.min.css" rel="stylesheet" />
    <script src="c:\users\devteam\documents\demo_c#\web_asp.net\asp_training_project\asp_evaluation_task\lib\bootstrap\js\bootstrap.min.js"></script>
    <link href="c:\users\devteam\documents\demo_c#\web_asp.net\asp_training_project\asp_evaluation_task\lib\bootstrap-multiselect\css\bootstrap-multiselect.min.css" rel="stylesheet" />
    <script src="c:\users\devteam\documents\demo_c#\web_asp.net\asp_training_project\asp_evaluation_task\lib\bootstrap-multiselect\js\bootstrap-multiselect.min.js"></script>--%>
    <%--<link href="file:///c:\users\devteam\documents\demo_c#\web_asp.net\asp_training_project\asp_evaluation_task\lib\bootstrap\css\bootstrap.min.css" rel="stylesheet" />--%>

    <%--  <link href="lib/bootstrap/css/bootstrap.min.css" rel="stylesheet" />

    <script src="lib/bootstrap/js/bootstrap.min.js"></script>

    --%>

    <script src="lib/jquery/jquery.min.js"></script>
    <link href="lib/bootstrap.min.css" rel="stylesheet" />
    <script src="lib/bootstrap.min.js"></script>



    <link href="lib/bootstrap-multiselect/css/bootstrap-multiselect.min.css" rel="stylesheet" />
    <script src="lib/bootstrap-multiselect/js/bootstrap-multiselect.min.js"></script>



    <script>
        $(document).ready(function () {
            // Enable multi-select dropdowns
            $('#<%= lbxCharacteristic.ClientID %>').multiselect({
                includeSelectAllOption: true,
                buttonWidth: '160px',




            });

            $('#<%= lbxStatus.ClientID %>').multiselect({
                includeSelectAllOption: true

            });

            //function updateSelectedCount(selector, label) {
            //    var count = $(selector).val() ? $(selector).val().length : 0;
            //    $(label).text(`All selected (${count})`);
            //}
        });
    </script>
    <style>
        body {
            font-family: Arial, sans-serif;
            margin: 0;
            padding: 0;
            text-align: center;
            background-color: #0a0c3f;
        }


        .container {
            margin-top: 80px;
            margin-bottom: 40px;
        }


        .head-container {
            display: flex;
            justify-content: space-between;
            align-items: center;
            background-color: #486ab7;
            color: white;
            height: 80px;
            padding: 0;
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            z-index: 1000;
            margin-bottom: 0px;
        }

        .header-left {
            width: 120px;
            height: 80px;
            flex-shrink: 0;
            margin: 0;
            border-radius: 10px;
        }

        .head-title1 {
            font-size: 24px;
            font-weight: bold;
            text-align: center;
            color: white;
            line-height: 60px;
            margin: 0;
        }

        .header-right-section {
            display: flex;
            align-items: center;
            gap: 10px;
            padding-right: 20px;
            padding-left: 50px;
        }



        .header-info {
            text-align: right;
        }


        .header-icon1 {
            width: 40px;
            height: 40px;
            cursor: pointer;
        }

        .header-icon {
            width: 70px;
            height: 70px;
            cursor: pointer;
            display: inline-flex;
        }



        #r, #h {
            padding: 16px;
            justify-content: center;
            align-content: center;
            align-items: center;
        }

        .right {
            margin-right: 0px;
            right: 0px;
            justify-content: center;
            align-content: center;
            align-items: center;
            padding-right: 0px;
            position: fixed;
        }

        .sidebar-icons {
            width: 40px;
            height: 35px;
            padding: 4px;
        }

        .sidebar {
            background-color: darkslategray;
            top: 9%;
            left: 0px;
            justify-content: left;
            font-size: 16px;
            height: 100%;
            width: 13%;
            position: fixed;
            text-align: left;
            display: flex;
            flex-direction: column;
        }

        .sidebar-items {
            color: #DADBDD;
            text-align: center;
            justify-content: center;
            padding-left: 15px;
            align-content: center;
            text-decoration: none;
        }

        .sidebar-list {
            display: flex;
            flex-direction: row;
            padding: 14px;
            color: white;
            border: 1px solid white;
        }

        #settings {
            padding: 12px;
        }

        #rightlogo {
            width: 120px;
            height: 80px;
            flex-shrink: 0;
            margin: 0;
            border-radius: 10px;
            padding-left: 4px;
            padding-right: 0px;
        }

        .row {
            border: 2px solid black;
            padding: 10px;
        }


        /* Default styles for the table */
        table {
            width: 100%; /* Adjust as needed */
            border-collapse: collapse;
            margin-top: 100px;
            margin: 40px 18%;
            position: relative;
            margin-left: 15%;
            width: 50%;
            border: 1px solid #DADBDD;
            table-layout: fixed;
            color: #DADBDD;
        }

        .grid-table {
            width: 150%; /* Adjust as needed */
            border-collapse: collapse;
            margin: 180px 18%;
            margin-left: 15%;
            padding-top: 17%;
            border: 1px solid #DADBDD;
            table-layout: fixed;
            color: #DADBDD;
            width: 80%;
            padding-left: 20%;
        }

        .scrollable-gridview-container {
            max-height: 400px; /* Set maximum height for vertical scrolling */
            overflow-y: auto; /* Enable vertical scrolling */
            overflow-x: auto; /* Enable horizontal scrolling for wide columns */
            border: 1px solid #ddd; /* Add a border for visibility */
        }



        .grid-table th, .grid-table td {
            max-width: 100px; /* Limit the width of cells */
            word-wrap: break-word; /* Allow wrapping of long words */
            overflow: hidden; /* Hide overflowing content */
            text-overflow: ellipsis; /* Add ellipsis for truncated content */
            white-space: normal; /* Allow wrapping */
            padding: 8px; /* Add padding for readability */
            border: 1px solid #ddd; /* Add borders to cells */
        }

        .gridview-header {
            word-wrap: break-word; /* Allow text to wrap */
            white-space: normal; /* Ensure normal text wrapping */
            overflow: hidden; /* Hide overflowing content */
            /*text-overflow: ellipsis; */ /* Add ellipsis for overflow */
            max-width: 200px; /* Set a maximum width for column headers */
            text-align: center; /* Align text to the left */
            padding: 8px;
        }

        table th, table td {
            border: 1px solid #ddd;
            padding: 6px;
            text-align: left;
        }

        table td {
            height: 50px;
            width: 180px;
        }

        table {
            margin-top: 0px;
        }

        .cellinput {
            width: 180px;
            height: 40px; /*                font-size:30px;*/
            font-size: 18px;
            color: black;
        }

        .cellserial {
            width: 80px;
            height: 40px;
            color: black;
        }

        .celllength {
            width: 200px;
            height: 40px;
            color: black;
        }

        .cellLong {
            width: 210px;
            color: black;
        }

        .cellinputchar, .cellinputstatus {
            font-size: 16px;
            width: 170px;
            max-height: 100px;
            overflow-y: auto;
        }

        .serialholder {
            width: 90px;
        }

        .cellname {
            font-size: 18px;
            width: 110px; /* Adjust this value to set your desired width */
            text-align: left; /* Align text to the left if needed */
            padding: 5px;
        }

        .cellnamechar {
            font-size: 18px;
            width: 130px; /* Adjust this value to set your desired width */
            text-align: left; /* Align text to the left if needed */
            padding: 5px;
        }

        .dynamic-column {
            text-align: center;
        }


        #viewbtn, #exportbtn {
            width: 80px;
            height: 40px;
            background-color: #0094ff;
            border-radius: 5px;
            font-size: 20px;
            font-weight: bold;
            padding-top: 2px;
        }

        .grid-table {
            width: 82%;
            border-collapse: collapse;
            margin-top: 48px;
        }

            .grid-table th, .grid-table td {
                padding: 10px;
                text-align: center;
                border: 1px solid #ddd;
            }

            .grid-table th {
                background-color: #0e273f; /* Green for headers */
                color: white;
            }

            .grid-table td {
                background-color: white;
                color: black;
            }

            /* Styling for sub-column headers (Top, Bottom) */


            /* Style for dynamic columns */
            .grid-table td {
                text-align: center;
            }

            /* Additional spacing for headers */
            .grid-table th {
                text-align: center;
            }


        #exportbtn {
            background-color: forestgreen;
        }

        .btn {
            display: flex;
            gap: 10px;
        }

        .listcss {
            color: black;
            backface-visibility: visible;
            border-radius: 5px;
            height: 50px;
        }
        /*
        .multiselect-container {
            height: 100vh !important
        }*/
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <div class="head-container">
                <div>
                    <img src="./Images/AmiTLogo.jpg" class="header-left" alt="Left Icon" />
                </div>
                <div class="head-title1">TPM-Trak Analysis</div>
                <div class="header-right-section">
                    <div class="header-info">
                        <span id="currentDate"></span>
                    </div>

                    <div class="right">
                        <img src="./Images/maximize.png" class="header-icon" id="r" alt="Maximize Icon" />
                        <img src="./Images/profile-user.png" class="header-icon" id="h" alt="Home Icon" />
                        <img src="./Images/AmiTLogo.jpg" class="header-icon" id="rightlogo" alt="AmiT Loga" />
                    </div>

                </div>
            </div>




            <div class="sidebar">
                <div class="sidebar-list">
                    <img src="./Images/list.png" class="sidebar-icons" runat="server" alt="Menu Icon" />
                </div>
                <div class="sidebar-list">
                    <img src="./Images/play-button.png" class="sidebar-icons" runat="server" alt="Play Icon" />
                    <a href="#" class="sidebar-items">Historical Analytics</a>
                </div>
                <div class="sidebar-list">
                    <img src="./Images/push-pin.png" class="sidebar-icons" runat="server" alt="Push Pin Icon" />
                    <a href="#" class="sidebar-items">Live Analytics</a>
                </div>
                <div class="sidebar-list">
                    <img src="./Images/atom.png" class="sidebar-icons" runat="server" alt="Atom Icon" />
                    <a href="#" class="sidebar-items">Smart Shop</a>
                </div>
                <div class="sidebar-list">
                    <img src="./Images/back-in-time.png" class="sidebar-icons" runat="server" alt="Back in Time Icon" />
                    <a href="#" class="sidebar-items">Bajaj Analytics</a>
                </div>
                <div class="sidebar-list">
                    <img src="./Images/contact.png" class="sidebar-icons" runat="server" alt="Contact Icon" />
                    <a href="#" class="sidebar-items">User Access</a>
                </div>
                <div class="sidebar-list">
                    <a href="#" class="sidebar-items" id="settings">Settings</a>
                </div>
            </div>

        </div>
        <asp:Table runat="server">
            <asp:TableRow>
                <asp:TableCell CssClass="cellname">Machine</asp:TableCell>
                <asp:TableCell CssClass="cellLong">
                    <asp:DropDownList runat="server" ID="machineDroplist" CssClass="celllength" AutoPostBack="true" OnSelectedIndexChanged="MachineDroplist_SelectedIndexChanged">
                        <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell CssClass="cellname">Component</asp:TableCell>
                <asp:TableCell CssClass="cellLong">
                    <asp:DropDownList runat="server" ID="componentDroplist" CssClass="celllength" OnSelectedIndexChanged="ComponentDroplist_SelectedIndexChanged">
                        <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell CssClass="cellname">Operation</asp:TableCell>
                <asp:TableCell CssClass="serialholder">
                    <asp:DropDownList runat="server" ID="OpDroplist" AutoPostBack="true" CssClass="cellserial" OnSelectedIndexChanged="Operation_SelectedIndexChanged">
                        <asp:ListItem Text="Select" Value="-1"></asp:ListItem>
                    </asp:DropDownList>
                </asp:TableCell>
                <asp:TableCell CssClass="cellnamechar">Characteristic</asp:TableCell>
                <asp:TableCell CssClass="listcss">
                    <asp:ListBox ID="lbxCharacteristic" runat="server" CssClass="form-control" Syle="color:black" SelectionMode="Multiple"></asp:ListBox>

                </asp:TableCell>
            </asp:TableRow>
            <asp:TableRow>
                <asp:TableCell CssClass="cellname">From Date</asp:TableCell>
                <asp:TableCell CssClass="cellLong">
                    <asp:TextBox runat="server" ID="txtFromDate" CssClass="celllength" TextMode="DateTimeLocal" Style="color: black"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="cellname">To Date</asp:TableCell>
                <asp:TableCell CssClass="cellLong">
                    <asp:TextBox runat="server" ID="txtToDate" CssClass="celllength" TextMode="DateTimeLocal" Text='<%# DateTime.Now.ToString("yyyy-MM-ddTHH:mm:SS") %>'></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="cellname">Serial No.</asp:TableCell>
                <asp:TableCell CssClass="serialholder">
                    <asp:TextBox runat="server" ID="txtSerialNo" CssClass="cellserial"></asp:TextBox>
                </asp:TableCell>
                <asp:TableCell CssClass="cellname">Status</asp:TableCell>
                <asp:TableCell CssClass="listcss">
                    <asp:ListBox ID="lbxStatus" runat="server" CssClass="form-control" SelectionMode="Multiple"></asp:ListBox>

                </asp:TableCell>
                <asp:TableCell CssClass="btn">
                    <asp:Button runat="server" ID="viewbtn" Text="View" OnClick=" ViewButton_Click" />
                    <asp:Button runat="server" ID="exportbtn" Text="Export" />
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        <%--  <asp:Label ID="lblMessage" runat="server" CssClass="message" Visible="false"></asp:Label>--%>
        <asp:ListView ID="listview1" ItemPlaceholderID="itemPlaceHolder" runat="server" OnItemDataBound="listview1_ItemDataBound" OnDataBound="listview1_DataBound">
            <LayoutTemplate>
                <table class="grid-table">
                    <thead>
                        <tr>
                            <th rowspan="2">Date</th>
                            <th rowspan="2">Shift</th>
                            <th rowspan="2">Component</th>
                            <th rowspan="2">Serial No.</th>

                            <!-- Dynamic headers placeholder -->
                            <td>
                                <asp:ListView runat="server" ID="phDynamicHeaders" ItemPlaceholderID="phDynamicHeaderItem" OnItemDataBound="phDynamicHeaders_ItemDataBound">
                                    <LayoutTemplate>
                                        <table>
                                            <tr>
                                                <th runat="server" id="phDynamicHeaderItem"></th>
                                            </tr>
                                        </table>
                                    </LayoutTemplate>
                                    <ItemTemplate>
                                        <asp:Literal ID="headerPlaceHolder" runat="server" />
                                        <asp:ListView ID="phDynamicSubHeaders" runat="server" ItemPlaceholderID="phDynamicSubHeaderItem"
                                            OnItemDataBound="phDynamicSubHeaders_ItemDataBound">
                                            <LayoutTemplate>
                                                <th runat="server" id="phDynamicSubHeaderItem" />
                                            </LayoutTemplate>
                                            <ItemTemplate>
                                                <th>
                                                    <asp:Literal ID="subHeaderPlaceHolder" runat="server" />
                                                </th>
                                            </ItemTemplate>
                                        </asp:ListView>
                                        <%-- <th><%# Eval("RenderData") %></th>--%>
                                    </ItemTemplate>
                                </asp:ListView>
                            </td>

                            <%--  <asp:ListView runat="server" ID="phDynamicHeaders" ItemPlaceholderID="phDynamicHeaderItem" OnItemDataBound="phDynamicHeaders_ItemDataBound">
                                <LayoutTemplate>

                                    <th runat="server" id="phDynamicHeaderItem"></th>


                                </LayoutTemplate>
                                <ItemTemplate>
                                    <asp:Literal ID="headerPlaceHolder" runat="server" />
                                    <asp:ListView ID="phDynamicSubHeaders" runat="server" ItemPlaceholderID="phDynamicSubHeaderItem"
                                        OnItemDataBound="phDynamicSubHeaders_ItemDataBound">
                                        <LayoutTemplate>

                                            <th runat="server" id="phDynamicSubHeaderItem" />
                                        </LayoutTemplate>
                                        <ItemTemplate>
                                            <th>
                                                <asp:Literal ID="subHeaderPlaceHolder" runat="server" />
                                            </th>
                                        </ItemTemplate>
                                    </asp:ListView>
                                    <%-- <th><%# Eval("RenderData") %></th>--%>
                                </ItemTemplate>
                            </asp:ListView>--%>

                            <!-- Static columns -->
                            <th rowspan="2">Spindle Load</th>
                            <th rowspan="2">Result</th>
                            <th rowspan="2">Remark</th>
                        </tr>
                    </thead>
                    <tbody>
                        <asp:PlaceHolder ID="itemPlaceHolder" runat="server" />
                    </tbody>
                </table>
            </LayoutTemplate>
            <ItemTemplate>

                <tr>
                    <!-- Static fields -->
                    <td class="static"><%# Eval("Date") %></td>
                    <td class="static"><%# Eval("Shift") %></td>
                    <td class="static"><%# Eval("ComponentID") %></td>
                    <td class="static"><%# Eval("SerialNo") %></td>

                    <!-- Dynamic data placeholder -->
                    <asp:ListView runat="server" ID="phDynamicData" ItemPlaceholderID="phDynamicDataItem" OnItemDataBound="phDynamicData_ItemDataBound">
                        <LayoutTemplate>

                            <td runat="server" id="phDynamicDataItem"></td>

                        </LayoutTemplate>
                        <ItemTemplate>
                            <td>
                                <asp:Literal ID="DataPlaceHolder" runat="server" /></td>
                        </ItemTemplate>
                    </asp:ListView>

                    <!-- Static fields -->
                    <td class="static"><%# Eval("SpindleLoad") %></td>
                    <td class="static"><%# Eval("Result") %></td>
                    <td class="static"><%# Eval("Remarks") %></td>
                </tr>

            </ItemTemplate>
            <EmptyDataTemplate>
                <table>
                    <tr>
                        <td style="font-size: 30px; color: white; padding: 10px;">Sorry!!! there is no data available for given user input</td>
                    </tr>
                </table>
            </EmptyDataTemplate>
        </asp:ListView>

    </form>

</body>
</html>

