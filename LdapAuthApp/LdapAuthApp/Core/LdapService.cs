using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace LdapAuthApp
{
    public class LdapService : IDisposable
    {
        private readonly LdapConfig _config;
        private bool _disposed;
        public string LastBindUsername { get; private set; }

        public LdapService(LdapConfig config)
        {
            if (config == null) throw new ArgumentNullException("config");
            _config = config;
        }

        // ── Authentication ────────────────────────────────────

        /// <summary>
        /// Explicitly authenticates the supplied username + password
        /// against the LDAP server. Does NOT use Windows/Kerberos SSO.
        /// The bind is done with Basic (simple) authentication over SSL.
        /// </summary>
        public LdapUser Authenticate(string username, string password)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return null;

            // Build the UPN: john.doe → john.doe@company.com
            // This is what AD expects for a simple bind
            string bindUsername = BuildBindUsername(username);
            LastBindUsername = bindUsername;

            string path = BuildPath(_config.BaseDn);

            try
            {
                // AuthenticationTypes.None forces a simple (Basic) bind
                // over the SSL connection — explicitly uses the typed credentials,
                // never the logged-in Windows user
                using (DirectoryEntry entry = new DirectoryEntry(
                    path,
                    bindUsername,
                    password,
                    AuthenticationTypes.None))
                {
                    // NativeObject forces the actual bind to happen
                    object native = entry.NativeObject;
                }
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                // 0x8007052E = invalid credentials
                // 0x80072020 = account restrictions / disabled
                // Any COMException here = authentication failed
                System.Diagnostics.Debug.WriteLine("LDAP bind failed: " + ex.Message);
                return null;
            }
           // catch (DirectoryServicesCOMException ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("LDAP bind failed: " + ex.Message);
            //    return null;
            //}

            // Bind succeeded — now fetch the user's full profile
            // Search using the same credentials the user just provided
            return FetchUserProfile(username, bindUsername, password);
        }

        // ── Private: fetch profile after successful auth ──────

        private LdapUser FetchUserProfile(string samAccountName, string bindUser, string bindPass)
        {
            string filter = string.Format(
                "(&(objectClass=user)(|(sAMAccountName={0})(userPrincipalName={1})))",
                EscapeLdapFilter(samAccountName),
                EscapeLdapFilter(bindUser));

            IList<LdapUser> results = SearchInternal(filter, 1, bindUser, bindPass);
            return results.Count > 0 ? results[0] : null;
        }

        // ── Public search methods ─────────────────────────────

        public LdapUser FindUserBySam(string samAccountName, string bindUser, string bindPass)
        {
            string filter = string.Format(
                "(&(objectClass=user)(sAMAccountName={0}))",
                EscapeLdapFilter(samAccountName));
            IList<LdapUser> r = SearchInternal(filter, 1, bindUser, bindPass);
            return r.Count > 0 ? r[0] : null;
        }

        public IList<LdapUser> SearchUsers(string query, string bindUser, string bindPass, int maxResults = 50)
        {
            string safe = EscapeLdapFilter(query);
            string filter = string.Format(
                "(&(objectClass=user)(|(sAMAccountName={0})(cn={0})(displayName={0})(mail={0})))",
                safe);
            return SearchInternal(filter, maxResults, bindUser, bindPass);
        }

        public bool IsUserInGroup(string samAccountName, string groupDn, string bindUser, string bindPass)
        {
            LdapUser user = FindUserBySam(samAccountName, bindUser, bindPass);
            if (user == null) return false;
            foreach (string g in user.Groups)
                if (string.Equals(g, groupDn, StringComparison.OrdinalIgnoreCase))
                    return true;
            return false;
        }

        // ── Core search ───────────────────────────────────────

        private IList<LdapUser> SearchInternal(string filter, int maxResults, string bindUser, string bindPass)
        {
            string path = BuildPath(_config.BaseDn);

            using (DirectoryEntry root = new DirectoryEntry(
                path, bindUser, bindPass, AuthenticationTypes.None))
            using (DirectorySearcher searcher = new DirectorySearcher(root))
            {
                searcher.Filter      = filter;
                searcher.SizeLimit   = maxResults;
                searcher.SearchScope = SearchScope.Subtree;

                foreach (string attr in new[] {
                    "cn", "displayName", "sAMAccountName", "mail",
                    "department", "title", "telephoneNumber",
                    "memberOf", "distinguishedName" })
                    searcher.PropertiesToLoad.Add(attr);

                List<LdapUser> users = new List<LdapUser>();
                using (SearchResultCollection results = searcher.FindAll())
                {
                    foreach (SearchResult result in results)
                    {
                        users.Add(MapToUser(result));
                        if (users.Count >= maxResults) break;
                    }
                }
                return users;
            }
        }

        // ── Helpers ───────────────────────────────────────────

        /// <summary>
        /// Converts a plain username to the format AD expects for a simple bind.
        /// Supports: john.doe → john.doe@company.com
        ///           john.doe@company.com → unchanged
        ///           COMPANY\john.doe → unchanged
        /// </summary>
        private string BuildBindUsername(string username)
        {
            // Already a UPN (user@domain) or NetBIOS (DOMAIN\user) — use as-is
            if (username.Contains("@") || username.Contains("\\"))
                return username;

            // Plain sAMAccountName — append domain to make a UPN
            if (!string.IsNullOrEmpty(_config.DomainName))
                return username + "@" + _config.DomainName;

            return username;
        }

        private string BuildPath(string dn)
        {
            return string.Format("LDAP://{0}:{1}/{2}",
                _config.Server, _config.Port, dn);
        }

        private LdapUser MapToUser(SearchResult result)
        {
            LdapUser user = new LdapUser
            {
                DistinguishedName = GetProp(result, "distinguishedName"),
                SamAccountName    = GetProp(result, "sAMAccountName"),
                DisplayName       = GetProp(result, "displayName") ?? GetProp(result, "cn"),
                Email             = GetProp(result, "mail"),
                Department        = GetProp(result, "department"),
                Title             = GetProp(result, "title"),
                Phone             = GetProp(result, "telephoneNumber")
            };

            if (result.Properties.Contains("memberOf"))
                foreach (object val in result.Properties["memberOf"])
                    user.Groups.Add(val.ToString());

            return user;
        }

        private string GetProp(SearchResult result, string name)
        {
            if (!result.Properties.Contains(name)) return null;
            ResultPropertyValueCollection vals = result.Properties[name];
            return vals.Count > 0 ? vals[0].ToString() : null;
        }

        private string EscapeLdapFilter(string value)
        {
            if (string.IsNullOrEmpty(value)) return value;
            StringBuilder sb = new StringBuilder();
            foreach (char c in value)
            {
                switch (c)
                {
                    case '\\': sb.Append("\\5c"); break;
                    case '*':  sb.Append("\\2a"); break;
                    case '(':  sb.Append("\\28"); break;
                    case ')':  sb.Append("\\29"); break;
                    case '\0': sb.Append("\\00"); break;
                    default:   sb.Append(c);      break;
                }
            }
            return sb.ToString();
        }

        public void Dispose()
        {
            _disposed = true;
        }
    }
}
