using System;

using jabber.protocol.client;

namespace AuctionSniper.Core {
    public class AuctionMessageTranslator {
        public AuctionMessageTranslator(AuctionEventListener inListener) {
            this.Listenr = inListener;
        }

        public void ProcessMessage(Chat inChar, Message inMessage) {
            this.Listenr.AuctionClosed();
        }

        public AuctionEventListener Listenr {get; private set;}
    }

    public class AuctionEventListener {
        public  virtual void AuctionClosed() {

        }
    }

    public class Chat {
    }
}

