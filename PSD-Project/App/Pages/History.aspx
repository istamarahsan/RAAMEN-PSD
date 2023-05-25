<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="History.aspx.cs" Inherits="PSD_Project.App.Pages.History" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Transaction History</title>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Content">
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
                    <td><%= transaction.Date?.ToShortDateString() ?? "No date available" %></td>
                    <td><%= transaction.StaffName ?? "NOT FOUND" %></td>
                    <td><%= transaction.CustomerName ?? "NOT FOUND" %></td>
                    <td><%= transaction.Total %></td>
                    <td>
                        <a href="Transaction.aspx?transaction=<%= transaction.Id %>">View Details</a>
                    </td>
                </tr>
            <% } %>
        </table>
    </div>
</asp:Content>