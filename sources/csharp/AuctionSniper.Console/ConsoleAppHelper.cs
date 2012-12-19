using System;
using System.Threading;

using jabber.client;

namespace AuctionSniper.Console {
    public static class ConsoleAppHelper {
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

