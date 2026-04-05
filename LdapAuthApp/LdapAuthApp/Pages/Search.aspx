<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Search.aspx.cs" Inherits="LdapAuthApp.SearchPage" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>User Search — LDAPS Auth</title>
    <link rel="stylesheet" href="../Styles/site.css" />
</head>
<body>
<form id="form1" runat="server">
<div class="page-wrapper">

  <div class="topbar">
    <a href="Dashboard.aspx" class="topbar-brand"><span class="dot"></span>LDAPS POC</a>
    <nav class="topbar-nav">
      <a href="Dashboard.aspx">Dashboard</a>
      <a href="Search.aspx" class="active">User Search</a>
      <asp:Button ID="btnLogout" runat="server" Text="Sign out"
          CssClass="topbar-nav-btn" OnClick="btnLogout_Click" />
    </nav>
  </div>

  <div class="main-content">

    <div class="section-header">
      <div>
        <h2>User Search</h2>
        <p>Search your LDAP directory &mdash; use * as a wildcard</p>
      </div>
    </div>

    <div class="card">
      <div class="card-title"><div class="icon">&#128269;</div>Search Filters</div>
      <div class="form-row">
        <div class="form-group">
          <label>Search by</label>
          <asp:DropDownList ID="ddlType" runat="server" CssClass="form-control">
            <asp:ListItem Value="any"  Text="Any field"                 Selected="True" />
            <asp:ListItem Value="sam"  Text="Username (sAMAccountName)" />
            <asp:ListItem Value="cn"   Text="Common Name (CN)"          />
            <asp:ListItem Value="mail" Text="Email address"             />
            <asp:ListItem Value="dept" Text="Department"                />
          </asp:DropDownList>
        </div>
        <div class="form-group">
          <label>Search term</label>
          <asp:TextBox ID="txtQuery" runat="server" CssClass="form-control"
              placeholder="e.g. john* or jane.doe@company.com" />
        </div>
      </div>
      <div class="form-row-3">
        <div class="form-group">
          <label>Override Base DN (optional)</label>
          <asp:TextBox ID="txtBaseDn" runat="server" CssClass="form-control mono"
              placeholder="Uses Web.config default" />
        </div>
        <div class="form-group">
          <label>Max results</label>
          <asp:TextBox ID="txtMax" runat="server" CssClass="form-control" Text="25" />
        </div>
        <div class="form-group" style="display:flex;align-items:flex-end;">
          <asp:Button ID="btnSearch" runat="server" Text="Search Directory"
              CssClass="btn btn-primary btn-full" OnClick="btnSearch_Click" />
        </div>
      </div>

      <asp:Panel ID="pnlFilterPreview" runat="server" Visible="false">
        <div class="sep"></div>
        <p class="text-small text-muted">LDAP filter:
          <code style="background:#f3f4f6;padding:2px 7px;border-radius:4px;font-family:Consolas,monospace;">
            <asp:Label ID="lblFilter" runat="server" /></code>
        </p>
      </asp:Panel>
    </div>

    <asp:Panel ID="pnlStatus" runat="server" Visible="false">
      <asp:Literal ID="litStatus" runat="server" />
    </asp:Panel>

    <asp:Panel ID="pnlResults" runat="server" Visible="false">
      <div class="card">
        <div class="card-title">
          <div class="icon">&#128101;</div>
          Results &mdash; <asp:Label ID="lblCount" runat="server" />
        </div>
        <asp:Repeater ID="rptResults" runat="server">
          <ItemTemplate>
            <div class="user-card">
              <div class="avatar"><%# GetInitials(Eval("DisplayName") as string) %></div>
              <div class="user-meta">
                <div class="name"><%# Server.HtmlEncode(Eval("DisplayName") as string ?? "-") %></div>
                <div class="sub">
                  <%# Server.HtmlEncode(Eval("Email") as string ?? "") %>
                  <%# !string.IsNullOrEmpty(Eval("Department") as string)
                      ? " &mdash; " + Server.HtmlEncode(Eval("Department") as string) : "" %>
                </div>
                <div class="attrs">
                  <span class="badge badge-gray"><%# Server.HtmlEncode(Eval("SamAccountName") as string ?? "") %></span>
                  <%# !string.IsNullOrEmpty(Eval("Title") as string)
                      ? "<span class=\"badge badge-purple\">" + Server.HtmlEncode(Eval("Title") as string) + "</span>" : "" %>
                </div>
                <p class="text-small text-muted" style="font-family:Consolas,monospace;font-size:11px;margin-top:4px;word-break:break-all;">
                  <%# Server.HtmlEncode(Eval("DistinguishedName") as string ?? "") %>
                </p>
              </div>
            </div>
          </ItemTemplate>
        </asp:Repeater>
      </div>
    </asp:Panel>

  </div>
</div>
</form>
</body>
</html>
