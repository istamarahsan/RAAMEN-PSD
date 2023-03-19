<%@ Page Language="C#" CodeBehind="Profile.aspx.cs" Inherits="PSD_Project.App.Pages.Profile" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Profile</title>
</head>
<body>
<form id="HtmlForm" runat="server">
    <div style="display: flex; flex-flow: column; gap: 10px">
            <div>
                <asp:Label runat="server" Text="Username"></asp:Label>
                <asp:TextBox runat="server" Text="" ID="UsernameTextBox"></asp:TextBox>
                <asp:Label runat="server" ID="UsernameErrorLabel"></asp:Label>
            </div>
            <div>
                <asp:Label runat="server" Text="Email"></asp:Label>
                <asp:TextBox runat="server" Text="" ID="EmailTextBox"></asp:TextBox>
                <asp:Label runat="server" ID="EmailErrorLabel"></asp:Label>
            </div>
            <div>
                <asp:Label runat="server" Text="Gender"></asp:Label>
                <div>
                    <asp:RadioButtonList runat="server" ID="GenderRadioButtons" AutoPostBack="True">
                        <asp:ListItem Text="Female"/>
                        <asp:ListItem Text="Male"/>
                        <asp:ListItem Text="Rather Not Say"/>
                    </asp:RadioButtonList>
                </div>
                <asp:Label runat="server" ID="GenderErrorLabel"></asp:Label>
            </div>
            <div>
                <asp:Label runat="server" Text="Password"></asp:Label>
                <asp:TextBox runat="server" TextMode="Password" Text="" ID="PasswordTextBox"></asp:TextBox>
                <asp:Label runat="server" ID="PasswordErrorLabel"></asp:Label>
            </div>
            <asp:Button runat="server" Text="Update Profile" ID="SubmitButton" OnClick="OnSubmitButtonClicked"/>
            <asp:Label runat="server" Text="" ID="UpdateProfileResultLabel"></asp:Label>
        </div>
</form>
</body>
</html>