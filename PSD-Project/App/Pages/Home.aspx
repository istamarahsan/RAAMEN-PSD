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
            <h1>Welcome, <%= CurrentUser.Username %>! (<%= CurrentUser.Rolename %>)</h1>
        </div>
        <% if (Customers != null)
           { %>
            <table>
                <% foreach (var customer in Customers)
                   { %>
                    <tr>
                        <td><%=customer.Username%></td>
                        <td><%=customer.Email%></td>
                        <td><%=customer.Rolename%></td>
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
                                <td><%=staff.Username%></td>
                                <td><%=staff.Email%></td>
                                <td><%=staff.Rolename%></td>
                            </tr>
        
                        <% } %>
                    </table>
                <% } %>
    </div>
</form>
</body>
</html>