using System;
using System.Xml;

using jabber;
using jabber.client;
using jabber.protocol.client;

namespace AuctionSniper.Console {


    public class AuctionSniperConsole {
        public static readonly string AuctionResource = "Auction";
        public static readonly string ItemIdAsLogin = "auction-{0}";

        public AuctionSniperConsole() {
            this.Connection = new JabberClient();        
            this.Connection.AutoLogin = true;
            this.Connection.AutoStartTLS = false;
        }

        private void ShowStatus(SniperStatus inStatus) {
            System.Console.WriteLine("status changed to: {0}", inStatus);
        }

        public void RunShell(string inHostName, AuctionCredencial inCredencial) {
            this.Connection.User = inCredencial.Id.User;
            this.Connection.Server = inCredencial.Id.Server;
            this.Connection.Resource = inCredencial.Id.Resource;
            this.Connection.Password = inCredencial.Password;
            this.Connection.Port = 5222;
            this.Connection.NetworkHost = inHostName;

            this.Connection.OnMessage += (s, m) => {
                this.Status = SniperStatus.Lost;
            };

            this.Connection.OnAuthenticate += (s) => {
                var msg = new Message(new XmlDocument()) {
                    Type = MessageType.chat, 
                    To = this.ToJid("item-54321", this.Connection), 
                };

                this.Connection.Write(msg);

                this.Status = SniperStatus.Joining;
            };

            this.Connection.Connect();
        }

        private JID ToJid(string inItemId, JabberClient inConnection) {
            return new JID(
                string.Format(ItemIdAsLogin, inItemId),
                inConnection.Server,
                inConnection.Resource
            );
        }

        public void Terminate() {
            if (! this.Connection.IsAuthenticated) return;

            this.Connection.OnDisconnect += (s) => {
                this.Status = SniperStatus.Disconnected;
            };

            this.Connection.Close();
        }

        public JabberClient Connection { get; private set; }
        
        public SniperStatus Status {get; private set;}
    }

    public struct AuctionCredencial {
        public JID Id {get;set;}
        public string Password {get;set;}
    }
}

