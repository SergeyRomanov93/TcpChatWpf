using ChatBl.Model;
using ChatBl.Network;
using Gtk;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using UI = Gtk.Builder.ObjectAttribute;

namespace ChatClientGladeGtk
{
    internal class MainWindow : Window
    {
        private Server _server;

        [UI] private Button _connectButton = null;
        [UI] private Button _sendMessageButton = null;
        [UI] private Entry _ipAddress = null;
        [UI] private Entry _nickname = null;
        [UI] private Entry _message = null;
        [UI] private Label _userBox = null;
        [UI] private Label _messageBox = null;

        public ObservableCollection<UserModel> Users { get; set; }
        public ObservableCollection<string> Messages { get; set; }

        public MainWindow() : this(new Builder("MainWindow.glade")) { }

        private MainWindow(Builder builder) : base(builder.GetRawOwnedObject("MainWindow"))
        {
            builder.Autoconnect(this);

            DeleteEvent += Window_DeleteEvent;

            Users = new ObservableCollection<UserModel>();
            Messages = new ObservableCollection<string>();

            _server = new Server();
            _server.connectedEvent += UserConnected;
            _server.messageReceivedEvent += MessageReceived;
            _server.userDisconnectEvent += RemoveUser;

            _connectButton.Clicked += Connect;
            _sendMessageButton.Clicked += Send;
        }

        private void RemoveUser()
        {
            var uid = _server.PacketReader.ReadMessage();
            var user = Users.Where(x => x.Uid == uid).FirstOrDefault();
            Users.Remove(user);
        }

        private void MessageReceived()
        {
            var message = _server.PacketReader.ReadMessage();
            Messages.Add(message);
            _messageBox.Text += $"\r{message}";
        }

        private void UserConnected()
        {
            var user = new UserModel
            {
                Username = _server.PacketReader.ReadMessage(),
                Uid = _server.PacketReader.ReadMessage()
            };

            if (!Users.Any(x => x.Uid == user.Uid))
            {
                Users.Add(user);
                _messageBox.Text += $"\r{user.Username} connected";
                _userBox.Text += $"\r{user.Username}";
            }
        }

        private void Send(object sender, EventArgs e)
        {
            var message = _message.Text;
            _server.SendMessageToServer(message);
        }

        private void Connect(object sender, EventArgs e)
        {
            var nickname = _nickname.Text;
            var ipAddress = _ipAddress.Text;
            _server.Connect(nickname, ipAddress);
        }

        private void Window_DeleteEvent(object obj, DeleteEventArgs args)
        {
            Application.Quit();
        }
    }
}
