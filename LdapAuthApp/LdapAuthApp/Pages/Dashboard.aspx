<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Dashboard.aspx.cs" Inherits="LdapAuthApp.DashboardPage" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Dashboard — LDAPS Auth</title>
    <link rel="stylesheet" href="../Styles/site.css" />
</head>
<body>
<form id="form1" runat="server">
<div class="page-wrapper">

  <div class="topbar">
    <a href="Dashboard.aspx" class="topbar-brand"><span class="dot"></span>LDAPS POC</a>
    <nav class="topbar-nav">
      <a href="Dashboard.aspx" class="active">Dashboard</a>
      <a href="Search.aspx">User Search</a>
      <asp:Button ID="btnLogout" runat="server" Text="Sign out"
          CssClass="topbar-nav-btn" OnClick="btnLogout_Click" />
    </nav>
  </div>

  <div class="main-content">

    <div class="section-header" style="margin-bottom:24px;">
      <div>
        <h2>Welcome, <asp:Label ID="lblDisplayName" runat="server" /></h2>
        <p>LDAPS authentication successful &mdash; session active</p>
      </div>
      <span class="badge badge-green">&#10003; Authenticated</span>
    </div>

    <div class="stat-grid">
      <div class="stat-card"><div class="label">Auth method</div><div class="value" style="font-size:16px;margin-top:4px;">LDAPS / SSL</div></div>
      <div class="stat-card"><div class="label">Port</div><div class="value">636</div></div>
      <div class="stat-card"><div class="label">Protocol</div><div class="value" style="font-size:16px;margin-top:4px;">LDAPv3</div></div>
    </div>

    <div class="card">
      <div class="card-title"><div class="icon">&#128100;</div>Your Directory Profile</div>
      <div style="display:flex;align-items:flex-start;gap:20px;">
        <div class="avatar" style="width:56px;height:56px;font-size:18px;">
          <asp:Label ID="lblInitials" runat="server" />
        </div>
        <div style="flex:1;">
          <div style="font-size:18px;font-weight:700;margin-bottom:4px;">
            <asp:Label ID="lblFullName" runat="server" />
          </div>
          <div class="text-muted text-small" style="margin-bottom:14px;">
            <asp:Label ID="lblTitle" runat="server" />
          </div>
          <table class="attr-table">
            <tr><td>Username</td>  <td><asp:Label ID="lblSam"   runat="server" /></td></tr>
            <tr><td>Email</td>     <td><asp:Label ID="lblEmail" runat="server" /></td></tr>
            <tr><td>Department</td><td><asp:Label ID="lblDept"  runat="server" /></td></tr>
            <tr><td>Phone</td>     <td><asp:Label ID="lblPhone" runat="server" /></td></tr>
            <tr><td>DN</td>        <td style="font-family:Consolas,monospace;font-size:11px;word-break:break-all;">
              <asp:Label ID="lblDN" runat="server" /></td></tr>
          </table>
        </div>
      </div>
    </div>

    <div class="card">
      <div class="card-title"><div class="icon">&#128101;</div>Group Memberships</div>
      <asp:Panel ID="pnlGroups" runat="server">
        <asp:Repeater ID="rptGroups" runat="server">
          <ItemTemplate>
            <span class="badge badge-purple" style="margin:3px;">
              <%# GetShortName(Container.DataItem.ToString()) %>
            </span>
          </ItemTemplate>
        </asp:Repeater>
      </asp:Panel>
      <asp:Panel ID="pnlNoGroups" runat="server" Visible="false">
        <p class="text-muted text-small">No group memberships found.</p>
      </asp:Panel>
    </div>

  </div>
</div>
</form>
</body>
</html>
