using System;
using System.Collections.Generic;
using System.Web.Security;
using System.Web.UI;

namespace LdapAuthApp
{
    public partial class SearchPage : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!User.Identity.IsAuthenticated)
                Response.Redirect("~/Pages/Login.aspx");
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            string query = txtQuery.Text.Trim();
            if (string.IsNullOrEmpty(query))
            {
                ShowStatus("warning", "Please enter a search term. Use * as a wildcard, e.g. john*");
                return;
            }

            int max = 25;
            if (!int.TryParse(txtMax.Text, out max) || max < 1) max = 25;
            if (max > 200) max = 200;

            // Retrieve the bind credentials stored at login
            string bindUser = Session["BindUser"] as string;
            string bindPass = Session["BindPass"] as string;

            if (string.IsNullOrEmpty(bindUser))
            {
                ShowStatus("danger", "Session expired. Please sign in again.");
                return;
            }

            try
            {
                LdapConfig config = LdapConfig.FromAppSettings();
                if (!string.IsNullOrEmpty(txtBaseDn.Text.Trim()))
                    config.BaseDn = txtBaseDn.Text.Trim();

                using (LdapService ldap = new LdapService(config))
                {
                    string filter = BuildFilterDisplay(ddlType.SelectedValue, query);
                    lblFilter.Text        = Server.HtmlEncode(filter);
                    pnlFilterPreview.Visible = true;

                    IList<LdapUser> results = ldap.SearchUsers(query, bindUser, bindPass, max);

                    if (results.Count == 0)
                    {
                        ShowStatus("info", "No users matched your search.");
                        pnlResults.Visible = false;
                    }
                    else
                    {
                        pnlStatus.Visible     = false;
                        lblCount.Text         = results.Count + " user" + (results.Count != 1 ? "s" : "") + " found";
                        rptResults.DataSource = results;
                        rptResults.DataBind();
                        pnlResults.Visible    = true;
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStatus("danger", "Error: " + ex.Message);
            }
        }

        private string BuildFilterDisplay(string type, string term)
        {
            switch (type)
            {
                case "sam":  return string.Format("(&(objectClass=user)(sAMAccountName={0}))", term);
                case "cn":   return string.Format("(&(objectClass=user)(cn={0}))", term);
                case "mail": return string.Format("(&(objectClass=user)(mail={0}))", term);
                case "dept": return string.Format("(&(objectClass=user)(department={0}))", term);
                default:
                    return string.Format(
                        "(&(objectClass=user)(|(sAMAccountName={0})(cn={0})(displayName={0})(mail={0})))",
                        term);
            }
        }

        private void ShowStatus(string type, string msg)
        {
            string icon = type == "danger" ? "&#10007;" : type == "warning" ? "&#9888;" : "&#8505;";
            litStatus.Text = string.Format(
                "<div class=\"alert alert-{0}\"><span class=\"alert-icon\">{1}</span><span>{2}</span></div>",
                type, icon, msg);
            pnlStatus.Visible = true;
        }

        protected void btnLogout_Click(object sender, EventArgs e)
        {
            Session.Clear();
            FormsAuthentication.SignOut();
            Response.Redirect("~/Pages/Login.aspx");
        }

        protected string GetInitials(string name)
        {
            if (string.IsNullOrEmpty(name)) return "?";
            string[] parts = name.Trim().Split(' ');
            if (parts.Length >= 2)
                return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
        }
    }
}
