Public Class frmClient

    Dim client As New SocketClient
    Dim cServerPort As Integer = 9696

    Private Sub frmClient_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        client.ConnectToServer()
        btnConnect.Text = "Disconnect"
    End Sub

    Private Sub btnConnect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnConnect.Click
        If btnConnect.Text = "Connect" Then
            client.ConnectToServer()
            btnConnect.Text = "Disconnect"
        Else
            client.DisconnectFromServer()
            btnConnect.Text = "Connect"
        End If

    End Sub

    Private Sub btnSend_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSend.Click
        sendMessage()
    End Sub

    Private Sub txtSend_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles txtSend.KeyDown
        If e.KeyCode = Keys.Enter Then
            sendMessage()
        End If
    End Sub

    Sub sendMessage()
        client.SendMessageToServer("/say " & txtSend.Text)
    End Sub

End Class