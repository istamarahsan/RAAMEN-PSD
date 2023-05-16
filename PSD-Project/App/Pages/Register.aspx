<%@ Page Language="C#" CodeBehind="Register.aspx.cs" Inherits="PSD_Project.App.Pages.Register" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>RAAMEN</title>
    <style type="text/css">
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
            .footer{
                height: 100px;
                width: 100%;
                background-color: #34334c;
            }
            .auto-style1 {
                width: 45%;
                border: 2px solid black;
                background-color: burlywood;
                border-radius: 10px;
                margin: auto;
            }
            .auto-style2 {
                height: 37px;
                text-align: center;
                width: 321px;
            }
            .auto-style3 {
                width: 194px;
                text-align: center;
            }
            .auto-style4 {
                height: 26px;
                width: 194px;
                text-align: center;
            }
            .auto-style6 {
                margin-left: 0px;
            }
            .auto-style7 {
                text-align: center;
                width: 321px;
            }
            .auto-style8 {
                width: 321px;
            }
        
        </style>
</head>
<body>
<form id="form1" runat="server">

    <div class="header">
        <center>
            <h1>
                <bold>RAAMEN</bold>
            </h1>
        </center>
    </div>
    <div class="body">
        <div>
            <br/>
            <table class="auto-style1">
                <tr>
                    <td class="auto-style3">Username</td>
                    <td class="auto-style7">
                        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style4">Email</td>
                    <td class="auto-style2">
                        <asp:TextBox ID="TextBox2" runat="server" OnTextChanged="TextBox2_TextChanged"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Gender</td>
                    <td class="auto-style8">
                        <asp:RadioButtonList ID="GenderCheckBoxList" RepeatDirection="Horizontal" runat="server">
                            <asp:ListItem>
                                Male
                            </asp:ListItem>
                            <asp:ListItem>
                                Female
                            </asp:ListItem>
                            <asp:ListItem>
                                Rather Not Say
                            </asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Password</td>
                    <td class="auto-style7">
                        <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="auto-style3">Confirmation Password</td>
                    <td class="auto-style7">
                        <asp:TextBox ID="TextBox4" runat="server"></asp:TextBox>
                    </td>
                </tr>
            </table>

            <center>
                <br/>
                <asp:Button ID="Button1" runat="server" Text="Back" CssClass="auto-style6" Width="150px" BackColor="#D07402"/>
                <asp:Button ID="Button2" runat="server" Text="Create" Width="150px" BackColor="#D07402"/>
                <br/>
                <br/>
            </center>

        </div>

    </div>
    <div class="footer">

    </div>
</form>
</body>
</html>