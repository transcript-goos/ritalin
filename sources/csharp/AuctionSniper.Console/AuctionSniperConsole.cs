using System;

namespace AuctionSniper.Console {
    public class AuctionSniperConsole {
        public AuctionSniperConsole() {
        }

        private void ShowStatus(SniperStatus inStatus) {
            System.Console.WriteLine("status changed to: {0}", inStatus);
        }

        public void RunShell(AuctionCredencial inCredencial) {

        }

        public void Execute(string[] inArgs) {

        }

        public void Terminate() {

        }

        public SniperStatus Status {get; private set;}
    }

    public struct AuctionCredencial {
        public string Id {get;set;}
        public string Password {get;set;}
    }
}

