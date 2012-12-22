using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;
using System.Linq;

using jabber.client;
using jabber.protocol.client;

namespace AuctionSniper.Test {
    internal class MessageQueue {
        private Queue<Message> mMessages = new Queue<Message>();
        private object mLock = new object();
        
        public void AssignEvents(JabberClient inConn) {
            inConn.OnMessage += (s, msg) => {
                lock (mLock) {
                    mMessages.Enqueue(msg);
                    
                    Monitor.PulseAll(mLock);
                }
            };
        }

        public bool TryPoll(TimeSpan inTimeout, TimeSpan inWait, out Message outItem) {
            outItem = null;
            
            var endTime = DateTime.Now.Add(inTimeout);
            
            while (DateTime.Now < endTime) {
                lock (mLock) {                
//                    if (mMessages.TryDequeue(out outItem)) {
                    if (mMessages.Any()) {
                        outItem = mMessages.Dequeue();
                        return true;
                    }
                    
                    Monitor.Wait(mLock, inWait);
                }
            }

            return false;
        }

        public bool TryPoll(TimeSpan inTimeout, out Message outItem) {
            return this.TryPoll(inTimeout, TimeSpan.FromMilliseconds(100), out outItem);
        }
    } 
}

