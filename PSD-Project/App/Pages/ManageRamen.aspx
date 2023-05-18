<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManageRamen.aspx.cs" Inherits="PSD_Project.App.Pages.ManageRamen" %>
<%@ Import Namespace="PSD_Project.App.Common" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Manage Ramen</title>
</head>
<body>
<form id="form1" runat="server">
    <div>
        <table>
            <tr>
                <td>Ramen ID</td>
                <td>Name</td>
                <td>Broth</td>
                <td>Meat</td>
                <td>Price</td>
                <td>Action</td>
            </tr>
            <% foreach (var ramen in Ramen)
               { %>
                <tr>
                    <td><%=ramen.Id%></td>
                    <td><%=ramen.Name%></td>
                    <td><%=ramen.Borth%></td>
                    <td><%=ramen.Meat.Name%></td>
                    <td><%=ramen.Price.TryParseDouble().Ok().OrElse(0)%></td>
                    <td><a>Update</a></td>
                </tr>
            <% } %>
        </table>
        <a>New Ramen</a>
    </div>
</form>
</body>
</html>