<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="UpdateRamen.aspx.cs" Inherits="PSD_Project.App.Pages.UpdateRamen" %>
<%@ Import Namespace="PSD_Project.App.Common" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Update Ramen</title>
</asp:Content>
<asp:Content runat="server" ContentPlaceHolderID="Content">
    <div>
        <table>
            <tr>
                <td>Ramen ID</td>
                <td><%= Ramen.Id %></td>
            </tr>
            <td>Name</td>
            <td><%= Ramen.Name %></td>
            <td>
                <asp:TextBox runat="server" ID="RamenNameTextBox"/>
            </td>
            <td>
                <asp:Label runat="server" ID="RamenNameErrorLabel"></asp:Label>
            </td>
            <tr>
                <td>Meat</td>
                <td><%= Ramen.Meat.Name %></td>
                <td>
                    <asp:DropDownList runat="server" ID="MeatDropDown"/>
                </td>
            </tr>
            <tr>
                <td>Broth</td>
                <td><%= Ramen.Borth %></td>
                <td>
                    <asp:TextBox runat="server" ID="BrothTextBox"/>
                </td>
                <td>
                    <asp:Label runat="server" ID="BrothErrorLabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Price</td>
                <td><%= Ramen.Price.TryParseDouble().Ok().OrElse(0) %></td>
                <td>
                    <asp:TextBox runat="server" ID="PriceTextBox"/>
                </td>
                <td>
                    <asp:Label runat="server" ID="PriceErrorLabel"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Button runat="server" ID="BackButton" Text="Back" OnClick="BackButton_OnClick"/>
        <asp:Button runat="server" ID="SubmitButton" Text="Update" OnClick="SubmitButton_OnClick"/>
    </div>
</asp:Content>