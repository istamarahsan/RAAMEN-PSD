<%@ Page Language="C#" CodeBehind="Transaction.aspx.cs" Inherits="PSD_Project.App.Pages.Transaction" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Transaction: <%= TransactionId %></title>
</head>
<body>
<form id="HtmlForm" runat="server">
    <div>
        <table>
            <tr>
                <td>Ramen ID</td>
                <td>Ramen</td>
                <td>Price</td>
                <td>Quantity</td>
                <td>Subtotal</td>
            </tr>
            <% foreach (var transactionDetail in TransactionDetails)
               { %>
                <tr>
                    <td><%= transactionDetail.RamenId %></td>
                    <td><%= transactionDetail.RamenName %></td>
                    <td><%= transactionDetail.Price %></td>
                    <td><%= transactionDetail.Quantity %></td>
                    <td><%= transactionDetail.Price * transactionDetail.Quantity %></td>
                </tr>
            <% } %>
        </table>
    </div>
</form>
</body>
</html>