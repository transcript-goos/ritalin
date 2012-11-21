using System;

using jabber;
using jabber.protocol.client;

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

        public void HasShownSniperInBidding() {
            mDriver.ShowSniperStatus(SniperStatus.Bidding);
        }

        public void ShowsSniperHasLostAuction() {
            mDriver.HasReceivedMessageFromServer();
            mDriver.ShowSniperStatus(SniperStatus.Lost);
        }

        public void Stop() {
            if (mDriver != null) {
                mDriver.Dispose();
                mDriver = null;            
            }
        }

        public JID JId {
            get {
                return mDriver.JId;
            }
        }
    }

    internal interface IAuctionSniperDriver {        
        void HasReceivedMessageFromServer();       
        void ShowSniperStatus(SniperStatus inStatus);
        void Dispose();

        JID JId {get;}
    }

    internal class ConsoleAuctionSniperDriver : IAuctionSniperDriver {
        private AuctionSniperConsole mApp;
        private MessageQueue mQueue;

        public ConsoleAuctionSniperDriver(string inHostName, string inId, string inPassword, int inTimeout) {
            mApp = new AuctionSniperConsole();

            mQueue = new MessageQueue();
            mQueue.AssignEvents(mApp.Connection);

            Assert.That(mApp.Status, Is.EqualTo(SniperStatus.Disconnected));

            this.JId = TestHelper.ToJId(inId);

            mApp.RunShell(
                inHostName,
                    new AuctionCredencial {Id = this.JId, Password = inPassword}
            );

            TestHelper.WaitConnectingTo(mApp.Connection);           
        }

        void IAuctionSniperDriver.HasReceivedMessageFromServer() {
            Message msg;
            Assert.That(mQueue.TryPoll(TimeSpan.FromSeconds(5), out msg), Is.True, "expired at 5 seconds");
            Assert.That(msg, Is.Not.Null);
        }

        void IAuctionSniperDriver.ShowSniperStatus(SniperStatus inStatus) {
            Assert.That(mApp.Status, Is.EqualTo(inStatus));
        }

        void IAuctionSniperDriver.Dispose() {
            mApp.Terminate();

            TestHelper.WaitDisconnectingTo(mApp.Connection);

            Assert.That(mApp.Status, Is.EqualTo(SniperStatus.Disconnected));
        }

        public JID JId {get; private set;}
    }
}

