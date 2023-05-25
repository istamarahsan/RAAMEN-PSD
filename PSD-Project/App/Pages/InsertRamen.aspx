<%@ Page Language="C#" MasterPageFile="Main.master" CodeBehind="InsertRamen.aspx.cs" Inherits="PSD_Project.App.Pages.InsertRamen" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <title>Insert Ramen</title>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="Content">
    <div>
        <table>
            <tr>
                <td>Name</td>
                <td>
                    <asp:TextBox runat="server" ID="RamenNameTextBox"/>
                </td>
                <td>
                    <asp:Label runat="server" ID="RamenNameErrorLabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Meat</td>
                <td>
                    <asp:DropDownList runat="server" ID="MeatDropDown"/>
                </td>
            </tr>
            <tr>
                <td>Broth</td>
                <td>
                    <asp:TextBox runat="server" ID="BrothTextBox"/>
                </td>
                <td>
                    <asp:Label runat="server" ID="BrothErrorLabel"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>Price</td>
                <td>
                    <asp:TextBox runat="server" ID="PriceTextBox"/>
                </td>
                <td>
                    <asp:Label runat="server" ID="PriceErrorLabel"></asp:Label>
                </td>
            </tr>
        </table>
        <asp:Button runat="server" ID="BackButton" OnClick="BackButton_OnClick" Text="Back"/>
        <asp:Button runat="server" ID="SubmitButton" OnClick="SubmitButton_OnClick" Text="Create"/>
    </div>
</asp:Content>