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
    }
}

