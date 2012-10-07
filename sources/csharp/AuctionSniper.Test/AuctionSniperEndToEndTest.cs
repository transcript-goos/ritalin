using System;

using NUnit.Framework;

namespace AuctionSniper.Test {
    [TestFixture]
    public class AuctionSniperEndToEndTest {
        private FakeAuctionServer mAuction = new FakeAuctionServer("item-54321");
        private ApplicationRunner mApp = new ApplicationRunner();

        [TearDown]
        public void StopAuction() {
            mAuction.Stop();
        } 

        [TearDown]
        public void StopApplication() {
            mApp.Stop();
        }

        [Test]
        public void _オークションが開始されてから終わるまでの間のテスト() {
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
    }
}

