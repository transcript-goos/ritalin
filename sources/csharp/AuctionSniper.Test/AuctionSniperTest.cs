using System;

using AuctionSniper.Core;

using NUnit.Framework;
using Rhino.Mocks;

using Test;

namespace AuctionSniper.Test {
    [TestFixture]
    public class _Sniperコア機能に関するTestSuite {
        [Test]
        public void _オークションが閉じたときに落札に失敗したことの報告を受ける() {
            var listener = MockRepository.GenerateMock<ISniperListener>();
            listener.Expect(x => {
                x.SniperLost();
            });

            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(null, listener);
            sniper.AuctionClosed();

            listener.VerifyAllExpectations();
        }

        [Test]
        public void _新しい金額が提示された場合に総額を行い報告も受ける() {
            var mocker = new MockRepository();

            var price = 1001;
            var inc = 25;

            var auction = mocker.GenerateMockHelper<IAuction>();
            auction.Expect(x => {
                x.Bid(price+inc);
            });

            var listener = mocker.GenerateMockHelper<ISniperListener>();
            listener.Expect(x => {
                x.SniperBidding();
            })
            .Repeat.AtLeastOnce();

            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(auction, listener);
            sniper.CurrentPrice(price, inc, PriceSource.FromOtherBidder);

            mocker.VerifyAll();
        }

        [Test]
        public void _Sniperが新しい金額を提示したら一位入札状態として報告を上げる() {
            var mocker = new MockRepository();

            var listener = mocker.GenerateMockHelper<ISniperListener>();
            listener.Expect(x => {
                x.SniperWinning();
            })
            .Repeat.AtLeastOnce();

            var auction = mocker.GenerateMockHelper<IAuction>();
            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(auction, listener);
            sniper.CurrentPrice(123, 45, PriceSource.FromSniper);
            
            mocker.VerifyAll();
        }
    }
}

