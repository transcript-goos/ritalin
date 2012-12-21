using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

using AuctionSniper.Core;
using AuctionSniper.Console;

using NUnit.Framework;

namespace AuctionSniper.Test {
    internal class FakeAuctionServer {
        private static readonly string ItemIdAsLogin = "auction-{0}";
        private static readonly string AuctionPassword = "auction";
        private static readonly string DomainName = "ktz.local";

        private MessageQueue mQueue = new MessageQueue();

        private IChat mChat;

        public FakeAuctionServer(string inItemId) {
            this.ItemId = inItemId;
            this.Connection = new JabberClient();
            this.Connection.Port = 5222;
            this.Connection.NetworkHost = DomainName;
            this.Connection.AutoLogin = true;
            this.Connection.AutoStartTLS = false;
        }

        public void StartSellingItem() {
            var id = TestHelper.ToJId(
                string.Format(ItemIdAsLogin, this.ItemId)
            );
            
            this.Connection.User = id.User;
            this.Connection.Server = id.Server;
            this.Connection.Resource = id.Resource;
            this.Connection.Password = AuctionPassword;

            mQueue.AssignEvents(this.Connection);

            this.Connection.Connect();
            ConsoleAppHelper.WaitConnectingTo(this.Connection);
        }

        public void HasReceivedJoinRequestFrom(JID inJId) {
            mChat = new Chat(this.ReceiveMessage(inJId, Is.Not.Empty).From, this.Connection);
        }

        public void HasReceivedBid(int inBidPrice, string inSniperJId) {
            this.ReceiveMessage(
                inSniperJId,
                //  ToDo: AuctionSniper.Console.MainClassに強依存するのが気に入らない。後で治るのかこれ？
                Is.EqualTo(string.Format(AuctionSniperConsole.BidCommandFormat, inBidPrice))
            );
        }

        private Message ReceiveMessage(JID inJId, NUnit.Framework.Constraints.Constraint inMatcher) {
            Message result;
            
            Assert.That(mQueue.TryPoll(TimeSpan.FromSeconds(5), out result), Is.True,  "5秒間に何らかのメッセージが送られてこなければならない");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Body, inMatcher);
            Assert.That(result.From.ToString(), Is.EqualTo(inJId.ToString()));

            return result;
        }

        public void ReportPrice(int inPrice, int inInc,  string inBidder) {
            var msg = new Message(new XmlDocument()) {
                //  ToDo: AuctionSniper.Console.MainClassに強依存するのが気に入らない。後で治るのかこれ？
                Body = string.Format(AuctionSniperConsole.ReportCommandFormat, inPrice, inInc, inBidder),
            };
            
            mChat.SendMessage(msg);  
        }

        public void AnnounceClosed() {
            var msg = new Message(new XmlDocument()) {
                Body = "SOLVersion: 1.1; Event: CLOSE;",
            };
                
            mChat.SendMessage(msg);
        }

        public void Stop() {
            if (! this.Connection.IsAuthenticated) return;

            this.Connection.Close();
            ConsoleAppHelper.WaitDisconnectingTo(this.Connection);
        }

        public JabberClient Connection {get; private set;}
        public string ItemId {get;private set;}
        public string HostName {
            get {
                return DomainName;
            }
        }
    }
}

