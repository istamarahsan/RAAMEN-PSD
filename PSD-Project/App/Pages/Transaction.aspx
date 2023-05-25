<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="Transaction.aspx.cs" Inherits="PSD_Project.App.Pages.Transaction" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Transaction ID: <%= TransactionId %></title>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Content">
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
</asp:Content>