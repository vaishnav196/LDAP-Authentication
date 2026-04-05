using System;
using System.Web.Security;
using System.Web.UI;

namespace LdapAuthApp
{
    public partial class DashboardPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
            { Response.Redirect("~/Pages/Login.aspx"); return; }

            if (!IsPostBack) PopulateProfile();
        }

        private void PopulateProfile()
        {
            LdapUser user = Session["CurrentUser"] as LdapUser;

            if (user == null)
            {
                // Session expired — re-fetch using stored bind credentials
                string bindUser = Session["BindUser"] as string;
                string bindPass = Session["BindPass"] as string;

                if (!string.IsNullOrEmpty(bindUser) && !string.IsNullOrEmpty(bindPass))
                {
                    try
                    {
                        using (LdapService ldap = new LdapService(LdapConfig.FromAppSettings()))
                        {
                            user = ldap.FindUserBySam(User.Identity.Name, bindUser, bindPass);
                            if (user != null) Session["CurrentUser"] = user;
                        }
                    }
                    catch { }
                }

                if (user == null)
                {
                    // Can't re-fetch — fall back to name only
                    user = new LdapUser
                    {
                        SamAccountName = User.Identity.Name,
                        DisplayName    = User.Identity.Name
                    };
                }
            }

            lblDisplayName.Text = Server.HtmlEncode(user.DisplayName ?? user.SamAccountName);
            lblInitials.Text    = Server.HtmlEncode(user.Initials);
            lblFullName.Text    = Server.HtmlEncode(user.DisplayName ?? "-");
            lblSam.Text         = Server.HtmlEncode(user.SamAccountName ?? "-");
            lblEmail.Text       = Server.HtmlEncode(user.Email ?? "-");
            lblDept.Text        = Server.HtmlEncode(user.Department ?? "-");
            lblPhone.Text       = Server.HtmlEncode(user.Phone ?? "-");
            lblDN.Text          = Server.HtmlEncode(user.DistinguishedName ?? "-");
            lblTitle.Text       = Server.HtmlEncode(user.Title ?? "");

            if (user.Groups != null && user.Groups.Count > 0)
            {
                rptGroups.DataSource = user.Groups;
                rptGroups.DataBind();
            }
            else
            {
                pnlGroups.Visible   = false;
                pnlNoGroups.Visible = true;
            }
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Pages/Login.aspx");
        }

        protected string GetShortName(string dn)
        {
            if (string.IsNullOrEmpty(dn)) return dn;
            string[] parts = dn.Split(',');
            if (parts.Length > 0 && parts[0].StartsWith("CN=", StringComparison.OrdinalIgnoreCase))
                return parts[0].Substring(3);
            return dn;
        }
    }
}
