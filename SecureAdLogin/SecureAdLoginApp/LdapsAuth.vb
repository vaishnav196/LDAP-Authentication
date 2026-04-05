
Imports System
Imports System.DirectoryServices
Imports System.Runtime.InteropServices

Public Module LdapsAuth

    ''' <summary>
    ''' Validate user credentials via LDAPS (port 636).
    ''' </summary>
    Public Function ValidateUserLDAPS(ByVal dcFqdn As String, _
                                      ByVal baseDn As String, _
                                      ByVal username As String, _
                                      ByVal password As String) As Boolean
        ' e.g. LDAP://dc01.contoso.com:636/DC=contoso,DC=com
        Dim ldapPath As String = "LDAP://" & dcFqdn & ":636/" & baseDn

        Try
            Using entry As New DirectoryEntry(ldapPath, username, password, _
                                              AuthenticationTypes.Secure Or AuthenticationTypes.SecureSocketsLayer)
                ' Forces bind — throws if invalid creds or SSL/cert issues.
                Dim native As Object = entry.NativeObject
                Return True
            End Using

        Catch ex As COMException
            ' TIP: Log ex.ToString() to a file/event log in production for troubleshooting.
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Auto-discovers the default naming context (Base DN) from rootDSE.
    ''' Tries LDAPS rootDSE first, then plain LDAP as a read-only fallback.
    ''' </summary>
    Public Function GetDefaultNamingContext(ByVal dcFqdn As String) As String
        ' Try LDAPS rootDSE first
        Dim rootLdaps As String = "LDAP://" & dcFqdn & ":636/rootDSE"
        Dim val As String = ReadRootDseValue(rootLdaps, "defaultNamingContext")
        If Not String.IsNullOrEmpty(val) Then Return val

        ' Fallback: plain LDAP rootDSE (read). This does NOT authenticate credentials.
        Dim rootLdap As String = "LDAP://" & dcFqdn & "/rootDSE"
        val = ReadRootDseValue(rootLdap, "defaultNamingContext")
        Return val
    End Function

    Private Function ReadRootDseValue(ByVal ldapPath As String, ByVal attributeName As String) As String
        Try
            Using de As New DirectoryEntry(ldapPath)
                Dim v As Object = de.Properties(attributeName).Value
                If v <> "" Then Return v.ToString()
            End Using
        Catch
            ' ignore
        End Try
        Return Nothing
    End Function

End Module
