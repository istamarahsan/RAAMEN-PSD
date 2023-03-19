<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="PSD_Project.App.Pages.Home" %>
<%@ Import Namespace="PSD_Project.App.Models" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Title</title>
</head>
<body>
<form id="HtmlForm" runat="server">
    <div style="flex-flow: row; justify-items: flex-start">
        <asp:Label runat="server" ID="RoleLabel"></asp:Label>
    </div>
    <% if (CurrentUserRole == UserRole.Staff || CurrentUserRole == UserRole.Admin)
       { %>
        <div>
            <h4>Customers</h4>
            <table>
                <tr>
                    <td>Id</td>
                    <td>Username</td>
                    <td>Email</td>
                    <td>Gender</td>
                </tr>
                <% foreach (var customer in Customers)
                   { %>
                    <tr>
                        <td><%= customer.Id%></td>
                        <td><%= customer.Username%></td>
                        <td><%= customer.Email%></td>
                        <td><%= customer.Gender%></td>
                    </tr>
                <% } %>
            </table>
        </div>
    <%}%>
    <% if (CurrentUserRole == UserRole.Admin)
       { %>
        <div>
            <h4>Staff</h4>
            <table>
                <tr>
                    <td>Id</td>
                    <td>Username</td>
                    <td>Email</td>
                    <td>Gender</td>
                </tr>
                <% foreach (var staff in Staff)
                   { %>
                    <tr>
                        <td><%= staff.Id%></td>
                        <td><%= staff.Username%></td>
                        <td><%= staff.Email%></td>
                        <td><%= staff.Gender%></td>
                    </tr>
                <% } %>
            </table>
        </div>
    <% } %>
</form>
</body>
</html>