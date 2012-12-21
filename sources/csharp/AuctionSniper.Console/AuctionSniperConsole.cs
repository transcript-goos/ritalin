using System;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

using AuctionSniper.Core;

namespace AuctionSniper.Console {
    public class AuctionSniperConsole {
        public static readonly string AuctionResource = "Auction";
        public static readonly string ItemIdAsLogin = "auction-{0}";

        public static readonly string ReportCommandFormat = "SOLVersion: 1.1; Event: PRICE; CurrentPrice: {0}; Increment: {1}; Bidder: {2}";
        public static readonly string BidCommandFormat = "SOLVersion: 1.1; Command: BID; Price: {0}";
        public static readonly string JoinCommandFormat = "SOLVersion: 1.1; Command: JOIN;}";

        private IAuction mAuction;

        public void RunShell(string inHostName, AuctionCredencial inCredencial, string inItemId) {
            this.JoinAuction(this.Connection(inHostName, inCredencial), inItemId);
        }

        private void JoinAuction(JabberClient inConnection, string inItemId) {
            mAuction = new XMPPAuction(
                new Chat(this.ToJid(inItemId, inConnection), inConnection), new SniperStateDisplayer()
            );

            if (this.BeginJoining != null) {
                this.BeginJoining(mAuction.NotToBeGCD.Connection);
            }
            
            inConnection.Connect();
            ConsoleAppHelper.WaitConnectingTo(inConnection);  

            mAuction.Join();
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

            mAuction.Disconnect();
        }

        public event Action<JabberClient> BeginJoining;
        
        public SniperStatus Status {
            get {
                return mAuction != null ? mAuction.Status : SniperStatus.Disconnected;
            }
        }

        private class XMPPAuction : IAuction {
            private ISniperListener mListener;

            public XMPPAuction(Chat inChat, ISniperListener inListener) {
                this.NotToBeGCD = inChat;
                this.NotToBeGCD.Translator = new AuctionMessageTranslator(new AuctionSniper.Core.AuctionSniper(this, inListener));

                mListener = inListener;
            }
            
            void IAuction.Join() {
                this.NotToBeGCD.SendMessage(
                    new Message(new XmlDocument()) {Body = JoinCommandFormat}
                );

                mListener.SniperJoining();
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

            void IAuction.Disconnect() {
                this.NotToBeGCD.Connection.Close();
                ConsoleAppHelper.WaitDisconnectingTo(this.NotToBeGCD.Connection);

                mListener.AuctionDisconnected();
            }

            public Chat NotToBeGCD { get; private set; }

            public SniperStatus Status {
                get {
                    return mListener.Status;
                }
            }
        }

        private class SniperStateDisplayer : ISniperListener {
            void ISniperListener.SniperJoining() {
                this.ShowStatus(SniperStatus.Joining);
            }

            void ISniperListener.SniperLost() {
                this.ShowStatus(SniperStatus.Lost);
            }
            
            void ISniperListener.SniperBidding() {
                this.ShowStatus(SniperStatus.Bidding);
            }

            void ISniperListener.AuctionDisconnected() {
                this.ShowStatus(SniperStatus.Disconnected);
            }

            private void ShowStatus(SniperStatus inStatus) {
                this.Status = inStatus;
                
                System.Console.WriteLine("status changed to: {0}", inStatus);
            }

            public SniperStatus Status {get; private set;}
        }
    }

    public struct AuctionCredencial {
        public JID Id {get;set;}
        public string Password {get;set;}
    }
}

