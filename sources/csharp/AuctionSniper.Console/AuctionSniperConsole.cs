using System;

namespace AuctionSniper.Console {
    public class AuctionSniperConsole {
        public AuctionSniperConsole() {
        }

        public void ShowStatus(SniperStatus inStatus) {
            System.Console.WriteLine("status changed to: {0}", inStatus);
        }

        public void Execute(string[] inArgs) {

        }

        public void Terminate() {

        }
    }
}

