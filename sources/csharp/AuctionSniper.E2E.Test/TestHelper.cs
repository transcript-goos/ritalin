using System;
using System.Threading;

using jabber;
using jabber.client;

namespace AuctionSniper.Test {
    public static class TestHelper {
        public static readonly string JIdResource = "auction";
        public static readonly string JIdServer = "ktz.local";
        
        public static JID ToJId(string inUserName) {
            return new JID(inUserName, JIdServer, JIdResource); 
        }

        public static void WaitConnectingTo(JabberClient inClient) {
            if (inClient.IsAuthenticated) return;
            
            var locker = new object();
            var connected = false;
            
            inClient.OnAuthenticate += (s) => {
                lock (locker) {
                    connected = true;
                    Monitor.PulseAll(locker);
                }
            };

            while (true) {
                lock (locker) {
                    Monitor.Wait(locker, TimeSpan.FromMilliseconds(100));
                    
                    if (connected) break;
                }
            }
        }      

        public static void WaitDisconnectingTo(JabberClient inClient) {
            if (! inClient.IsAuthenticated) return;

            var locker = new object();
            var connected = true;
            
            inClient.OnDisconnect += (s) => {
                lock (locker) {
                    connected = false;
                    Monitor.PulseAll(locker);
                }
            };

            while (true) {
                lock (locker) {
                    Monitor.Wait(locker, TimeSpan.FromMilliseconds(100));
                    
                    if (! connected) break;
                }
            }
        }
    }
}

