<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="Orders.aspx.cs" Inherits="PSD_Project.App.Pages.HandleOrders" %>

<asp:Content runat="server" ContentPlaceHolderID="Head"><title>Orders</title></asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Content">
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
                    <td>
                        <a href="Orders.aspx?handle=<%= order.Id %>">Handle</a>
                    </td>
                </tr>
            <% } %>
        </table>
    </div>
</asp:Content>