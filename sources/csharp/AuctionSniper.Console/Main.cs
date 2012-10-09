using System;

namespace AuctionSniper.Console {
    class MainClass {
        public static void Main(string[] args) {
            var console = new AuctionSniperConsole();

            console.RunShell("", new AuctionCredencial());
        }
    }
}
