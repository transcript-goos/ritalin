using System;
using System.Collections.Generic;
using System.Linq;

using jabber;
using jabber.client;
using jabber.protocol.client;

namespace AuctionSniper.Core {
    public interface IAuctionEventListener {
        void AuctionClosed();
        void CurrentPrice(int inPrice, int inIncrement);
    }
    
    public interface IMessageListener {
        void ProcessMessage(Chat inChat, Message inMessage);
    }
    
    public class AuctionMessageTranslator : IMessageListener {
        public AuctionMessageTranslator(IAuctionEventListener inListener) {
            this.Listenr = inListener;
        }

        void IMessageListener.ProcessMessage(Chat inChar, Message inMessage) {
            var ev = this.UppackEventFrom(inMessage);

            switch (ev["Event"]) {
            case "CLOSE":
                this.Listenr.AuctionClosed();
                break;
            case "PRICE":
                // エラーチェックあまくね？
                this.Listenr.CurrentPrice(
                    int.Parse(ev["CurrentPrice"]),
                    int.Parse(ev["Increment"])
                );
                break;
            }
        }

        public IAuctionEventListener Listenr {get; private set;}

        private IDictionary<string, string> UppackEventFrom(Message inMessage) {
            return inMessage.Body
                .Split(new char[] {';'})
                .Select(elem => elem.Split(new char[] {':'}).Select(s => s.Trim()).ToList())
                .Where(elem => elem.Count > 1)
                .ToDictionary(elem => elem[0], elem => elem[1])
            ;
        }
    }

    public class Chat {
        public Chat(JID inToJId, JabberClient inConnection) {
            this.Connection = inConnection;
            this.Connection.OnMessage += (s, m) => {
                if (this.Translator != null) {
                    this.Translator.ProcessMessage(this, m);
                }
            };

            this.ToJId = inToJId;
        }

        public void SendMessage(Message inMessage) {
            inMessage.Type = MessageType.chat;
            inMessage.To = this.ToJId;

            this.Connection.Write(inMessage);
        }

        public JabberClient Connection {get; private set;}
        public JID ToJId {get; private set;}

        public IMessageListener Translator {get; set;}
    }
}

