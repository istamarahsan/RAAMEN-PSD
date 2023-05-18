<%@ Page Language="C#" CodeBehind="Orders.aspx.cs" Inherits="PSD_Project.App.Pages.HandleOrders" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Orders</title>
</head>
<body>
<form id="HtmlForm" runat="server">
    <div>
        <table>
            <tr>
                <td>ID</td>
                <td>Date</td>
                <td>Customer ID</td>
                <td>Customer</td>
                <td>Number of Items</td>
                <td>Action</td>
            </tr>
            <% foreach (var order in Orders)
               { %>
                <tr>
                    <td><%= order.Id %></td>
                    <td><%= order.TimeCreated.ToShortDateString() %></td>
                    <td><%= order.CustomerId %></td>
                    <td><%= order.CustomerUsername %></td>
                    <td><%= order.Items.Count %></td>
                    <td><a href="Orders.aspx?handle=<%=order.Id%>">Handle</a></td>
                </tr>
            <% } %>
        </table>
    </div>
</form>
</body>
</html>