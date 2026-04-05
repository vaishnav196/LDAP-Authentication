<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="LdapAuthApp.LoginPage" %>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />
    <title>Sign In — LDAPS Auth</title>
    <link rel="stylesheet" href="../Styles/site.css" />
</head>
<body>
<div class="login-wrapper">
  <div class="login-box">
    <div class="login-header">
      <div class="logo">&#128274;</div>
      <h1>LDAPS Authentication</h1>
      <p>Sign in with your directory credentials</p>
    </div>

    <form id="form1" runat="server">
      <div class="card">
        <asp:Panel ID="pnlError" runat="server" Visible="false"
            CssClass="alert alert-danger" style="margin-top:0;margin-bottom:16px;">
          <span class="alert-icon">&#10007;</span>
          <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <div class="form-group">
          <label for="txtUsername">Username</label>
          <asp:TextBox ID="txtUsername" runat="server" CssClass="form-control"
              placeholder="e.g. john.doe" autocomplete="username" />
        </div>

        <div class="form-group">
          <label for="txtPassword">Password</label>
          <asp:TextBox ID="txtPassword" runat="server" CssClass="form-control"
              TextMode="Password" placeholder="••••••••" autocomplete="current-password" />
        </div>

        <div class="mt-8">
          <asp:Button ID="btnLogin" runat="server" Text="Sign in"
              CssClass="btn btn-primary btn-full" OnClick="btnLogin_Click" />
        </div>

        <p class="text-muted text-small" style="text-align:center;margin-top:16px;">
          Credentials validated via LDAPS (port 636)
        </p>
      </div>
    </form>

  </div>
</div>
</body>
</html>
