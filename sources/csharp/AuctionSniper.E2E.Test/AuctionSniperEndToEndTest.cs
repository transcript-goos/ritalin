using System;

using NUnit.Framework;

namespace AuctionSniper.Test {
    [TestFixture]
    public class _オークションが開始されてから終わるまでの間のTestSuite {
        private FakeAuctionServer mAuction;
        private ApplicationRunner mRunner;

        [SetUp]
        public void SetUp() {
            mAuction = new FakeAuctionServer("item-54321");
            mRunner = new ApplicationRunner();
        }

        [TearDown]
        public void StopAuction() {
            mRunner.Stop();
            mAuction.Stop();
        } 

        [Test]
        public void _オークションに参加表明したあと何もせず終わるまで待つだけ() {
            // Step 1            
            // オークションに商品が出品されている
            mAuction.StartSellingItem(); 
            // Step 2
            // オークションスナイパー、その商品に入札を始める
            mRunner.StartBiddingIn(mAuction);
            // Step 3
            // オークションは、オークションスナイパーからのリクエストを受信する
            mAuction.HasReceivedJoinRequestFrom(mRunner.JId);
            // Step 4 
            // オークションは、終了を宣言する
            mAuction.AnnounceClosed();
            // Step 5
            // オークションスナイパーは、悪札に失敗したことを表示する
            mRunner.ShowsSniperHasLostAuction();
        }

        [Test]
        public void _一度だけBidするけど結局落札し損ねるの巻() {
            mAuction.StartSellingItem();
            // Step 1
            //  スナイパーからの参加を待つ
            mRunner.StartBiddingIn(mAuction);
            mAuction.HasReceivedJoinRequestFrom(mRunner.JId);
            // Step 2
            // 現在の価格、次回増額、現在の落札者を通知する
            mAuction.ReportPrice(1000, 98, "other bidder");
            // Step 3
            // 入札中になったかどうかチェックする
            mRunner.HasShownSniperInBidding();
            // Step 4
            // スナイパーからの入札を受信したことをチェックする
            mAuction.HasReceivedBid(1098,  mRunner.JId);
            // Step 5
            // 落札に失敗したかどうかチェックする
            mAuction.AnnounceClosed();
            mRunner.ShowsSniperHasLostAuction();
        }   

        [Test]
        public void _一位入札中これは勝つる() {
            mAuction.StartSellingItem();

            mRunner.StartBiddingIn(mAuction);
            mAuction.HasReceivedJoinRequestFrom(mRunner.JId);
            mAuction.ReportPrice(1000, 98, "other bidder");

            mRunner.HasShownSniperInBidding();
            mAuction.HasReceivedBid(1098,  mRunner.JId);

            // Step 1
            // Sniperから価格の上乗せが通知された
            mAuction.ReportPrice(1098, 97, mRunner.JId);
            // Step 2
            // ただいま一位入札中でござる
            mRunner.HasShownSniperInWinning();
            // Step 3
            // 落札できたかどうかチェックする
            mAuction.AnnounceClosed();
            mRunner.ShowsSniperHasWonAuction();
        }
    }
}

