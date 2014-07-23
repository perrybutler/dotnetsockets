<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmServer
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.btnStartServer = New System.Windows.Forms.Button
        Me.txtReceiveLog = New System.Windows.Forms.TextBox
        Me.ListBox1 = New System.Windows.Forms.ListBox
        Me.SuspendLayout()
        '
        'btnStartServer
        '
        Me.btnStartServer.Location = New System.Drawing.Point(4, 4)
        Me.btnStartServer.Name = "btnStartServer"
        Me.btnStartServer.Size = New System.Drawing.Size(75, 23)
        Me.btnStartServer.TabIndex = 0
        Me.btnStartServer.Text = "Start Server"
        Me.btnStartServer.UseVisualStyleBackColor = True
        '
        'txtReceiveLog
        '
        Me.txtReceiveLog.Location = New System.Drawing.Point(4, 32)
        Me.txtReceiveLog.Multiline = True
        Me.txtReceiveLog.Name = "txtReceiveLog"
        Me.txtReceiveLog.Size = New System.Drawing.Size(276, 228)
        Me.txtReceiveLog.TabIndex = 1
        '
        'ListBox1
        '
        Me.ListBox1.FormattingEnabled = True
        Me.ListBox1.Location = New System.Drawing.Point(288, 32)
        Me.ListBox1.Name = "ListBox1"
        Me.ListBox1.Size = New System.Drawing.Size(120, 225)
        Me.ListBox1.TabIndex = 2
        '
        'frmServer
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(413, 262)
        Me.Controls.Add(Me.ListBox1)
        Me.Controls.Add(Me.txtReceiveLog)
        Me.Controls.Add(Me.btnStartServer)
        Me.Name = "frmServer"
        Me.Text = "Socket Server"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents btnStartServer As System.Windows.Forms.Button
    Friend WithEvents txtReceiveLog As System.Windows.Forms.TextBox
    Friend WithEvents ListBox1 As System.Windows.Forms.ListBox

End Class
