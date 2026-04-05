
Imports System
Imports System.Windows.Forms

Module Startup
    <STAThread()> _
    Sub Main()
        Application.EnableVisualStyles()
        Application.SetCompatibleTextRenderingDefault(False)
        Application.Run(New Form1())
    End Sub
End Module
