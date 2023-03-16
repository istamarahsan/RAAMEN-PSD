<%@ Page Language="C#" CodeBehind="Login.aspx.cs" Inherits="PSD_Project.App.Pages.Login" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<script runat="server">

</script>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Title</title>
</head>
<body>
<form runat="server">
    <div>
        <asp:Label runat="server" ID="UsernameLabel" >Username: </asp:Label>
        <asp:TextBox runat="server" ID="UsernameTextBox"></asp:TextBox>
    </div>
    <div>
        <asp:Label runat="server" ID="PasswordLabel">Password: </asp:Label>
        <asp:TextBox runat="server" ID="PasswordTextBox"></asp:TextBox>
    </div>
    <div>
        <asp:Button runat="server" Text="Submit" OnClick="OnClick"/>
        <asp:Label runat="server" ID="LoginResultLabel"></asp:Label>
        <asp:CheckBox runat="server" ID="RememberMeCheckBox"/>
    </div>
</form>
</body>
</html>