using System.Text;

namespace ChatBl.Network.IO
{
    public class PacketBuilder
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

        public void WriteMessage(string message)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var messageLength = message.Length;
            _memoryStream.Write(BitConverter.GetBytes(messageLength));
            _memoryStream.Write(Encoding.GetEncoding("windows-1251").GetBytes(message));
        }

        public byte[] GetPacketBytes()
        {
            return _memoryStream.ToArray();
        }
    }
}
