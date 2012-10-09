using System;
using System.Threading;

using jabber.client;

namespace AuctionSniper.Console.Utils {
    public static class JabberClientHelper {
        public static void ConnectServer(JabberClient inClient) {
            var locker = new object();
            var connected = false;
            
            inClient.OnAuthenticate += (s) => {
                lock (locker) {
                    connected = true;
                    Monitor.PulseAll(locker);
                }
            };
            
            inClient.Connect();  
            
            while (true) {
                lock (locker) {
                    Monitor.Wait(locker, TimeSpan.FromMilliseconds(100));
                    
                    if (connected) break;
                }
            }
        }  

        public static void DisconnectServer(JabberClient inClient) {
            var locker = new object();
            var connected = true;

            inClient.OnDisconnect += (s) => {
                lock (locker) {
                    connected = false;
                    Monitor.PulseAll(locker);
                }
            };

            inClient.Close();
            
            while (true) {
                lock (locker) {
                    Monitor.Wait(locker, TimeSpan.FromMilliseconds(100));
                    
                    if (! connected) break;
                }
            }
        }
    }
}

