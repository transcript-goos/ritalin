using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

using AuctionSniper.Console.Utils;

using NUnit.Framework;

namespace AuctionSniper.Test {
    internal class FakeAuctionServer {
        private static readonly string ItemIdAsLogin = "auction-{0}";
        private static readonly string AuctionPassword = "auction";
        private static readonly string DomainName = "ktz.local";

        private MessageQueue mQueue = new MessageQueue();

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

            JabberClientHelper.ConnectServer(this.Connection);
        }

        public void HasReceivedJoinRequestFromSniper() {
            mQueue.ReceiveMessage();
        }

        public void AnnounceClosed() {
            this.Connection.Write(new Message(new XmlDocument()));
        }

        public void Stop() {
            JabberClientHelper.DisconnectServer(this.Connection);
        }

        public JabberClient Connection {get; private set;}
        public string ItemId {get;private set;}
        public string HostName {
            get {
                return DomainName;
            }
        }
    }

    internal class MessageQueue {
        private ConcurrentQueue<Message> mMessages = new ConcurrentQueue<Message>();
        private object mLock = new object();

        public void AssignEvents(JabberClient inConn) {
            inConn.OnMessage += (s, msg) => {
//                if (msg.Body != null) 
                {
                    lock (mLock) {
                        mMessages.Enqueue(msg);

                        Monitor.PulseAll(mLock);
                    }
                }
            };
        }

        public bool TryPoll(TimeSpan inTimeout, TimeSpan inWait, out Message outItem) {
            outItem = null;

            var endTime = DateTime.Now.Add(inTimeout);
            
            while (DateTime.Now < endTime) {
                lock (mLock) {                
                    if (mMessages.TryDequeue(out outItem)) {
                        return true;
                    }

                    Monitor.Wait(mLock, inWait);
                }
            }
            
            return false;
        }

        public Message ReceiveMessage() {
            Message result;

            Assert.That(this.TryPoll(TimeSpan.FromSeconds(5), out result), Is.True,  "5秒間に何らかのメッセージが送られてこなければならない");
            Assert.That(result, Is.Not.Null);

            return result;
        }
    }

    public static class QueueExtensions {
        internal static bool TryPoll(this MessageQueue inSelf, TimeSpan inTimeout, out Message outItem) {
            return inSelf.TryPoll(inTimeout, TimeSpan.FromMilliseconds(100), out outItem);
        }
    } 
}

