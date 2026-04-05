using System;
using System.Collections.Generic;

namespace LdapAuthApp
{
    public class LdapUser
    {
        public string       DistinguishedName { get; set; }
        public string       SamAccountName    { get; set; }
        public string       DisplayName       { get; set; }
        public string       Email             { get; set; }
        public string       Department        { get; set; }
        public string       Title             { get; set; }
        public string       Phone             { get; set; }
        public List<string> Groups            { get; set; }

        public LdapUser()
        {
            Groups = new List<string>();
        }

        public string Initials
        {
            get
            {
                if (string.IsNullOrEmpty(DisplayName)) return "?";
                string[] parts = DisplayName.Trim().Split(' ');
                if (parts.Length >= 2)
                    return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
                return DisplayName.Substring(0, Math.Min(2, DisplayName.Length)).ToUpper();
            }
        }
    }
}
