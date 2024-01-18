using ChatClient.Network.IO;
using System.Net.Sockets;

namespace ChatClient.Network
{
    internal class Server
    {
        private TcpClient _tcpClient;

        public PacketReader? PacketReader;

        public event Action? connectedEvent;
        public event Action? messageReceivedEvent;
        public event Action? userDisconnectEvent;

        public Server()
        {
            _tcpClient = new TcpClient();
        }

        public void Connect(string username, string ipAddress)
        {
            if (!_tcpClient.Connected)
            {
                _tcpClient.Connect(ipAddress, 8800);
                PacketReader = new PacketReader(_tcpClient.GetStream());

                if(!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteMessage(username);
                    _tcpClient.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        public void SendMessageToServer(string message)
        {
            var messagePacket = new PacketBuilder();
            messagePacket.WriteOpCode(5);
            messagePacket.WriteMessage(message);
            _tcpClient.Client.Send(messagePacket.GetPacketBytes());
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while(true)
                {
                    var opcode = PacketReader?.ReadByte();

                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
                            break;
                        case 5:
                            messageReceivedEvent?.Invoke();
                            break;
                        case 10:
                            userDisconnectEvent?.Invoke();
                            break;
                        default:
                            Console.WriteLine("wOw");
                            break;
                    }
                }
            });
        }
    }
}
