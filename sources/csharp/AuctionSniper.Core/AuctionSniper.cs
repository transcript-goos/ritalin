using System;

namespace AuctionSniper.Core {
    public interface ISniperListener {
        void SniperLost();
    }

    public class AuctionSniper : IAuctionEventListener {
        private ISniperListener mListener;

        public AuctionSniper(ISniperListener inListener) {
            mListener = inListener;
        }

        void IAuctionEventListener.AuctionClosed() {
            mListener.SniperLost();

        }

        void IAuctionEventListener.CurrentPrice(int inPrice, int inIncrement) {
        }
    }
}

