<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmClient
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
        Me.txtSend = New System.Windows.Forms.TextBox
        Me.txtChat = New System.Windows.Forms.TextBox
        Me.btnConnect = New System.Windows.Forms.Button
        Me.btnSend = New System.Windows.Forms.Button
        Me.SuspendLayout()
        '
        'txtSend
        '
        Me.txtSend.Location = New System.Drawing.Point(84, 232)
        Me.txtSend.Name = "txtSend"
        Me.txtSend.Size = New System.Drawing.Size(196, 20)
        Me.txtSend.TabIndex = 0
        '
        'txtChat
        '
        Me.txtChat.Location = New System.Drawing.Point(4, 32)
        Me.txtChat.Multiline = True
        Me.txtChat.Name = "txtChat"
        Me.txtChat.Size = New System.Drawing.Size(276, 192)
        Me.txtChat.TabIndex = 1
        '
        'btnConnect
        '
        Me.btnConnect.Location = New System.Drawing.Point(4, 4)
        Me.btnConnect.Name = "btnConnect"
        Me.btnConnect.Size = New System.Drawing.Size(75, 23)
        Me.btnConnect.TabIndex = 2
        Me.btnConnect.Text = "Connect"
        Me.btnConnect.UseVisualStyleBackColor = True
        '
        'btnSend
        '
        Me.btnSend.Location = New System.Drawing.Point(4, 232)
        Me.btnSend.Name = "btnSend"
        Me.btnSend.Size = New System.Drawing.Size(75, 23)
        Me.btnSend.TabIndex = 3
        Me.btnSend.Text = "Send"
        Me.btnSend.UseVisualStyleBackColor = True
        '
        'frmClient
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Controls.Add(Me.btnSend)
        Me.Controls.Add(Me.btnConnect)
        Me.Controls.Add(Me.txtChat)
        Me.Controls.Add(Me.txtSend)
        Me.Name = "frmClient"
        Me.Text = "Socket Client"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents txtSend As System.Windows.Forms.TextBox
    Friend WithEvents txtChat As System.Windows.Forms.TextBox
    Friend WithEvents btnConnect As System.Windows.Forms.Button
    Friend WithEvents btnSend As System.Windows.Forms.Button
End Class
