.NET Sockets
============

Client/server communication system utilizing asynchronous .NET sockets via an Event-based Asynchronous Pattern (EAP) on top of an IAsyncResult Pattern with thread pools (I/O completion ports) and a binary packet-based TCP communications for maximum concurrency, capacity, performance and scalability.

Built in the heyday of hobby game programming, perhaps before Protobuf (Google) and Thrift (Facebook) were a thing, this project was aiming to achieve something similar - as a universal networking framework for game development.

![dotnetsockets](http://files.glassocean.net/github/dotnetsockets.png)

In the screenshot above, multiple clients connect to and relay messages to the server, which may then relay other messages back to the client depending on the message recieved. I do realize the above screenshot shows a broken chat system because the clients cannot see eachother's messages, so I'll update this soon to show how custom packets can be designed and handled by the server/clients.

How it works
------------

The engine buffers large messages and data transfers by breaking them down into smaller fixed-size packets and reassembling them on the receiving end. Utilizes high performance .NET Framework natives such as TCP Sockets, Asynchronous BeginReceive/EndReceive methods, Message Queue, BinaryFormatter Serialization, Byte-level BlockCopy, etc. This is quite basically an asynchronous version of .NET's pre-included TCP Client which only functions in synchronous blocking mode.

One way around traditional blocking sockets is to spawn each socket as a separate process (thread), but this can have limitations on the number of client connections due to the high number of threads the operating system has to switch between unless using the Overlapped I/O technique, which can still be a performance hindrance when compared to an asynchronous sockets model that takes advantage of thread pools (I/O completion ports).  

"Imagine using a blocking socket on a multiplayer game. Each client would be completely stopped, frozen in place, until some data was received." -Game Coding Complete

![dotnetsockets2](http://files.glassocean.net/github/dotnetsockets2.jpg)

Using OOP principles, the idea is to offer a base packet object which can be inherited, allowing for custom packets that encapsulate any data structure using serialization, sending those packets to the server, and having the server respond by doing something or sending a message/command back to one or more clients. This allows the server to be the final authoritative voice, rather than clients communicating directly which would allow for a greater possibility of cheating. This type of networking architecture is used in Valve's [Source Engine](https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking) (Half-Life 2).

May be useful as a learning tool or starting point for multiplayer video game programming in Windows.

Simple usage scenario
---------------------

The code snippet below demonstrates a very simple full lifecycle of the application:
```VB.net
' create a server and start it:
Dim WithEvents server As New SocketServer
server.StartServer()

' create a client and connect it to the server:
Dim client As New SocketClient("8989", "127.0.0.1")
client.ConnectToServer()

' send a message from the client to the server:
client.SendMessageToServer("/say Hi server!")

' at this point an event is raised at the server so our
' custom logic checks the message then does something...
(...)
Case "/say"
    SendMessageToClient("/say Hi client!", current_client)
(...)

' disconnect the client from the server
client.DisconnectFromServer()

' stop the server
server.StopServer()
```

Test simulations
----------------
The following DOS Batch script can be used to launch 10 clients and connect them to the server automatically:
```Batchfile
for /l %%i in (0,1,10) do (start AsyncSocketClient.exe -port 8989 -address 127.0.0.1)
```

Here are 100 clients connected to the server, all running on the localhost:

![dotnetsockets2](http://files.glassocean.net/github/dotnetsockets3.png)

Upon further review, it appears the SendMessageToClient function, or the callback in the UI thread, isn't optimized properly. When this portion of the connection logic is ommitted, we can clearly see that the server is able to handle over 10,000 connections without a hiccup:

![dotnetsockets3](http://files.glassocean.net/software development/dotnetsockets/over-9000.jpg)

This heavy load experiment has piqued my interest, so I'll be looking into the SendMessageToClient function. Stay tuned...

Roadmap / Future Challenges
---------------------------
* Implement the publish/subscribe pattern for supporting multiple/individual channels of communication.
* Implement a RESTful HTTP web server component.
* Experiment with connecting to the server via HTML5 WebSockets.

References
----------
[1] [Multithreaded Programming with the Event-based Asynchronous Pattern (MSDN)](http://msdn.microsoft.com/en-us/library/hkasytyf.aspx)  
[2] [Deciding When to Implement the Event-based Asynchronous Pattern (MSDN)](http://msdn.microsoft.com/en-us/library/ms228966.aspx)  
[3] [Simplify Asynchronous Programming with Tasks (MSDN Magazine)](http://msdn.microsoft.com/en-us/magazine/ff959203.aspx)  
[4] [IAsyncResult Interface (MSDN)](http://msdn.microsoft.com/en-us/library/system.iasyncresult.aspx)  
[5] [Asynchronous method invocation (Wikipedia)](http://en.wikipedia.org/wiki/Asynchronous_method_invocation)  
[6] [Get Closer to the Wire with High-Performance Sockets in .NET](http://msdn.microsoft.com/en-us/magazine/cc300760.aspx)

History
-------

This was developed as a sub-component for a personal project (Carcassonne board game in .NET) which I have since decided would be more suited to a website platform using websockets and a similar architecture.
