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
            var mockery = new MockRepository();
            
            var status = SniperStatus.Disconnected;

            var listener = mockery.GenerateMockHelper<ISniperListener>();
            listener.Expect(x => {
                x.SniperLost();
            })
            .WhenCalled((m) => status = SniperStatus.Lost)
            .Repeat.Once();

            Func<SniperStatus> reportFunc = () => status;
            listener.Expect(x => {
                return x.Status;
            })
            .Do(reportFunc);

            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(null, listener);
            sniper.AuctionClosed();

            Assert.That(listener.Status, Is.EqualTo(SniperStatus.Lost));

            listener.VerifyAllExpectations();
        }

        [Test]
        public void _新しい金額が提示された場合に総額を行い報告も受ける() {
            var mockery = new MockRepository();

            var price = 1001;
            var inc = 25;

            var auction = mockery.GenerateMockHelper<IAuction>();
            auction.Expect(x => {
                x.Bid(price+inc);
            });

            var status = SniperStatus.Disconnected;

            var listener = mockery.GenerateMockHelper<ISniperListener>();
            listener.Expect(x => {
                x.SniperBidding();
            })
            .WhenCalled((m) => status = SniperStatus.Bidding)
            .Repeat.AtLeastOnce();

            Func<SniperStatus> reportFunc = () => status;
            listener.Expect(x => {
                return x.Status;
            })
            .Do(reportFunc);

            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(auction, listener);
            sniper.CurrentPrice(price, inc, PriceSource.FromOtherBidder);

            Assert.That(listener.Status, Is.EqualTo(SniperStatus.Bidding));

            mockery.VerifyAll();
        }

        [Test]
        public void _Sniperが新しい金額を提示したら一位入札状態として報告を上げる() {
            var mockery = new MockRepository();

            var status = SniperStatus.Disconnected;

            var listener = mockery.GenerateMockHelper<ISniperListener>();
            listener.Expect(x => {
                x.SniperWinning();
            })
            .WhenCalled((m) => status = SniperStatus.Winning)
            .Repeat.AtLeastOnce();

            Func<SniperStatus> reportFunc = () => status;
            listener.Expect(x => {
                return x.Status;
            })
            .Do(reportFunc);

            var auction = mockery.GenerateMockHelper<IAuction>();
            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(auction, listener);
            sniper.CurrentPrice(123, 45, PriceSource.FromSniper);

            Assert.That(listener.Status, Is.EqualTo(SniperStatus.Winning));

            mockery.VerifyAll();
        }

        [Test]
        public void _1位入札でオークションを終えた場合落札状態として報告を上げる() {
            var mockery = new MockRepository();

            var status = SniperStatus.Disconnected;
            
            var listener = mockery.GenerateMockHelper<ISniperListener>();
            listener.Expect(x => {
                x.SniperBidding();
            })
            .WhenCalled((m) => status = SniperStatus.Bidding)
            .Repeat.Never();
            
            listener.Expect(x => {
                x.SniperWinning();
            })
            .WhenCalled((m) => status = SniperStatus.Winning)
            .Repeat.AtLeastOnce();
            
            listener.Expect(x => {
                x.SniperWon();
            })
            .WhenCalled((m) => status = SniperStatus.Won)
            .Repeat.Once();

            listener.Expect(x => {
                x.SniperLost();
            })
            .WhenCalled((m) => status = SniperStatus.Lost)
            .Repeat.Never();

            Func<SniperStatus> reportFunc = () => status;
            listener.Expect(x => {
                return x.Status;
            })
            .Do(reportFunc);

            var auction = mockery.GenerateMockHelper<IAuction>();
            IAuctionEventListener sniper = new AuctionSniper.Core.AuctionSniper(auction, listener);

            sniper.CurrentPrice(123, 45, PriceSource.FromSniper);
            sniper.AuctionClosed();

            Assert.That(listener.Status, Is.EqualTo(SniperStatus.Won));

            mockery.VerifyAll();
        }
    }
}

