using System;

namespace AuctionSniper.Core {
    public interface ISniperListener {
        void SniperJoining();
        void SniperLost();
        void SniperBidding();
        void SniperWinning();
        void SniperWon();
        void AuctionDisconnected();

        SniperStatus Status {get;}
    }

    public interface IAuction {
        void Join();
        void Bid(int inNewPrice);
        void Disconnect();

        IChat NotToBeGCD { get; }
        SniperStatus Status {get;}
    }

    public class AuctionSniper : IAuctionEventListener {
        private IAuction mAuction;
        private ISniperListener mListener;

        public AuctionSniper(IAuction inAuction, ISniperListener inListener) {
            mAuction = inAuction;
            mListener = inListener;
        }

        void IAuctionEventListener.AuctionClosed() {
            if (mListener.Status == SniperStatus.Winning) {
                mListener.SniperWon();
            }
            else {
                mListener.SniperLost();
            }
        }

        void IAuctionEventListener.CurrentPrice(int inPrice, int inIncrement, PriceSource inBidderSource) {
            switch (inBidderSource) {
            case PriceSource.FromSniper:
                mListener.SniperWinning();
                break;

            case PriceSource.FromOtherBidder:  
                mAuction.Bid(inPrice+inIncrement);                
                mListener.SniperBidding();
                break;
            }
        }
    }
}

