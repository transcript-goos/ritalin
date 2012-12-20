using System;

namespace AuctionSniper.Core {
    public interface ISniperListener {
        void SniperLost();
        void SniperBidding();
    }

    public interface IAuction {
        void Bid(int inNewPrice);
    }

    public class AuctionSniper : IAuctionEventListener {
        private IAuction mAuction;
        private ISniperListener mListener;

        public AuctionSniper(IAuction inAuction, ISniperListener inListener) {
            mAuction = inAuction;
            mListener = inListener;
        }

        void IAuctionEventListener.AuctionClosed() {
            mListener.SniperLost();

        }

        void IAuctionEventListener.CurrentPrice(int inPrice, int inIncrement) {
            mAuction.Bid(inPrice+inIncrement);
            mListener.SniperBidding();
        }
    }
}

