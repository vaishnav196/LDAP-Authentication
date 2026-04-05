using System;
using System.Configuration;

namespace LdapAuthApp
{
    public class LdapConfig
    {
        public string Server          { get; set; }
        public int    Port            { get; set; }
        public string BaseDn          { get; set; }
        public string ServiceUserDn   { get; set; }
        public string ServicePassword { get; set; }
        public string CertThumbprint  { get; set; }
        public string DomainName      { get; set; } // e.g. company.com or COMPANY

        /// <summary>
        /// NoServiceAccount = user credentials used directly to search.
        /// Anonymous        = bind without credentials to search.
        /// ServiceAccount   = dedicated read-only account to search.
        /// </summary>
        public BindMode BindMode { get; set; }

        public static LdapConfig FromAppSettings()
        {
            string mode = ConfigurationManager.AppSettings["LDAP:BindMode"] ?? "UserBind";
            BindMode bindMode;
            if (!Enum.TryParse(mode, true, out bindMode))
                bindMode = BindMode.UserBind;

            return new LdapConfig
            {
                Server          = ConfigurationManager.AppSettings["LDAP:Server"],
                Port            = int.Parse(ConfigurationManager.AppSettings["LDAP:Port"] ?? "636"),
                BaseDn          = ConfigurationManager.AppSettings["LDAP:BaseDN"],
                ServiceUserDn   = ConfigurationManager.AppSettings["LDAP:ServiceAccount"] ?? "",
                ServicePassword = ConfigurationManager.AppSettings["LDAP:ServicePassword"] ?? "",
                CertThumbprint  = ConfigurationManager.AppSettings["LDAP:CertThumbprint"] ?? "",
                DomainName      = ConfigurationManager.AppSettings["LDAP:Domain"] ?? "",
                BindMode        = bindMode
            };
        }
    }

    public enum BindMode
    {
        /// <summary>
        /// No service account — user logs in with their own credentials.
        /// Their credentials are used to search their own profile.
        /// Most common when no dedicated service account exists.
        /// </summary>
        UserBind,

        /// <summary>
        /// A dedicated read-only service account is used to search.
        /// User credentials are verified by a separate bind.
        /// </summary>
        ServiceAccount,

        /// <summary>
        /// Anonymous bind used to search (server must permit this).
        /// User credentials verified by a separate bind.
        /// </summary>
        Anonymous
    }
}
