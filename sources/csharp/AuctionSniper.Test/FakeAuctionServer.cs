using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

using NUnit.Framework;

namespace AuctionSniper.Test {
    internal class FakeAuctionServer {
        private static readonly string ItemIdAsLogin = "auction-{0}";
        private static readonly string AuctionPassword = "auction";
        private static readonly string DomainName = "ktz.local";

        private MessageQueue mQueue = new MessageQueue();

        private JID mChat;

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
            TestHelper.WaitConnectingTo(this.Connection);
        }

        public void HasReceivedJoinRequestFromSniper() {
            mChat = this.ReceiveMessage().From;
        }
        
        private Message ReceiveMessage() {
            Message result;
            
            Assert.That(mQueue.TryPoll(TimeSpan.FromSeconds(5), out result), Is.True,  "5秒間に何らかのメッセージが送られてこなければならない");
            Assert.That(result, Is.Not.Null);
            
            return result;
        }

        public void AnnounceClosed() {
            var msg = new Message(new XmlDocument()) {
                Type = MessageType.chat, 
                To = mChat,
            };
                
            this.Connection.Write(msg);
        }

        public void Stop() {
            if (! this.Connection.IsAuthenticated) return;

            this.Connection.Close();
            TestHelper.WaitDisconnectingTo(this.Connection);
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

