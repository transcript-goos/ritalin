using System;

using AuctionSniper.Console;

namespace AuctionSniper.Test {
    internal class ApplicationRunner {
        public static readonly string SniperId = "sniper";
        public static readonly string SniperPassword = "sniper";

        private IAuctionSniperDriver mDriver;

        public void StartBiddingIn(FakeAuctionServer inAuction) {
            mDriver = new ConsoleAuctionSniperDriver(SniperId, SniperPassword, 1000);
            mDriver.ShowSniperStatus(SniperStatus.Joining);
        }

        public void ShowsSniperHasLostAuction() {
            mDriver.ShowSniperStatus(SniperStatus.Lost);
        }

        public void Stop() {
            if (mDriver != null) {
                mDriver.Dispose();
                mDriver = null;            
            }
        }
    }

    internal interface IAuctionSniperDriver {
        void ShowSniperStatus(SniperStatus inStatus);
        void Dispose();
    }

    internal class ConsoleAuctionSniperDriver : IAuctionSniperDriver {
        private AuctionSniperConsole mApp;

        public ConsoleAuctionSniperDriver(string inId, string inPassword, int inTimeout) {
            mApp = new AuctionSniperConsole();
        }

        public void ShowSniperStatus(SniperStatus inStatus) {
            mApp.ShowStatus(inStatus);
        }

        public void Dispose() {
            mApp.Terminate();
        }
    }
}

