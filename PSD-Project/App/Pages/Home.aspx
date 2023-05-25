<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="Home.aspx.cs" Inherits="PSD_Project.App.Pages.Home" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Home</title>
</asp:Content>

<asp:Content runat="server" ID="MainContent" ContentPlaceHolderID="Content">
    <div style="width: 50%; display: flex; flex-flow: column;">
        <div>
            <h1>Welcome, <%= CurrentUser.Username %>! (<%= CurrentUser.Rolename %>)</h1>
        </div>
        <% if (Customers != null)
           { %>
            <table>
                <% foreach (var customer in Customers)
                   { %>
                    <tr>
                        <td><%= customer.Username %></td>
                        <td><%= customer.Email %></td>
                        <td><%= customer.Rolename %></td>
                    </tr>

                <% } %>
            </table>
        <% } %>
        <% if (Staff != null)
           { %>
            <table>
                <% foreach (var staff in Staff)
                   { %>
                    <tr>
                        <td><%= staff.Username %></td>
                        <td><%= staff.Email %></td>
                        <td><%= staff.Rolename %></td>
                    </tr>

                <% } %>
            </table>
        <% } %>
    </div>
</asp:Content>