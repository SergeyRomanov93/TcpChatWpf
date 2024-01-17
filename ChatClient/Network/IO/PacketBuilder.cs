using System.IO;
using System.Text;

namespace ChatClient.Network.IO
{
    internal class PacketBuilder
    {
        private MemoryStream _memoryStream;
        public PacketBuilder() 
        {
            _memoryStream = new MemoryStream();
        }

        public void WriteOpCode(byte opCode)
        {
            _memoryStream.WriteByte(opCode);
        }

        public void WriteString(string message)
        {
            var messageLength = message.Length;
            _memoryStream.Write(BitConverter.GetBytes(messageLength));
            _memoryStream.Write(Encoding.ASCII.GetBytes(message));
        }

        public byte[] GetPacketBytes()
        {
            return _memoryStream.ToArray();
        }
    }
}
