﻿using System.Net.Sockets;
using System.Text;

namespace ChatBl.Network.IO
{
    public class PacketReader : BinaryReader
    {
        private NetworkStream _networkStream;
        public PacketReader(NetworkStream networkStream) : base(networkStream)
        {
            _networkStream = networkStream;
        }

        public string ReadMessage()
        {
            byte[] messageBuffer;
            var length = ReadInt32();
            messageBuffer = new byte[length];
            _networkStream.Read(messageBuffer, 0, length);
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            var message = Encoding.GetEncoding("windows-1251").GetString(messageBuffer);
            return message;
        }
    }
}
