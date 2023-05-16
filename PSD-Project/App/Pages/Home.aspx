<%@ Page Language="C#" CodeBehind="Home.aspx.cs" Inherits="PSD_Project.App.Pages.Home" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Home</title>
</head>
<body>
<form style="width: 100%; display: flex; justify-content: center; align-items: center" id="HtmlForm" runat="server">
    <div style="width: 50%; display: flex; flex-flow: column;">
        <div>
            <h1>Welcome, <%=UserSession.Username%>! (<%=UserSession.Role.Name%>)</h1>
        </div>
    </div>
</form>
</body>
</html>