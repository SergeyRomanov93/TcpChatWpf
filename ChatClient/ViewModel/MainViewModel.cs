using ChatClient.Command;
using ChatClient.Model;
using ChatClient.Network;
using Prism.Mvvm;
using System.Collections.ObjectModel;
using System.Windows;

namespace ChatClient.ViewModel
{
    internal class MainViewModel : BindableBase
    {
        private Server _server;

        public RelayCommand ConnectToServerCommand { get; set; }
        public RelayCommand SendMessageCommand { get; set; }

        public string? Username { get; set; }
        public string? Message { get; set; }
        public string IpAddress { get; set; }

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public MainViewModel()
        {
            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.messageReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;

            ConnectToServerCommand = new RelayCommand(o => _server.Connect(Username, IpAddress), o => !string.IsNullOrEmpty(Username));
            SendMessageCommand = new RelayCommand(o => _server.SendMessageToServer(Message), o => !string.IsNullOrEmpty(Message));
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.Uid == uid).FirstOrDefault();
            Application.Current.Dispatcher.Invoke(() => Users.Remove(user));
        }

        private void MessageReceived()
        {
            var message = _server.PacketReader.ReadMessage();
            Application.Current.Dispatcher.Invoke(() => Messages.Add(message));
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                Uid = _server.PacketReader.ReadMessage()
            };

            if(!Users.Any(x => x.Uid == user.Uid))
            {
                Application.Current.Dispatcher.Invoke(() => Users.Add(user));
            }
        }
    }
}
