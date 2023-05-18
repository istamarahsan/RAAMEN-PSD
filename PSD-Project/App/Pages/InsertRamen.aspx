<%@ Page Language="C#" CodeBehind="InsertRamen.aspx.cs" Inherits="PSD_Project.App.Pages.InsertRamen" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>InsertRamen</title>
</head>
<body>
<form id="HtmlForm" runat="server">
    <div>
        <table>
            <tr>
                <td>Name</td>
                <td><asp:TextBox runat="server" ID="RamenNameTextBox"/></td>
                <td><asp:Label runat="server" ID="RamenNameErrorLabel"></asp:Label></td>
            </tr>
            <tr>
                <td>Meat</td>
                <td><asp:DropDownList runat="server" ID="MeatDropDown"/></td>
            </tr>
            <tr>
                <td>Broth</td>
                <td><asp:TextBox runat="server" ID="BrothTextBox"/></td>
                <td><asp:Label runat="server" ID="BrothErrorLabel"></asp:Label></td>
            </tr>
            <tr>
                <td>Price</td>
                <td><asp:TextBox runat="server" ID="PriceTextBox"/></td>
                <td><asp:Label runat="server" ID="PriceErrorLabel"></asp:Label></td>
            </tr>
        </table>
        <asp:Button runat="server" ID="SubmitButton" OnClick="SubmitButton_OnClick" Text="Create" />
    </div>
</form>
</body>
</html>