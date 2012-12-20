using System;

using AuctionSniper.Core;

using NUnit.Framework;
using Rhino.Mocks;

namespace AuctionSniper.Test {
    [TestFixture]
    public class _Sniperコア機能に関するTestSuite {
        [Test]
        public void _オークションが閉じたときに落札に失敗したことの報告を受ける() {
            var listener = MockRepository.GenerateMock<ISniperListener>();
            listener.Expect(x => {
                x.SniperLost();
            });

            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(listener);
            sniper.AuctionClosed();

            listener.VerifyAllExpectations();
        }
    }
}

