
Imports System.Configuration

Public NotInheritable Class AppSettings
    Private Sub New()
    End Sub

    Public Shared Function Getdata(ByVal key As String) As String
        Try
            Dim v As String = ConfigurationManager.AppSettings(key)
            If v Is Nothing Then Return String.Empty
            Return v
        Catch
            Return String.Empty
        End Try
    End Function
End Class
