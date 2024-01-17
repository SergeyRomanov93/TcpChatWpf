using ChatClient.Network.IO;
using System.Net.Sockets;

namespace ChatClient.Network
{
    internal class Server
    {
        private TcpClient _tcpClient;

        public PacketReader PacketReader;

        public event Action connectedEvent;
        public Server()
        {
            _tcpClient = new TcpClient();
        }

        public void ConnectToServer(string username)
        {
            if(!_tcpClient.Connected)
            {
                _tcpClient.Connect("172.30.10.51", 8800);
                PacketReader = new PacketReader(_tcpClient.GetStream());

                if(!string.IsNullOrEmpty(username))
                {
                    var connectPacket = new PacketBuilder();
                    connectPacket.WriteOpCode(0);
                    connectPacket.WriteString(username);
                    _tcpClient.Client.Send(connectPacket.GetPacketBytes());
                }
                ReadPackets();
            }
        }

        private void ReadPackets()
        {
            Task.Run(() =>
            {
                while(true)
                {
                    var opcode = PacketReader.ReadByte();

                    switch (opcode)
                    {
                        case 1:
                            connectedEvent?.Invoke();
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
