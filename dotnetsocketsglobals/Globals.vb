Public Module Globals

    Public gBufferSize As Integer = 1024

    Public Class AsyncReceiveState
        Public Socket As System.Net.Sockets.Socket
        Public Buffer(gBufferSize - 1) As Byte
        Public PacketBufferStream As New IO.MemoryStream ' a buffer for appending received data to build the packet
        Public Packet As String
        Public ReceiveSize As Integer ' the size (in bytes) of the Packet
        Public TotalBytesReceived As Integer ' the total bytes received for the Packet so far
    End Class

    Public Class AsyncSendState
        Public Socket As System.Net.Sockets.Socket
        'Public Buffer(Carcassonne.Library.PacketBufferSize - 1) As Byte ' a buffer to store the currently received chunk of bytes
        Public BytesToSend() As Byte
        Public Progress As Integer
        Sub New(ByVal argSocket As System.Net.Sockets.Socket)
            Me.Socket = argSocket
        End Sub
        Function NextOffset() As Integer
            Return Progress
        End Function
        Function NextLength() As Integer
            If BytesToSend.Length - Progress > gBufferSize Then
                Return gBufferSize
            Else
                Return BytesToSend.Length - Progress
            End If
        End Function
    End Class

    Public Class MessageQueue
        Public Messages As New Queue
        Public Processing As Boolean
        Public Event MessageQueued()
        Public Sub Add(ByVal argState As AsyncSendState)
            Me.Messages.Enqueue(argState)
            RaiseEvent MessageQueued()
        End Sub
    End Class

End Module
