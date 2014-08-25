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

Benchmarks
----------------

The server can handle connections up to the OS port limit:

![64000clients](http://glassocean.net/media/64000-clients.jpg)

It can handle a high number of requests without failure:

![4000rps](http://glassocean.net/media/requests-per-second.jpg)

The client app has a stress test built in, but I mostly use ApacheBench, siege and WeigHTTP for comparing results with other servers now. Benchmarks and optimizations will be an ongoing process, so rather than spamming this readme with these results, they will be posted to my website, along with insight into the optimizations being performed.

Benchmarks:
* [.NET Sockets - Preliminary Benchmarks](http://glassocean.net/dotnetsockets-preliminary-benchmarks/)
* [Benchmarking Node.js, Apache and .NET Sockets](http://glassocean.net/benchmarking-node-js-apache-and-net-sockets/)

Roadmap / Future Challenges
---------------------------
* Implement the publish/subscribe pattern for supporting multiple/individual channels of communication.
* Implement a RESTful HTTP web server component.
* Experiment with connecting to the server via HTML5 WebSockets.
* Update the APM (Asynchronous Programming Model) + EAP (Event-based Asynchronous Pattern) to TAP (Task-based Asynchronous Pattern) which is available in .NET framework 4.0.
* Optimize the concurrency issue surrounding large numbers of concurrent processes such as starting up 1000 instances of php-cgi.exe. Turns out I've been using it incorrectly (as CGI instead of FastCGI).
* Do more benchmarks with real content and compare to other servers such as IIS, nginx, vert.x.
* Look into IIS Hostable Web Core.
* Flesh out the binary protocol and HTTP protocol into modular first-class citizens that can be used together or separately.

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
