Public Class frmServer

    Dim WithEvents server As New SocketServer
    Delegate Sub MessageReceivedDelegate(ByVal argMessage As String, ByVal argClientSocket As Net.Sockets.Socket)
    Delegate Sub ClientConnectedDelegate(ByVal argClientSocket As Net.Sockets.Socket)
    Delegate Sub ClientDisconnectedDelegate(ByVal argClientSocket As Net.Sockets.Socket)

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        'server.InitializeServer()
        server.StartServer()
        btnStartServer.Text = "Stop Server"
    End Sub

    Private Sub btnStartServer_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnStartServer.Click
        If btnStartServer.Text = "Start Server" Then
            server.StartServer()
            btnStartServer.Text = "Stop Server"
        Else
            server.StopServer()
            btnStartServer.Text = "Start Server"
        End If
    End Sub

    Private Sub server_ClientConnected(ByVal argClientSocket As System.Net.Sockets.Socket) Handles server.ClientConnected
        Invoke(New ClientConnectedDelegate(AddressOf ClientConnected), argClientSocket)
    End Sub

    Sub ClientConnected(ByVal argClientSocket As Net.Sockets.Socket)
        ListBox1.Items.Add(argClientSocket.RemoteEndPoint)
    End Sub

    Private Sub server_ClientDisconnected(ByVal argClientSocket As System.Net.Sockets.Socket) Handles server.ClientDisconnected
        Invoke(New ClientDisconnectedDelegate(AddressOf ClientDisconnected), argClientSocket)
    End Sub

    Sub ClientDisconnected(ByVal argClientSocket As Net.Sockets.Socket)
        ListBox1.Items.Remove(argClientSocket.RemoteEndPoint)
    End Sub

    Private Sub server_MessageReceived(ByVal argMessage As String, ByVal argClient As Net.Sockets.Socket) Handles server.MessageReceived
        Invoke(New MessageReceivedDelegate(AddressOf MessageReceived), New Object() {argMessage, argClient})
    End Sub

    Sub MessageReceived(ByVal argMessage As String, ByVal argClient As Net.Sockets.Socket)
        If txtReceiveLog.Text <> "" Then txtReceiveLog.Text = vbNewLine & txtReceiveLog.Text
        txtReceiveLog.Text = argClient.RemoteEndPoint.ToString & ": " & argMessage & txtReceiveLog.Text
    End Sub

End Class
