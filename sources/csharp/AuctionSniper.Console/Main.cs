using System;

namespace AuctionSniper.Console {
    class MainClass {
        // ex {"ktz.local", "sniper", "sniper", "item-54321"}
        public static void Main(string[] args) {
            var console = new AuctionSniperConsole();
            var credencial = new AuctionCredencial {
                Id = args[1],
                Password = args[2]
            };

            console.RunShell(args[0], credencial, args[3]);

            console.Terminate();
        }
    }
}
