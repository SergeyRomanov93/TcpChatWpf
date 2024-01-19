using ChatServer.Network.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    internal class Client
    {
        public string Username { get; set; }
        public Guid Uid { get; set; }
        public TcpClient ClientSocket { get; set; }

        PacketReader _packetReader;

        public Client(TcpClient client)
        {
            ClientSocket = client;
            Uid = Guid.NewGuid();
            _packetReader = new PacketReader(ClientSocket.GetStream());

            var opcode = _packetReader.ReadByte();
            Username = _packetReader.ReadMessage();

            Console.WriteLine($"[{DateTime.Now}]: Client has connected with username: {Username}");

            Task.Run(() => Process());
        }

        void Process()
        {
            while (true)
            {
                try
                {
                    var opcode = _packetReader.ReadByte();

                    switch (opcode)
                    {
                        case 5:
                            var message = _packetReader.ReadMessage();
                            Console.WriteLine($"[{DateTime.Now}] : Message received! {message}");
                            Program.BroadcastMessage($"[{DateTime.Now}]: [{Username}]: {message}");
                            break;
                    }
                }
                catch(Exception)
                {
                    Console.WriteLine($"[{Uid.ToString()}]: Lost connection!");
                    Program.BroadcastDisconnect(Uid.ToString());
                    ClientSocket.Close();
                    break;
                }
            }
        }
    }
}
