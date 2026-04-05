
Imports System
Imports System.Windows.Forms

Public Class Form1
    Inherits Form

    Private lblTitle As Label
    Private lblUser As Label
    Private lblPass As Label
    Private txtUser As TextBox
    Private txtPass As TextBox
    Private btnLogin As Button
    Private lblResult As Label

    Public Sub New()
        Me.Text = "Secure AD Login (LDAPS)"
        Me.StartPosition = FormStartPosition.CenterScreen
        Me.FormBorderStyle = FormBorderStyle.FixedDialog
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.ClientSize = New Drawing.Size(420, 220)

        lblTitle = New Label()
        lblTitle.Text = "Enter your domain credentials"
        lblTitle.AutoSize = True
        lblTitle.Font = New Drawing.Font("Segoe UI", 10.0!, Drawing.FontStyle.Bold)
        lblTitle.Location = New Drawing.Point(20, 15)

        lblUser = New Label()
        lblUser.Text = "User ID (DOMAIN\user or user@domain):"
        lblUser.AutoSize = True
        lblUser.Location = New Drawing.Point(20, 55)

        txtUser = New TextBox()
        txtUser.Location = New Drawing.Point(23, 75)
        txtUser.Width = 360

        lblPass = New Label()
        lblPass.Text = "Password:"
        lblPass.AutoSize = True
        lblPass.Location = New Drawing.Point(20, 105)

        txtPass = New TextBox()
        txtPass.Location = New Drawing.Point(23, 125)
        txtPass.Width = 360
        txtPass.UseSystemPasswordChar = True

        btnLogin = New Button()
        btnLogin.Text = "Login"
        btnLogin.Location = New Drawing.Point(308, 160)
        AddHandler btnLogin.Click, AddressOf Me.BtnLogin_Click

        lblResult = New Label()
        lblResult.AutoSize = True
        lblResult.Location = New Drawing.Point(23, 165)
        lblResult.Width = 270
        lblResult.ForeColor = Drawing.Color.DimGray

        Me.Controls.Add(lblTitle)
        Me.Controls.Add(lblUser)
        Me.Controls.Add(txtUser)
        Me.Controls.Add(lblPass)
        Me.Controls.Add(txtPass)
        Me.Controls.Add(btnLogin)
        Me.Controls.Add(lblResult)
    End Sub

    Private Sub BtnLogin_Click(ByVal sender As Object, ByVal e As EventArgs)
        lblResult.ForeColor = Drawing.Color.DimGray
        lblResult.Text = "Authenticating securely (LDAPS)..."
        lblResult.Refresh()

        Dim dcFqdn As String = AppSettings.Getdata("DcFqdn") ' e.g., "dc01.contoso.com"
        Dim configuredBaseDn As String = AppSettings.Getdata("BaseDn") ' optional; leave blank to auto-discover

        If String.IsNullOrEmpty(dcFqdn) Then
            MessageBox.Show("Missing DcFqdn in configuration. Please set your domain controller FQDN.", _
                            "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            lblResult.Text = "Configuration error."
            Return
        End If

        Dim baseDn As String = configuredBaseDn
        If String.IsNullOrEmpty(baseDn) Then
            baseDn = LdapsAuth.GetDefaultNamingContext(dcFqdn)
            If String.IsNullOrEmpty(baseDn) Then
                MessageBox.Show("Could not auto-discover Base DN from rootDSE. Please set BaseDn in config.", _
                                "Auto-discovery Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning)
                lblResult.Text = "Could not discover Base DN."
                Return
            End If
        End If

        Dim username As String = Trim(txtUser.Text)
        Dim password As String = txtPass.Text

        If username.Length = 0 OrElse password.Length = 0 Then
            lblResult.ForeColor = Drawing.Color.Firebrick
            lblResult.Text = "Please enter both User ID and Password."
            Return
        End If

        Dim ok As Boolean = LdapsAuth.ValidateUserLDAPS(dcFqdn, baseDn, username, password)

        If ok Then
            lblResult.ForeColor = Drawing.Color.ForestGreen
            lblResult.Text = "Login successful (LDAPS)."
            MessageBox.Show("Welcome!", "Login Successful", MessageBoxButtons.OK, MessageBoxIcon.Information)
            ' TODO: proceed to next screen / enable app
        Else
            lblResult.ForeColor = Drawing.Color.Firebrick
            lblResult.Text = "Login failed. Invalid credentials or LDAPS error."
        End If
    End Sub
End Class
