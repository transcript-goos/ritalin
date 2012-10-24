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
            mAuction.HasReceivedJoinRequestFromSniper();
            // Step 4 
            // オークションは、終了を宣言する
            mAuction.AnnounceClosed();
            // Step 5
            // オークションスナイパーは、悪札に失敗したことを表示する
            mApp.ShowsSniperHasLostAuction();
        }

        [Test]
        public void _一度だけBidするけど結局落札し損ねるの巻() {

        } 
    }
}

