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
			base.LoadContent();
		}

		protected override PermanentCollectable GetCollectableType()
		{
			return PermanentCollectable.Coin;
		}

		protected override byte GetImplByte()
		{
			return base.GetImplByte();
		}
	}
}
