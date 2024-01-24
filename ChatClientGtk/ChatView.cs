using Gtk;
using System;
using UI = Gtk.Builder.ObjectAttribute;

namespace ChatClientGtk
{
    internal class ChatView : Window
    {
        [UI] private Label _label1 = null;
        [UI] private Button _button1 = null;

        public ChatView() : this(new Builder("ChatView.glade")) { }

        private ChatView(Builder builder) : base(builder.GetRawOwnedObject("ChatView"))
        {
            builder.Autoconnect(this);
        }
    }
}
