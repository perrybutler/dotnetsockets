.NET Sockets
============

Client/server chat/packet engine using asynchronous .NET sockets.

This was developed as a sub-component for a personal project (Carcassonne board game in .NET) which I have since decided would be more suited to a website platform.

Using OOP principles, the idea is to offer a base packet object which can be inherited, allowing for custom packets that encapsulate any data structure using serialization, sending those packets to the server, and having the server respond by doing something or sending a message/command back to one or more clients. This allows the server to be the final authoritative voice, rather than clients communicating directly which would allow for a greater possibility of cheating. This type of networking architecture is used in Valve's [Source Engine](https://developer.valvesoftware.com/wiki/Source_Multiplayer_Networking) (Half-Life 2).

May be useful as a learning tool or starting point for multiplayer video game programming in Windows.

![dotnetsockets](http://files.glassocean.net/github/dotnetsockets.png)

In the screenshot above, multiple clients connect to and relay messages to the server, which may then relay other messages back to the client depending on the message recieved. I do realize the above screenshot shows a broken chat system because the clients cannot see eachother's messages, so I'll update this soon to show how custom packets can be designed and handled by the server/clients.
