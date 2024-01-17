using ChatServer.Network.IO;
using System;
using System.Data.SqlTypes;
using System.Net;
using System.Net.Sockets;

namespace ChatServer
{
    class Program
    {
        private static List<Client> _users;
        private static TcpListener _listener;
        static void Main(string[] args)
        {
            _users = new List<Client>();
            _listener = new TcpListener(IPAddress.Parse("172.30.10.51"), 8800);
            _listener.Start();

            while(true)
            {
                var client = new Client(_listener.AcceptTcpClient());
                _users.Add(client);

                /* Broadcast the connection on the server */
                BroadcastConnection();
            }
        }

        static void BroadcastConnection()
        {
            foreach(var user in _users)
            {
                foreach(var usr in _users)
                {
                    var broadcastPacket = new PacketBuilder();
                    broadcastPacket.WriteOpCode(1);
                    broadcastPacket.WriteMessage(usr.Username);
                    broadcastPacket.WriteMessage(usr.Uid.ToString());
                    user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
                }
            }
        }
    }
}