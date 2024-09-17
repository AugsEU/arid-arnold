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

			if(FlagsManager.I.CheckFlag(FlagCategory.kCurses, (UInt32)CurseFlagTypes.kCurseMoney))
			{
				MonoRandom rng = new MonoRandom(mTileMapIndex.X);
				rng.ChugNumber(mTileMapIndex.Y);

				if(rng.PercentChance(60.0f))
				{
					// Disable this tile
					pEnabled = false;
				}
			}

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

		protected override void OnCollect()
		{
			bool moneyBlessing = FlagsManager.I.CheckFlag(FlagCategory.kCurses, (UInt32)CurseFlagTypes.kBlessingMoney);

			if (!mIsGhost && moneyBlessing)
			{
				UInt16 collectableID = CampaignManager.I.GetCurrCoinID();
				CollectableManager.I.IncPermanentCount(collectableID, 1);
			}
			base.OnCollect();
		}
	}
}
