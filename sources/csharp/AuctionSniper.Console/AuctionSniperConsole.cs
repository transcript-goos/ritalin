using System;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

using AuctionSniper.Core;

namespace AuctionSniper.Console {
    public class AuctionSniperConsole : ISniperListener {
        public static readonly string AuctionResource = "Auction";
        public static readonly string ItemIdAsLogin = "auction-{0}";

        public static readonly string ReportCommandFormat = "SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2}";
        public static readonly string BidCommandFormat = "SOLVersion: 1.1; Command: BID; Price: {0}";
        public static readonly string JoinCommandFormat = "SOLVersion: 1.1; Command: JOIN;}";

        private class AuctionImpl : IAuction {
            private Action<int> mCallback;

            public AuctionImpl(Action<int> inCallback) {
                mCallback = inCallback;
            }

            void IAuction.Bid(int inNewPrice) { 
                if (mCallback != null) {
                    try {
                    mCallback(inNewPrice);
                    }
                    catch (Exception ex) {
                        System.Console.WriteLine(ex.StackTrace);
                    }
                }
            }
        }

        public AuctionSniperConsole() {
        }

        private void ShowStatus(SniperStatus inStatus) {
            this.Status = inStatus;

            System.Console.WriteLine("status changed to: {0}", inStatus);
        }

        public void RunShell(string inHostName, AuctionCredencial inCredencial, string inItemId) {
            this.JoinAuction(this.Connection(inHostName, inCredencial), inItemId);
        }

        private void JoinAuction(JabberClient inConnection, string inItemId) {
            this.NotToBeGCD = new Chat(
                this.ToJid(inItemId, inConnection), 
                inConnection
            );

            var auction = new AuctionImpl((price) => {
                this.NotToBeGCD.SendMessage(
                    new Message(new XmlDocument()) {Body = string.Format(BidCommandFormat, price)}
                );
            });

            this.NotToBeGCD.Translator = new AuctionMessageTranslator(new AuctionSniper.Core.AuctionSniper(auction, this));

            if (this.BeginJoining != null) {
                this.BeginJoining(this.NotToBeGCD.Connection);
            }
            
            inConnection.Connect();
            ConsoleAppHelper.WaitConnectingTo(inConnection);  

            this.NotToBeGCD.SendMessage(new Message(new XmlDocument()) {Body = JoinCommandFormat});
            
            this.ShowStatus(SniperStatus.Joining);
        }

        private JabberClient Connection(string inHostName, AuctionCredencial inCredencial) {
            var connection = new JabberClient();

            connection.AutoLogin = true;
            connection.AutoStartTLS = false;
            connection.User = inCredencial.Id.User;
            connection.Server = inCredencial.Id.Server;
            connection.Resource = inCredencial.Id.Resource;
            connection.Password = inCredencial.Password;
            connection.Port = 5222;
            connection.NetworkHost = inHostName;

            return connection;
        }

        private JID ToJid(string inItemId, JabberClient inConnection) {
            return new JID(
                string.Format(ItemIdAsLogin, inItemId),
                inConnection.Server,
                inConnection.Resource
            );
        }

        public void Terminate() {
            if (! this.NotToBeGCD.Connection.IsAuthenticated) return;

            this.NotToBeGCD.Connection.Close();
            ConsoleAppHelper.WaitDisconnectingTo(this.NotToBeGCD.Connection);

            this.ShowStatus(SniperStatus.Disconnected);
        }

        void ISniperListener.SniperLost() {
            this.ShowStatus(SniperStatus.Lost);
        }

        void ISniperListener.SniperBidding() {
            this.ShowStatus(SniperStatus.Bidding);
        }

        public Chat NotToBeGCD { get; private set; }
        
        public SniperStatus Status {get; private set;}

        public event Action<JabberClient> BeginJoining;
    }

    public struct AuctionCredencial {
        public JID Id {get;set;}
        public string Password {get;set;}
    }
}

