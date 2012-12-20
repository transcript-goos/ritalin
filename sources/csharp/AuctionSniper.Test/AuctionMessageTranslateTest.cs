using System;
using System.Xml;

using jabber.protocol.client;

using AuctionSniper.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace AuctionSniper.Test {
    [TestFixture]
    public class _オークションとのメッセージのやり取りを翻訳するところに関するTestSuite {
        private static readonly Chat UnusedChat = null; 


        private IAuctionEventListener mListener;
        private IMessageListener mTranslator;


        [SetUp]
        protected void SetUp() {
            mListener = MockRepository.GenerateMock<IAuctionEventListener>();
            mTranslator = new AuctionMessageTranslator(mListener);
        }

        private Action VoidAction() {
            return () => {};
        }

        [Test]
        public void _Closeメッセージを受け取ったらオークションのCloseを通知する() {
            mListener.Expect(listener => {
                listener.AuctionClosed();
            })
            .Repeat.Once();

            var m = new Message(new XmlDocument()) {
                Body = "SOLVersion: 1.1; Event: CLOSE;",
            };

            mTranslator.ProcessMessage(UnusedChat, m);

            mListener.VerifyAllExpectations();
        }

        [Test]
        public void _価格が変更された場合に新しい価格の通知を受ける() {
            mListener.Expect(listener =>  {
                listener.CurrentPrice(192, 7);
            })
            .Repeat.Times(1);

            var m = new Message(new XmlDocument()) {
                Body = "SOLVersion: 1.1; Event: PRICE; CurrentPrice: 192; Increment: 7; Bidder: Someone else;",
            };

            mTranslator.ProcessMessage(UnusedChat, m);

            mListener.VerifyAllExpectations();
        }
    }
}

