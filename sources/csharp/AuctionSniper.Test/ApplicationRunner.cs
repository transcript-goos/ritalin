using System;

using AuctionSniper.Console;

using NUnit.Framework;

namespace AuctionSniper.Test {
    internal class ApplicationRunner {
        public static readonly string SniperId = "sniper";
        public static readonly string SniperPassword = "sniper";

        private IAuctionSniperDriver mDriver;

        public void StartBiddingIn(FakeAuctionServer inAuction) {
            mDriver = new ConsoleAuctionSniperDriver(inAuction.HostName, SniperId, SniperPassword, 1000);
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

        public ConsoleAuctionSniperDriver(string inHostName, string inId, string inPassword, int inTimeout) {
            mApp = new AuctionSniperConsole();

            Assert.That(mApp.Status, Is.EqualTo(SniperStatus.Disconnected));

            mApp.RunShell(
                inHostName,
                new AuctionCredencial {Id = TestHelper.ToJId (inId), Password = inPassword}
            );
        }

        public void ShowSniperStatus(SniperStatus inStatus) {
            Assert.That(mApp.Status, Is.EqualTo(inStatus));
        }

        public void Dispose() {
            mApp.Terminate();

            Assert.That(mApp.Status, Is.EqualTo(SniperStatus.Disconnected));
        }
    }
}

