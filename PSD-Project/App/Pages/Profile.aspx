<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="PSD_Project.App.Pages.Profile" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile Page</title>
    <style>
        .header{
            height: 100px;
            width: 100%;
            background-color: #9f1618;
        }
        .header h1{
            font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;
            text-align: center;
            color: white;
        }
        .content{
            min-height: 100vh;
            display: flex;
            flex-direction: column;
            justify-content: center;
            background-color: #fff5ee;
            text-align: center;
        }
        .updatebtn {
            background-color: #8F481E;

            padding: 5px 5px 5px 5px;
            border: none;
            border-radius: 5px;
        }
        .footer{
            height: 100px;
            width: 100%;
            background-color: #34334c;
        }
    </style>
    <style type="text/css">
        .auto-style1 {
            width: 31%;
            border: 2px solid #834d4d;
            background-color: #fff5ee;
            border-radius: 10px;
            margin: auto;
        }
        .auto-style2 {
            height: 26px;
        }
        .auto-style3 {
            width: 194px;
        }
        .auto-style4 {
            height: 26px;
            width: 194px;
        }
        .auto-style5 {
            width: 194px;
            height: 29px;
        }
        .auto-style6 {
            height: 29px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">

        <%--Header--%>
        <div class ="header">

            <h1>RAAMEN</h1>

        </div>
        <div class="content">
            <div>
                <br />
                <table class="auto-style1">
                    <tr>
                        <td class="auto-style3">Username</td>
                        <td>
                            <asp:TextBox ID="UsernameTextBox" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style3">Email</td>
                        <td>
                            <asp:TextBox ID="EmailTextBox" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style5">Gender</td>
                        <td class="auto-style6">
                            <asp:DropDownList ID="GenderDropDownList" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="auto-style4">Password</td>
                        <td class="auto-style2">
                            <asp:TextBox ID="PasswordTextBox" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div>
                <br />
                <asp:Button ID="UpdateButton" runat="server" Text="Update" Cssclass="updatebtn" />
            </div>
        </div>
        <%--footer--%>
        <div class="footer">

        </div>
    </form>
</body>
</html>
