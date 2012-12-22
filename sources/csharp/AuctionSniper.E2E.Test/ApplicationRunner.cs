using System;

using jabber;
using jabber.protocol.client;

using AuctionSniper.Core;
using AuctionSniper.Console;

using NUnit.Framework;

namespace AuctionSniper.Test {
    internal class ApplicationRunner {
        public static readonly string SniperId = "sniper";
        public static readonly string SniperPassword = "sniper";

        private IAuctionSniperDriver mDriver;

        public void StartBiddingIn(FakeAuctionServer inAuction) {
            mDriver = new ConsoleAuctionSniperDriver(
                inAuction.HostName, SniperId, SniperPassword, 
                inAuction.ItemId, 
                1000
            );
            mDriver.ShowSniperStatus(SniperStatus.Joining);
        }

        public void HasShownSniperInBidding() {
            mDriver.HasReceivedMessageFromServer();
            mDriver.ShowSniperStatus(SniperStatus.Bidding);
        }

        public void HasShownSniperInWinning() {
            mDriver.HasReceivedMessageFromServer();
            mDriver.ShowSniperStatus(SniperStatus.Winning);
        }

        public void ShowsSniperHasLostAuction() {
            mDriver.HasReceivedMessageFromServer();
            mDriver.ShowSniperStatus(SniperStatus.Lost);
        }

        public void ShowsSniperHasWonAuction() {
            mDriver.HasReceivedMessageFromServer();
            mDriver.ShowSniperStatus(SniperStatus.Won);
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

        public ConsoleAuctionSniperDriver(string inHostName, string inId, string inPassword, string inAuctionItem, int inTimeout) {
            mQueue = new MessageQueue();
            
            mApp = new AuctionSniperConsole();
            mApp.BeginJoining += (conn) => {
                mQueue.AssignEvents(conn);
            };
            Assert.That(mApp.Status, Is.EqualTo(SniperStatus.Disconnected));

            this.JId = TestHelper.ToJId(inId);

            mApp.RunShell(
                inHostName,
                new AuctionCredencial {Id = this.JId, Password = inPassword},
                inAuctionItem               
            );
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

            Assert.That(mApp.Status, Is.EqualTo(SniperStatus.Disconnected));
        }

        public JID JId {get; private set;}
    }
}

