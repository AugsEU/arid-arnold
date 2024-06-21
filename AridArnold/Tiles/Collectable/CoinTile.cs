namespace AridArnold
{
	class CoinTile : PermCollectSimpleTile
	{
		public CoinTile(Vector2 pos) : base(pos)
		{
		}

		public override void LoadContent()
		{
			mFullAnim = MonoData.I.LoadAnimator("CoinFull");
			mGhostAnim = MonoData.I.LoadAnimator("CoinGhost");

			mExitAnim = MonoData.I.LoadAnimator("Shared/Coin/Explode.max");

			base.LoadContent();
		}

		protected override CollectableCategory GetCollectableType()
		{
			return CollectableCategory.Coin;
		}

		protected override byte GetImplByte()
		{
			return CampaignManager.I.GetCurrCoinImpl();
		}
	}
}
