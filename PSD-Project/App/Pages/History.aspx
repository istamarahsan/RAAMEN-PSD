<%@ Page Language="C#" CodeBehind="History.aspx.cs" Inherits="PSD_Project.App.Pages.History" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Transaction History</title>
</head>
<body>
<form runat="server">
    <div>
        <table style="border: medium #34334c;">
            <tr>
                <td>Date</td>
                <td>StaffId</td>
                <td>CustomerId</td>
                <td>Total</td>
            </tr>
            <% foreach (var transaction in Transactions)
               { %>
                <tr>
                    <td><%=transaction.Date?.ToShortDateString() ?? "No date available"%></td>
                    <td><%=transaction.StaffName ?? "NOT FOUND"%></td>
                    <td><%=transaction.CustomerName ?? "NOT FOUND"%></td>
                    <td><%=transaction.Total%></td>
                </tr>
            <% } %>
        </table>
    </div>
</form>
</body>
</html>