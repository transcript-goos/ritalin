using System;

using jabber;
using jabber.client;
using jabber.protocol.client;

namespace AuctionSniper.Core {
    public interface IChat {
        void SendMessage(Message inMessage);
        void Close();
        
        JabberClient Connection {get;}
        JID FromId {get;}
        JID ToJId {get;}
        
        AuctionMessageTranslator Translator {get; set;}
        
        event Action<JabberClient> ChatClosing;
    }

    public class Chat : IChat {
        public Chat(JID inToJId, JabberClient inConnection) {
            this.Connection = inConnection;
            this.Connection.OnMessage += (s, m) => {
                if (this.Translator != null) {
                    this.Translator.ProcessMessage(this, m);
                }
            };

            this.Connection.OnError += (s, ex) => {
                if (this.Translator != null) {
                }
            };

            this.ToJId = inToJId;
            this.FromId = new JID(
                this.Connection.User, this.Connection.NetworkHost, this.Connection.Resource
            );

        }
        
        void IChat.SendMessage(Message inMessage) {
            inMessage.Type = MessageType.chat;
            inMessage.To = this.ToJId;
            
            this.Connection.Write(inMessage);
        }
        
        void IChat.Close() {
            this.Connection.Close();
            
            if (this.ChatClosing != null) {
                this.ChatClosing(this.Connection);
            }
        }
        
        public JabberClient Connection {get; private set;}
        public JID ToJId {get; private set;}
        public JID FromId {get; private set;}

        public AuctionMessageTranslator Translator {get; set;}
        
        public event Action<JabberClient> ChatClosing;
    }
}

