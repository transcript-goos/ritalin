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
        AuctionEventParser mParser = new AuctionEventParser();

        public AuctionMessageTranslator(IAuctionEventListener inListener) {
            mParser.RegisterAction("CLOSE", (ev) => {
                inListener.AuctionClosed();
            });
            mParser.RegisterAction("PRICE", (ev) => {
                inListener.CurrentPrice(
                    int.Parse(ev["CurrentPrice"]),
                    int.Parse(ev["Increment"])
                );
            });
        }

        public void ProcessMessage(Chat inChar, Message inMessage) {
            mParser.RunActionFrom(inMessage);
        }

        private class AuctionEventParser {
            private Dictionary<string, Action<IDictionary<string, string>>> mActions = new Dictionary<string, Action<IDictionary<string, string>>>();

            public void RegisterAction(string inEventType, Action<IDictionary<string, string>> inAction) {
                mActions[inEventType] = inAction;
            }

            public void RunActionFrom(Message inMessage) {
                var ev = inMessage.Body
                    .Split(new char[] {';'})
                    .Select(elem => elem.Split(new char[] {':'}).Select(s => s.Trim()).ToList())
                    .Where(elem => elem.Count > 1)
                    .ToDictionary(elem => elem[0], elem => elem[1])
                ;

                Action<IDictionary<string, string>> action;
                if (mActions.TryGetValue(ev["Event"], out action)) {
                    action(ev);
                }
            }
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

        public AuctionMessageTranslator Translator {get; set;}
    }
}

