using System;

namespace AuctionSniper.Core {
    public enum SniperStatus {
        Disconnected = 0,
        Joining,
        Bidding,
        Winning,
        Won,
        Lost,
    }
}

