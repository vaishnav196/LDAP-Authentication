
# SecureAdLogin (VB.NET, .NET Framework 2.0)

A minimal Windows Forms app that captures a user ID and password and validates against Active Directory using **LDAPS** (TCP 636).

## Prerequisites
- Visual Studio 2005 or 2008
- .NET Framework 2.0 target
- References: System.DirectoryServices, System.Configuration
- Domain Controller with a valid, trusted LDAPS certificate

## Configure
Edit `App.config`:
- `DcFqdn`: FQDN of your domain controller (e.g., dc01.contoso.com)
- `BaseDn`: e.g., `DC=contoso,DC=com`. If omitted, the app tries to auto-discover via rootDSE.

## Build & Run
1. Open `SecureAdLogin.sln` in Visual Studio.
2. Build solution (Debug/Release).
3. Run. Enter `DOMAIN\user` or `user@domain.com` and password.

## Notes
- The login uses LDAPS exclusively. If LDAPS is not configured on the DC or the certificate is untrusted, authentication will fail.
- To diagnose, test with `ldp.exe` against port 636 with SSL.
