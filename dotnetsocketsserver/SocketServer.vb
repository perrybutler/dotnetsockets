Public Class SocketServer

    Dim cServerPort As Integer = 9898
    Dim cServerAddress As String = "localhost"

    Dim cServerSocket As Net.Sockets.Socket
    Dim cStopRequested As Boolean
    Dim cClients As New ArrayList

    Event MessageReceived(ByVal argMessage As String, ByVal argClientSocket As Net.Sockets.Socket)
    Event ClientConnected(ByVal argClientSocket As Net.Sockets.Socket)
    Event ClientDisconnected(ByVal argClientSocket As Net.Sockets.Socket)

    Sub InitializeServer()

    End Sub

    ''' <summary>
    ''' StartServer starts the server by listening for new client connections with a TcpListener.
    ''' </summary>
    ''' <remarks></remarks>
    Sub StartServer()

        ' create the TcpListener which will listen for and accept new client connections asynchronously
        cServerSocket = New System.Net.Sockets.Socket(Net.Sockets.AddressFamily.InterNetwork, Net.Sockets.SocketType.Stream, Net.Sockets.ProtocolType.Tcp)

        ' convert the server address and port into an ipendpoint
        Dim mHostAddresses() As Net.IPAddress = Net.Dns.GetHostAddresses(cServerAddress)
        Dim mEndPoint As Net.IPEndPoint = Nothing
        For Each mHostAddress In mHostAddresses
            If mHostAddress.AddressFamily = Net.Sockets.AddressFamily.InterNetwork Then
                mEndPoint = New Net.IPEndPoint(mHostAddress, cServerPort)
            End If
        Next

        ' bind to the server's ipendpoint
        cServerSocket.Bind(mEndPoint)

        ' configure the listener to allow 1 incoming connection at a time
        cServerSocket.Listen(1)

        ' accept client connection async
        cServerSocket.BeginAccept(New AsyncCallback(AddressOf ClientAccepted), cServerSocket)

    End Sub

    Sub StopServer()
        'cServerSocket.Disconnect(True)
        cServerSocket.Close()
        'cStopRequested = True
    End Sub

    ''' <summary>
    ''' ClientConnected is a callback that gets called when the server accepts a client connection from the async BeginAccept method.
    ''' </summary>
    ''' <param name="ar"></param>
    ''' <remarks></remarks>
    Sub ClientAccepted(ByVal ar As IAsyncResult)
        ' get the async state object from the async BeginAccept method, which contains the server's listening socket
        Dim mServerSocket As System.Net.Sockets.Socket = ar.AsyncState
        ' call EndAccept which will connect the client and give us the the client socket
        Dim mClientSocket As System.Net.Sockets.Socket = Nothing
        Try
            mClientSocket = mServerSocket.EndAccept(ar)
        Catch ex As ObjectDisposedException
            ' if we get an ObjectDisposedException it that means the server socket terminated while this async method was still active
            Exit Sub
        End Try
        ' instruct the client to begin receiving data
        Dim mState As New AsyncReceiveState
        mState.Socket = mClientSocket
        RaiseEvent ClientConnected(mState.Socket)
        mState.Socket.BeginReceive(mState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ClientMessageReceived), mState)
        ' begin accepting another client connection
        mServerSocket.BeginAccept(New AsyncCallback(AddressOf ClientAccepted), mServerSocket)
    End Sub

    ''' <summary>
    ''' BeginReceiveCallback is an async callback method that gets called when the server receives some data from a client socket after calling the async BeginReceive method.
    ''' </summary>
    ''' <param name="ar"></param>
    ''' <remarks></remarks>
    Sub ClientMessageReceived(ByVal ar As IAsyncResult)
        ' get the async state object from the async BeginReceive method
        Dim mState As AsyncReceiveState = ar.AsyncState
        ' call EndReceive which will give us the number of bytes received
        Dim numBytesReceived As Integer
        Try
            numBytesReceived = mState.Socket.EndReceive(ar)
        Catch ex As Net.Sockets.SocketException
            ' if we get a ConnectionReset exception, it could indicate that the client has disconnected
            If ex.SocketErrorCode = Net.Sockets.SocketError.ConnectionReset Then
                RaiseEvent ClientDisconnected(mState.Socket)
                Exit Sub
            End If
        End Try
        ' if we get numBytesReceived equal to zero, it could indicate that the client has disconnected
        If numBytesReceived = 0 Then
            RaiseEvent ClientDisconnected(mState.Socket)
            Exit Sub
        End If
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
            mState.Socket.BeginReceive(mState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ClientMessageReceived), mState)
        Else
            ' ## FINAL DATA RECEIVED, PARSE AND PROCESS THE PACKET ##
            ' the TotalBytesReceived is equal to the ReceiveSize, so we are done receiving this Packet...parse it!
            Dim mSerializer As New System.Runtime.Serialization.Formatters.Binary.BinaryFormatter
            ' rewind the PacketBufferStream so we can de-serialize it
            mState.PacketBufferStream.Position = 0
            ' de-serialize the PacketBufferStream which will give us an actual Packet object
            mState.Packet = mSerializer.Deserialize(mState.PacketBufferStream)
            ' handle the message
            ParseReceivedClientMessage(mState.Packet, mState.Socket)
            ' call BeginReceive again, so we can start receiving another packet from this client socket
            Dim mNextState As New AsyncReceiveState
            mNextState.Socket = mState.Socket
            mNextState.Socket.BeginReceive(mNextState.Buffer, 0, gBufferSize, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf ClientMessageReceived), mNextState)
        End If
    End Sub

    Sub ParseReceivedClientMessage(ByVal argCommandString As String, ByVal argClient As Net.Sockets.Socket)

        Console.WriteLine("ParseReceivedClientMessage: " & argCommandString)

        ' parse the command string
        Dim argCommand As String
        Dim argText As String
        argCommand = argCommandString.Substring(0, argCommandString.IndexOf(" "))
        argText = argCommandString.Remove(0, argCommand.Length + 1)

        Select Case argText
            Case "hi server"
                SendMessageToClient("/say Server replied.", argClient)
        End Select

        RaiseEvent MessageReceived(argCommandString, argClient)

        '' respond back to the client on certain messages
        'Select Case argMessageString
        '    Case "hi"
        '        SendMessageToClient("\say", "hi received", argClient)
        'End Select
        'RaiseEvent MessageReceived(argCommandString & " | " & argMessageString)
    End Sub

    ''' <summary>
    ''' QueueMessage prepares a Message object containing our data to send and queues this Message object in the OutboundMessageQueue.
    ''' </summary>
    ''' <remarks></remarks>
    Sub SendMessageToClient(ByVal argCommandString As String, ByVal argClient As Net.Sockets.Socket)

        ' parse the command string
        Dim argCommand As String
        Dim argText As String
        argCommand = argCommandString.Substring(0, argCommandString.IndexOf(" "))
        argText = argCommandString.Remove(0, argCommand.Length)

        '' create a Packet object from the passed data
        'Dim mPacket As New Dictionary(Of String, String)
        'mPacket.Add("CMD", argCommandMessage)
        'mPacket.Add("MSG", argCommandData)

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
        Dim mState As New AsyncSendState(argClient)

        ' resize the BytesToSend array to fit both the mSizeBytes and the mPacketBytes
        ReDim mState.BytesToSend(mPacketBytes.Length + mSizeBytes.Length - 1)

        ' copy the mSizeBytes and mPacketBytes to the BytesToSend array
        System.Buffer.BlockCopy(mSizeBytes, 0, mState.BytesToSend, 0, mSizeBytes.Length)
        System.Buffer.BlockCopy(mPacketBytes, 0, mState.BytesToSend, mSizeBytes.Length, mPacketBytes.Length)

        ' queue the Message
        argClient.BeginSend(mState.BytesToSend, mState.NextOffset, mState.NextLength, Net.Sockets.SocketFlags.None, New AsyncCallback(AddressOf MessagePartSent), mState)

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

        Catch ex As Exception

            MessageBox.Show("DataSent error: " & ex.Message)

        End Try

    End Sub

End Class
