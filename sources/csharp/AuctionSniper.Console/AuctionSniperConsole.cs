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

        private class XMPPAuction : IAuction {
            public XMPPAuction(Chat inChat, ISniperListener inListener) {
                this.NotToBeGCD = inChat;
                this.NotToBeGCD.Translator = new AuctionMessageTranslator(new AuctionSniper.Core.AuctionSniper(this, inListener));
            }

            void IAuction.Join() {
                this.NotToBeGCD.SendMessage(
                    new Message(new XmlDocument()) {Body = JoinCommandFormat}
                );
            }

            void IAuction.Bid(int inNewPrice) { 
                try {
                    this.NotToBeGCD.SendMessage(
                        new Message(new XmlDocument()) {Body = string.Format(BidCommandFormat, inNewPrice)}
                    );
                }
                catch (Exception ex) {
                    System.Console.WriteLine(ex.StackTrace);
                }
            }
 
            public Chat NotToBeGCD { get; private set; }
        }

        private IAuction mAuction;

        private void ShowStatus(SniperStatus inStatus) {
            this.Status = inStatus;

            System.Console.WriteLine("status changed to: {0}", inStatus);
        }

        public void RunShell(string inHostName, AuctionCredencial inCredencial, string inItemId) {
            this.JoinAuction(this.Connection(inHostName, inCredencial), inItemId);
        }

        private void JoinAuction(JabberClient inConnection, string inItemId) {
            mAuction = new XMPPAuction(
                new Chat(this.ToJid(inItemId, inConnection), inConnection), this
            );

            if (this.BeginJoining != null) {
                this.BeginJoining(mAuction.NotToBeGCD.Connection);
            }
            
            inConnection.Connect();
            ConsoleAppHelper.WaitConnectingTo(inConnection);  

            mAuction.Join();

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
            if (! mAuction.NotToBeGCD.Connection.IsAuthenticated) return;

            mAuction.NotToBeGCD.Connection.Close();
            ConsoleAppHelper.WaitDisconnectingTo(mAuction.NotToBeGCD.Connection);

            this.ShowStatus(SniperStatus.Disconnected);
        }

        void ISniperListener.SniperLost() {
            this.ShowStatus(SniperStatus.Lost);
        }

        void ISniperListener.SniperBidding() {
            this.ShowStatus(SniperStatus.Bidding);
        }

        public SniperStatus Status {get; private set;}

        public event Action<JabberClient> BeginJoining;
    }

    public struct AuctionCredencial {
        public JID Id {get;set;}
        public string Password {get;set;}
    }
}

