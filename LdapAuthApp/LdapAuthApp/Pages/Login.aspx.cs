using System;
using System.Web.Security;
using System.Web.UI;

namespace LdapAuthApp
{
    public partial class LoginPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && User.Identity.IsAuthenticated)
                Response.Redirect("~/Pages/Dashboard.aspx");
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ShowError("Please enter your domain username and password.");
                return;
            }

            try
            {
                LdapConfig config = LdapConfig.FromAppSettings();

                using (LdapService ldap = new LdapService(config))
                {
                    LdapUser user = ldap.Authenticate(username, password);

                    if (user != null)
                    {
                        // Store user + bind credentials in session
                        // so Dashboard/Search can query LDAP as this user
                        Session["CurrentUser"]     = user;
                        Session["BindUser"]        = ldap.LastBindUsername;
                        Session["BindPass"]        = password;

                        FormsAuthentication.SetAuthCookie(
                            user.SamAccountName ?? username, false);

                        string returnUrl = Request.QueryString["ReturnUrl"];
                        Response.Redirect(!string.IsNullOrEmpty(returnUrl)
                            ? returnUrl : "~/Pages/Dashboard.aspx");
                    }
                    else
                    {
                        ShowError("Invalid username or password. Please try again.");
                    }
                }
            }
            catch (Exception ex)
            {
                ShowError("Error connecting to directory: " + ex.Message);
            }
        }

        private void ShowError(string msg)
        {
            lblError.Text    = Server.HtmlEncode(msg);
            pnlError.Visible = true;
        }
    }
}
