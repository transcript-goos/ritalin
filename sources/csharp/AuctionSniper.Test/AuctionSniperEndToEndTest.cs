using System;

using NUnit.Framework;

namespace AuctionSniper.Test {
    [TestFixture]
    public class _オークションが開始されてから終わるまでの間のTestSuite {
        private FakeAuctionServer mAuction = new FakeAuctionServer("item-54321");
        private ApplicationRunner mApp = new ApplicationRunner();

        [TearDown]
        public void StopAuction() {
            mApp.Stop();
            mAuction.Stop();
        } 

        [Test]
        public void _オークションに参加表明したあと何もせず終わるまで待つだけ() {
            // Step 1            
            // オークションに商品が出品されている
            mAuction.StartSellingItem(); 
            // Step 2
            // オークションスナイパー、その商品に入札を始める
            mApp.StartBiddingIn(mAuction);
            // Step 3
            // オークションは、オークションスナイパーからのリクエストを受信する
            mAuction.HasReceivedJoinRequestFrom(mApp.JId);
            // Step 4 
            // オークションは、終了を宣言する
            mAuction.AnnounceClosed();
            // Step 5
            // オークションスナイパーは、悪札に失敗したことを表示する
            mApp.ShowsSniperHasLostAuction();
        }

        [Test]
        public void _一度だけBidするけど結局落札し損ねるの巻() {
            mAuction.StartSellingItem();
            // Step 1
            //  スナイパーからの参加を待つ
            mApp.StartBiddingIn(mAuction);
            mAuction.HasReceivedJoinRequestFrom(mApp.JId);
            // Step 2
            // 現在の価格、次回増額、現在の落札者を通知する
            mAuction.ReportPrice(1000, 98, "other bidder");
            // Step 3
            // 入札中になったかどうかチェックする
            mApp.HasShownSniperInBidding();
            // Step 4
            // スナイパーからの入札を受信したことをチェックする
            mAuction.HasReceivedBid(1098,  mApp.JId);
            // Step 5
            // 落札に失敗したかどうかチェックする
            mAuction.AnnounceClosed();
            mApp.ShowsSniperHasLostAuction();
        }           
    }
}

