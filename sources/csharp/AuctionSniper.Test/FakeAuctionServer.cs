using System;
using System.Collections.Concurrent;
using System.Threading;

using agsXMPP;
using agsXMPP.protocol.client;

using NUnit.Framework;

namespace AuctionSniper.Test {
    internal class FakeAuctionServer {
        private static readonly string ItemIdAsLogin = "auction-{0}";
        private static readonly string AuctionPassword = "auction";
        private static readonly string AuctionResource = "Auction";
        private static readonly string HostName = "localhost";

        private MessageQueue mQueue = new MessageQueue();

        public FakeAuctionServer(string inItemId) {
            this.ItemId = inItemId;
            this.Connection = new XmppClientConnection(HostName, 9090);
        }

        public void StartSellingItem() {
            Jid jid = new Jid(string.Format(ItemIdAsLogin, this.ItemId));
            
//            PrintHelp(String.Format("Enter password for '{0}': ", jid.ToString()));
            
            this.Connection.Password = AuctionPassword;
            this.Connection.Username = jid.User;
            this.Connection.Resource = AuctionResource;

            this.Connection.AutoAgents = false;
            this.Connection.AutoPresence = true;
            this.Connection.AutoRoster = true;
            this.Connection.AutoResolveConnectServer = true;

            mQueue.Connect(this.Connection);

            //            xmppCon.OnRosterStart += new ObjectHandler(xmppCon_OnRosterStart);
//            xmppCon.OnRosterItem += new XmppClientConnection.RosterHandler(xmppCon_OnRosterItem);
//            xmppCon.OnRosterEnd += new ObjectHandler(xmppCon_OnRosterEnd);
//            xmppCon.OnPresence += new PresenceHandler(xmppCon_OnPresence);

//            xmppCon.OnLogin += new ObjectHandler(xmppCon_OnLogin);
            
            this.Connection.Open();        
        }

        public void HasReceivedJoinRequestFromSniper() {
            mQueue.ReceiveMessage();
        }

        public void AnnounceClosed() {
            this.Connection.Send("");
        }

        public void Stop() {
            this.Connection.Close();
        }

        public XmppClientConnection Connection {get; private set;}
        public string ItemId {get;private set;}
    }

    internal class MessageQueue {
        private ConcurrentQueue<Message> mMessages = new ConcurrentQueue<Message>();

        public void Connect(XmppClientConnection inConn) {
            inConn.OnMessage += (s, msg) => {
                if (msg.Body != null) {
                    mMessages.Enqueue(msg);
                }
            };
        }

        public Message ReceiveMessage() {
            Message result;

            Assert.That(mMessages.TryPoll(TimeSpan.FromSeconds(5), out result), Is.True,  "5秒間に何らかのメッセージが送られてこなければならない");
            Assert.That(result, Is.Not.Null);

            return result;
        }
    }

    public static class QueueExtensions {
        public static bool TryPoll<TResult>(this ConcurrentQueue<TResult> inSelf, TimeSpan inTimeout, out TResult outItem) {
            return inSelf.TryPoll(inTimeout, TimeSpan.FromMilliseconds(100), out outItem);
        }

        public static bool TryPoll<TResult>(this ConcurrentQueue<TResult> inSelf, TimeSpan inTimeout, TimeSpan inWait, out TResult outItem) {
            var endTime = DateTime.Now.Add(inTimeout);

            do {
                if (inSelf.TryPeek(out outItem)) {
                    return true;
                }

                Thread.Sleep(inWait);
            }
            while (DateTime.Now < endTime);

            return false;
        }
    } 
}

