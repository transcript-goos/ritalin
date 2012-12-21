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
                    ev.IntegerValueOf("CurrentPrice"),
                    ev.IntegerValueOf("Increment")
                );
            });
        }

        public void ProcessMessage(Chat inChar, Message inMessage) {
            mParser.RunActionFrom(inMessage);
        }

        private class AuctionEvent {
            private IDictionary<string, string> mResult;

            public AuctionEvent(IDictionary<string, string> inResult) {
                mResult = inResult;
            }

            public string EventType {
                get {
                    return this.ValueOf("Event");
                }
            }

            public string ValueOf(string inField) {
                return mResult[inField];
            }

            public int IntegerValueOf(string inField) {
                return int.Parse(this.ValueOf(inField));
            }
        }

        private class AuctionEventParser {
            private Dictionary<string, Action<AuctionEvent>> mActions = new Dictionary<string, Action<AuctionEvent>>();

            public void RegisterAction(string inEventType, Action<AuctionEvent> inAction) {
                mActions[inEventType] = inAction;
            }

            public void RunActionFrom(Message inMessage) {
                var ev = new AuctionEvent(inMessage.Body
                    .Split(new char[] {';'})
                    .Select(elem => elem.Split(new char[] {':'}).Select(s => s.Trim()).ToList())
                    .Where(elem => elem.Count > 1)
                    .ToDictionary(elem => elem[0], elem => elem[1])
                );

                Action<AuctionEvent> action;
                if (mActions.TryGetValue(ev.EventType, out action)) {
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

