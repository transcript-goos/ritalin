using System;

namespace AuctionSniper.Console {
    class MainClass {
        public static readonly string JoinCommandFormat = "SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2}";
        public static readonly string BidCommandFormat = "SOLVersion: 1.1; Command: BID; Price: {0}";

        public static void Main(string[] args) {
            var console = new AuctionSniperConsole();

            console.RunShell("", new AuctionCredencial());
        }
    }
}
