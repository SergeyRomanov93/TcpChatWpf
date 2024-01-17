using ChatClient.Command;
using ChatClient.Model;
using ChatClient.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatClient.ViewModel
{
    internal class MainViewModel
    {
        private Server _server;

        public RelayCommand ConnectToServerCommand { get; set; }

        public string Username { get; set; }

        public MainViewModel()
        {
            _server = new Server();
            _server.connectedEvent += UserConnected;
            ConnectToServerCommand = new RelayCommand(o => _server.ConnectToServer(Username), o => !string.IsNullOrEmpty(Username));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                Uid = _server.PacketReader.ReadMessage()
            };

            //see video https://www.youtube.com/watch?v=I-Xmp-mulz4 36:10
        }
    }
}
