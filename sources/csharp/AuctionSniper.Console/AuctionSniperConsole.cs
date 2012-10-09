using System;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

namespace AuctionSniper.Console {
    public class AuctionSniperConsole {
        public static readonly string AuctionResource = "Auction";
        public static readonly string ItemIdAsLogin = "auction-{0}";

        private JabberClient mConnection;

        public AuctionSniperConsole() {
        }

        private void ShowStatus(SniperStatus inStatus) {
            System.Console.WriteLine("status changed to: {0}", inStatus);
        }

        public void RunShell(string inHostName, AuctionCredencial inCredencial) {
            mConnection = new JabberClient();
            mConnection.User = inCredencial.Id.User;
            mConnection.Server = inCredencial.Id.Server;
            mConnection.Resource = inCredencial.Id.Resource;
            mConnection.Password = inCredencial.Password;
            mConnection.Port = 5222;
            mConnection.NetworkHost = inHostName;
            mConnection.AutoLogin = true;
            mConnection.AutoStartTLS = false;

            Utils.JabberClientHelper.ConnectServer(mConnection);

            var msg = new Message(new XmlDocument()) {
                ID = Guid.NewGuid().ToString(),
                Type = MessageType.chat, 
                To = this.ToJid("item-54321", mConnection), 
            };

            mConnection.Write(msg);

            this.Status = SniperStatus.Joining;
        }

        private JID ToJid(string inItemId, JabberClient inConnection) {
            return new JID(
                string.Format(ItemIdAsLogin, inItemId),
                inConnection.Server,
                inConnection.Resource
            );
        }

        public void Terminate() {
            Utils.JabberClientHelper.DisconnectServer(mConnection);

            this.Status = SniperStatus.Disconnected;
        }

        public SniperStatus Status {get; private set;}
    }

    public struct AuctionCredencial {
        public JID Id {get;set;}
        public string Password {get;set;}
    }
}

