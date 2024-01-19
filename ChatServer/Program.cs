using ChatServer.Network.IO;
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
            Console.Write("Введите адрес сервера: ");
            var ipAddress = Console.ReadLine();

            try
            {
                _users = new List<Client>();
                _listener = new TcpListener(IPAddress.Parse(ipAddress), 8800);
                _listener.Start();
                Console.WriteLine("Сервер активен");
            }
            catch
            {
                Console.WriteLine("Ошибка подключения!");
                return;
            }

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

        public static void BroadcastMessage(string message)
        {
            foreach (var user in _users)
            {
                var messagePacket = new PacketBuilder();
                messagePacket.WriteOpCode(5);
                messagePacket.WriteMessage(message);
                user.ClientSocket.Client.Send(messagePacket.GetPacketBytes());
            }
        }

        public static void BroadcastDisconnect(string uid)
        {
            var disconnectedUser = _users.Where(x => x.Uid.ToString() == uid).FirstOrDefault();
            var disconnectedMessage = $"[{disconnectedUser.Username}] disconnected!";

            _users.Remove(disconnectedUser);

            foreach (var user in _users)
            {
                var broadcastPacket = new PacketBuilder();
                broadcastPacket.WriteOpCode(10);
                broadcastPacket.WriteMessage(uid);
                user.ClientSocket.Client.Send(broadcastPacket.GetPacketBytes());
            }

            BroadcastMessage(disconnectedMessage);
        }
    }
}