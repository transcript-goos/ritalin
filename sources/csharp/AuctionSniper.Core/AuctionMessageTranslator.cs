using System;
using System.Collections.Generic;
using System.Linq;

using jabber;
using jabber.client;
using jabber.protocol.client;

namespace AuctionSniper.Core {
    public enum PriceSource {
        Unknown = 0,
        FromSniper,
        FromOtherBidder,
    }
    
    public interface IAuctionEventListener {
        void AuctionClosed();
        void CurrentPrice(int inPrice, int inIncrement, PriceSource inBidderSource);
    }
    
    public interface IMessageListener {
        void ProcessMessage(IChat inChat, Message inMessage);
    }

    public class AuctionMessageTranslator : IMessageListener {
        private AuctionEventParser mParser = new AuctionEventParser();
        private JID mSniperId;

        public AuctionMessageTranslator(JID inSniperJId, IAuctionEventListener inListener) {
            mSniperId = inSniperJId;

            mParser.RegisterAction("CLOSE", (ev) => {
                inListener.AuctionClosed();
            });
            mParser.RegisterAction("PRICE", (ev) => {
                inListener.CurrentPrice(
                    ev.IntegerValueOf("CurrentPrice"),
                    ev.IntegerValueOf("Increment"),
                    ev.PriceSourceFrom(mSniperId)
                );
            });
        }

        public void ProcessMessage(IChat inChar, Message inMessage) {
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

            public PriceSource PriceSourceFrom(JID inSniperId) {
                return this.ValueOf("Bidder") == inSniperId.User ? PriceSource.FromSniper : PriceSource.FromOtherBidder;
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
}

