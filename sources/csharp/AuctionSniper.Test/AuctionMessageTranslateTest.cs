using System;
using System.Xml;

using jabber.protocol.client;

using AuctionSniper.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace AuctionSniper.Test {
    [TestFixture()]
    public class _オークションとのメッセージのやり取りを翻訳するところに関するTestSuite {
        private static readonly Chat UnusedChat = null; 


        private readonly AuctionEventListener mListener = MockRepository.GenerateMock<AuctionEventListener>();
        private AuctionMessageTranslator mTranslator;


        [SetUp]
        protected void SetUp() {
            mTranslator = new AuctionMessageTranslator(mListener);
        }

        [Test]
        public void _Closeメッセージを受け取ったらオークションのCloseを通知する() {
            mListener.Expect(listener => listener.AuctionClosed());

            var m = new Message(new XmlDocument()) {
                Body = "SOLVersion: 1.1; Event: Close;",
            };

            mTranslator.ProcessMessage(UnusedChat, m);

            mListener.VerifyAllExpectations();
        }
    }
}

