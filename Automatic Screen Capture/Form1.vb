Imports System
Imports System.IO
Imports System.Windows.Forms
Imports System.Drawing
Imports System.Collections.Generic
Imports System.Object

Public Class Form1
    Public Declare Function GetAsyncKeyState Lib "user32" (ByVal vkey As Int32) As Short
    Public Const VK_SNAPSHOT As Integer = &H2C 'PrintScreen key
    Public Const WM_HOTKEY As Integer = &H312
    Public Declare Function RegisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer, ByVal fsModifiers As Integer, ByVal vk As Integer) As Integer
    Public Declare Function UnregisterHotKey Lib "user32" (ByVal hwnd As IntPtr, ByVal id As Integer) As Integer

    Public Sub takescrshot()
        Dim bounds As Rectangle
        Dim screenshot As Bitmap
        Dim graph As Graphics
        bounds = Screen.PrimaryScreen.Bounds
        screenshot = New Bitmap(bounds.Width, bounds.Height, Imaging.PixelFormat.Format32bppPArgb)
        graph = Graphics.FromImage(screenshot)
        graph.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy)
        PictureBox1.Image = screenshot
    End Sub
    Public Sub foldercheck()
        If (Not IO.Directory.Exists(TextBox1.Text)) Then
            IO.Directory.CreateDirectory(TextBox1.Text)
        End If
    End Sub
    Public Sub showevent()
        NotifyIcon1.Visible = False
        Visible = True
        WindowState = FormWindowState.Normal
    End Sub

    Protected Overrides Sub WndProc(ByRef m As Message)
        If m.Msg = WM_HOTKEY Then
            btnCapture.PerformClick()
        End If
        MyBase.WndProc(m)
    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Call RegisterHotKey(Me.Handle, 9, 0, VK_SNAPSHOT)
        takescrshot()
        TextBox1.Text = Environment.CurrentDirectory & "\ASC"
    End Sub

    Private Sub Form1_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Resize
        Dim f As Form = sender
        If f.WindowState = FormWindowState.Minimized Then
            Visible = False
            NotifyIcon1.Visible = True
            NotifyIcon1.ShowBalloonTip(500)
        End If
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs) Handles MyBase.FormClosing
        Call UnregisterHotKey(Me.Handle, 9)
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnOpen.Click
        If (Not IO.Directory.Exists(TextBox1.Text)) Then
            Dim result As Integer = MessageBox.Show("Folder " & TextBox1.Text & " not found. Do you want to manually create it?", "Automatic Screen Capture", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
            If result = DialogResult.Yes Then
                IO.Directory.CreateDirectory(TextBox1.Text)
                MessageBox.Show("This folder is created", "Automatic Screen Capture", MessageBoxButtons.OK, MessageBoxIcon.Information)
                Process.Start("explorer.exe", TextBox1.Text)
            ElseIf result = DialogResult.No Then
                MessageBox.Show("This folder is still missing.", "Automatic Screen Capture", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            End If
        Else
            Process.Start("explorer.exe", TextBox1.Text)
        End If

    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnCapture.Click
        takescrshot()
        foldercheck()
        PictureBox1.Image.Save(TextBox1.Text & "\" & "ASC_" & Now.ToString("yyyyMMdd") & "_" & Now.ToString("HH_mm_ss") & ".png", Imaging.ImageFormat.Png)
        Label1.Text = "Taken on " & Now.ToString("dd/MM/yyyy") & " at " & Now.ToString("HH:mm:ss")
    End Sub

    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles Label3.Click
        If (FolderBrowserDialog1.ShowDialog() = DialogResult.OK) Then
            TextBox1.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub RadioButton2_CheckedChanged(sender As Object, e As EventArgs) Handles radioOff.CheckedChanged
        Timer1.Stop()
        Label2.Enabled = False
        NumericUpDown1.Enabled = False
        btnStart.Enabled = False
        btnStop.Enabled = False
    End Sub

    Private Sub RadioButton1_CheckedChanged(sender As Object, e As EventArgs) Handles radioOn.CheckedChanged

        Label2.Enabled = True
        NumericUpDown1.Enabled = True
        btnStart.Enabled = True
    End Sub

    Private Sub btnStart_Click(sender As Object, e As EventArgs) Handles btnStart.Click
        Timer1.Interval = NumericUpDown1.Value * 1000
        Timer1.Start()
        btnCapture.PerformClick()
        btnStart.Enabled = False
        NumericUpDown1.Enabled = False
        btnStop.Enabled = True
        StopToolStripMenuItem.Visible = True
    End Sub

    Private Sub btnStop_Click(sender As Object, e As EventArgs) Handles btnStop.Click
        Timer1.Stop()
        btnStart.Enabled = True
        btnStop.Enabled = False
        NumericUpDown1.Enabled = True
        StopToolStripMenuItem.Visible = False
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        btnCapture.PerformClick()
    End Sub

    Private Sub btnHelp_Click(sender As Object, e As EventArgs) Handles btnHelp.Click
        Threading.Thread.Sleep(200)
        MessageBox.Show("1. Click Capture or press PrtScr to capture screenshots automatically saved in default location." & vbNewLine & "2. Click Open to see the folder storing screenshots." & vbNewLine & "3. Click Set location to set default folder." & vbNewLine & "4. Check Start with Windows to run on startup." & vbNewLine & "5. Choose On to run Auto Mode, set the number of second(s) (1~3600) after each shot, then click Start, and Stop whenever you want.", "How to use Automatic Screen Capture", MessageBoxButtons.OK, MessageBoxIcon.Information)
    End Sub

    Private Sub ShowToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ShowToolStripMenuItem.Click
        showevent()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        Close()
    End Sub

    Private Sub StopToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles StopToolStripMenuItem.Click
        Timer1.Stop()
        showevent()
    End Sub
End Class
