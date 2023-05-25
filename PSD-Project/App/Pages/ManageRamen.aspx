<%@ Page Language="C#" MasterPageFile="Main.master" AutoEventWireup="true" CodeBehind="ManageRamen.aspx.cs" Inherits="PSD_Project.App.Pages.ManageRamen" %>
<%@ Import Namespace="PSD_Project.App.Common" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Manage Ramen</title>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Content">
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
                    <td><%= ramen.Id %></td>
                    <td><%= ramen.Name %></td>
                    <td><%= ramen.Borth %></td>
                    <td><%= ramen.Meat.Name %></td>
                    <td><%= ramen.Price.TryParseDouble().Ok().OrElse(0) %></td>
                    <td>
                        <a href="UpdateRamen.aspx?ramenId=<%= ramen.Id %>">Update</a>
                    </td>
                </tr>
            <% } %>
        </table>
        <a href="InsertRamen.aspx">New Ramen</a>
    </div>
</asp:Content>