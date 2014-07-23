Public Class SocketClient

    Dim cServerPort As Integer = 9898
    Dim cServerAddress As String = "localhost"

    Dim cClientSocket As Net.Sockets.Socket
    Dim WithEvents cSendQueue As New MessageQueue

    Event MessageSentToServer(ByVal argCommandString As String)

    Sub ConnectToServer()
        ' create the TcpListener which will listen for and accept new client connections asynchronously
        cClientSocket = New System.Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)
        ' convert the server address and port into an ipendpoint
        Dim mHostAddresses() As Net.IPAddress = Net.Dns.GetHostAddresses(cServerAddress)
        Dim mEndPoint As Net.IPEndPoint = Nothing
        For Each mHostAddress In mHostAddresses
            If mHostAddress.AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                mEndPoint = New Net.IPEndPoint(mHostAddress, cServerPort)
            End If
        Next
        ' connect to server async
        Try
            cClientSocket.BeginConnect(mEndPoint, New AsyncCallback(AddressOf ConnectToServerCompleted), New AsyncSendState(cClientSocket))
        Catch ex As Exception
            MsgBox("ConnectToServer error: " & ex.Message)
        End Try
    End Sub

    Sub DisconnectFromServer()
        cClientSocket.Disconnect(False)
    End Sub

    ''' <summary>
    ''' Fires right when a client is connected to the server.
    ''' </summary>
    ''' <param name="ar"></param>
    ''' <remarks></remarks>
    Sub ConnectToServerCompleted(ByVal ar As IAsyncResult)
        ' get the async state object which was returned by the async beginconnect method
        Dim mState As AsyncSendState = ar.AsyncState
        ' end the async connection request so we can check if we are connected to the server
        Try
            ' call the EndConnect method which will succeed or throw an error depending on the result of the connection
            mState.Socket.EndConnect(ar)
            ' at this point, the EndConnect succeeded and we are connected to the server!
            ' send a welcome message
            SendMessageToServer("/say What? My name is...")
            ' start waiting for messages from the server
            Dim mReceiveState As New AsyncReceiveState
            mReceiveState.Socket = mState.Socket
            mReceiveState.Socket.BeginReceive(mReceiveState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ServerMessageReceived), mReceiveState)
        Catch ex As Exception
            ' at this point, the EndConnect failed and we are NOT connected to the server!
            MessageBox.Show("Connect error: " & ex.Message)
        End Try
    End Sub

    Sub ServerMessageReceived(ByVal ar As IAsyncResult)
        ' get the async state object from the async BeginReceive method
        Dim mState As AsyncReceiveState = ar.AsyncState
        ' call EndReceive which will give us the number of bytes received
        Dim numBytesReceived As Integer
        numBytesReceived = mState.Socket.EndReceive(ar)
        ' determine if this is the first data received
        If mState.ReceiveSize = 0 Then
            ' this is the first data recieved, so parse the receive size which is encoded in the first four bytes of the buffer
            mState.ReceiveSize = BitConverter.ToInt32(mState.Buffer, 0)
            ' write the received bytes thus far to the packet data stream
            mState.PacketBufferStream.Write(mState.Buffer, 4, numBytesReceived - 4)
        Else
            ' write the received bytes thus far to the packet data stream
            mState.PacketBufferStream.Write(mState.Buffer, 0, numBytesReceived)
        End If
        ' increment the total bytes received so far on the state object
        mState.TotalBytesReceived += numBytesReceived
        ' check for the end of the packet
        If mState.TotalBytesReceived < mState.ReceiveSize Then ' bytesReceived = Carcassonne.Library.PacketBufferSize Then
            ' ## STILL MORE DATA FOR THIS PACKET, CONTINUE RECEIVING ##
            ' the TotalBytesReceived is less than the ReceiveSize so we need to continue receiving more data for this packet
            mState.Socket.BeginReceive(mState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ServerMessageReceived), mState)
        Else
            ' ## FINAL DATA RECEIVED, PARSE AND PROCESS THE PACKET ##
            ' the TotalBytesReceived is equal to the ReceiveSize, so we are done receiving this Packet...parse it!
            Dim mSerializer As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            ' rewind the PacketBufferStream so we can de-serialize it
            mState.PacketBufferStream.Position = 0
            ' de-serialize the PacketBufferStream which will give us an actual Packet object
            mState.Packet = mSerializer.Deserialize(mState.PacketBufferStream)
            ' parse the complete message that was received from the server
            ParseReceivedServerMessage(mState.Packet, mState.Socket)
            ' call BeginReceive again, so we can start receiving another packet from this client socket
            Dim mNextState As New AsyncReceiveState
            mNextState.Socket = mState.Socket
            mNextState.Socket.BeginReceive(mNextState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ServerMessageReceived), mNextState)
        End If
    End Sub

    Sub ParseReceivedServerMessage(ByVal argCommandString As String, ByVal argClient As Net.Sockets.Socket)
        Console.WriteLine(argCommandString)
        'Select Case argDat
        '    Case "hi"
        '        Send("hi", argClient)
        'End Select
        'RaiseEvent MessageReceived(argMsg & " | " & argDat)
    End Sub

    Sub SendMessageToServer(ByVal argCommandString As String)

        ' create a Packet object from the passed data; this packet can be any object type because we use serialization!
        'Dim mPacket As New Dictionary(Of String, String)
        'mPacket.Add("CMD", argCommandString)
        'mPacket.Add("MSG", argMessageString)
        Dim mPacket As String = argCommandString

        ' serialize the Packet into a stream of bytes which is suitable for sending with the Socket
        Dim mSerializer As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
        Dim mSerializerStream As New System.IO.MemoryStream
        mSerializer.Serialize(mSerializerStream, mPacket)

        ' get the serialized Packet bytes
        Dim mPacketBytes() As Byte = mSerializerStream.GetBuffer

        ' convert the size into a byte array
        Dim mSizeBytes() As Byte = BitConverter.GetBytes(mPacketBytes.Length + 4)

        ' create the async state object which we can pass between async methods
        Dim mState As New AsyncSendState(cClientSocket)

        ' resize the BytesToSend array to fit both the mSizeBytes and the mPacketBytes
        ReDim mState.BytesToSend(mPacketBytes.Length + mSizeBytes.Length - 1)

        ' copy the mSizeBytes and mPacketBytes to the BytesToSend array
        System.Buffer.BlockCopy(mSizeBytes, 0, mState.BytesToSend, 0, mSizeBytes.Length)
        System.Buffer.BlockCopy(mPacketBytes, 0, mState.BytesToSend, mSizeBytes.Length, mPacketBytes.Length)

        cClientSocket.BeginSend(mState.BytesToSend, mState.NextOffset, mState.NextLength, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf MessagePartSent), mState)

    End Sub

    '''' <summary>
    '''' QueueMessage prepares a Message object containing our data to send and queues this Message object in the OutboundMessageQueue.
    '''' </summary>
    '''' <param name="argCommandMessage"></param>
    '''' <param name="argCommandData"></param>
    '''' <remarks></remarks>
    'Sub QueueMessage(ByVal argCommandMessage As String, ByVal argCommandData As Object)

    'End Sub

    Private Sub cSendQueue_MessageQueued() Handles cSendQueue.MessageQueued
        ' when a message is queued, we need to check whether or not we are currently processing the queue before allowing the top item in the queue to start sending
        If cSendQueue.Processing = False Then
            ' process the top message in the queue, which in turn will process all other messages until the queue is empty
            Dim mState As AsyncSendState = cSendQueue.Messages.Dequeue
            ' we must send the correct number of bytes, which must not be greater than the remaining bytes
            cClientSocket.BeginSend(mState.BytesToSend, mState.NextOffset, mState.NextLength, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf MessagePartSent), mState)
        End If
    End Sub

    Sub MessagePartSent(ByVal ar As IAsyncResult)
        ' get the async state object which was returned by the async beginsend method
        Dim mState As AsyncSendState = ar.AsyncState
        Try
            Dim numBytesSent As Integer
            ' call the EndSend method which will succeed or throw an error depending on if we are still connected
            numBytesSent = mState.Socket.EndSend(ar)
            ' increment the total amount of bytes processed so far
            mState.Progress += numBytesSent
            ' determine if we havent' sent all the data for this Packet yet
            If mState.NextLength > 0 Then
                ' we need to send more data
                mState.Socket.BeginSend(mState.BytesToSend, mState.NextOffset, mState.NextLength, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf MessagePartSent), mState)
            End If
            ' at this point, the EndSend succeeded and we are ready to send something else!
            ' TODO: use the queue to determine what message was sent and show it in the local chat buffer
            'RaiseEvent MessageSentToServer()
        Catch ex As Exception
            MessageBox.Show("DataSent error: " & ex.Message)
        End Try
    End Sub

End Class
