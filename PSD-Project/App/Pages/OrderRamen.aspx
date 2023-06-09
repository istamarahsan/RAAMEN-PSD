<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="OrderRamen.aspx.cs" Inherits="PSD_Project.App.Pages.OrderRamen" %>
<%@ Import Namespace="System.Collections.Generic" %>
<%@ Import Namespace="System.Linq" %>
<%@ Import Namespace="PSD_Project.API.Features.Ramen" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Order</title>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Content">
    <div style="display: flex; flex-flow: row; flex-wrap: nowrap">
        <div style="display: flex; flex-flow: row; flex-wrap: wrap">
            <% foreach (var ramen in Ramen?.Values.ToList() ?? new List<Ramen>())
               { %>
                <div style="display: flex; flex-flow: column">
                    <div><%= ramen.Name %></div>
                    <div><%= ramen.Meat.Name %></div>
                    <div><%= ramen.Borth %></div>
                    <div>Rp. <%= ramen.Price %></div>
                    <div id="quantity-<%= ramen.Id %>"></div>
                    <div>
                        <a href="?ramenId=<%= ramen.Id %>&quantity=1">Add to Cart</a>
                    </div>
                </div>
            <% } %>
        </div>
        <div style="display: flex; flex-flow: column; justify-content: flex-start">
            <div style="display: flex; flex-flow: column" id="cart">
                <tb>
                    <tr>
                        <td>Item</td>
                        <td>Quantity</td>
                    </tr>
                    <% foreach (var item in Cart ?? new Dictionary<int, int>())
                       {
                           var details = Ramen[item.Key];
                    %>
                        <tr>
                            <td><%= details.Name %></td>
                            <td><%= item.Value %></td>
                        </tr>
                    <% } %>
                </tb>
            </div>
            <asp:Button runat="server" ID="ClearCartButton" OnClick="ClearCartButton_OnClick" Text="Clear"/>
            <asp:BUtton runat="server" ID="PlaceOrderButton" OnClick="PlaceOrderButton_OnClick" Text="Place Order"/>
        </div>
    </div>
</asp:Content>