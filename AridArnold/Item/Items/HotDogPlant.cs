namespace AridArnold
{
	class HotDogPlant : OnceItem
	{
		int mNumHotDogs;
		Texture2D[] mPlantTextures;
		Level mCurrLevel;

		public HotDogPlant(int price) : base("Items.HotdogPlantTitle", "Items.HotdogPlantDesc", price)
		{
			mNumHotDogs = 0;

			mPlantTextures = new Texture2D[5];
			mPlantTextures[0] = MonoData.I.MonoGameLoad<Texture2D>("Items/HotDogPlant/PlantPhase0");
			mPlantTextures[1] = MonoData.I.MonoGameLoad<Texture2D>("Items/HotDogPlant/PlantPhase1");
			mPlantTextures[2] = MonoData.I.MonoGameLoad<Texture2D>("Items/HotDogPlant/PlantPhase2");
			mPlantTextures[3] = MonoData.I.MonoGameLoad<Texture2D>("Items/HotDogPlant/PlantPhase3");
			mPlantTextures[4] = MonoData.I.MonoGameLoad<Texture2D>("Items/HotDogPlant/PlantPhase4");

			mTexture = mPlantTextures[0];

			mCurrLevel = CampaignManager.I.GetCurrentLevel();
		}

		public override void Update(GameTime gameTime)
		{
			Level newLevel = CampaignManager.I.GetCurrentLevel();
			if (mNumHotDogs < 4 && !object.ReferenceEquals(newLevel, mCurrLevel) && newLevel is not null)
			{
				mNumHotDogs++;
				mTexture = mPlantTextures[mNumHotDogs];
				mCurrLevel = newLevel;
			}

			base.Update(gameTime);
		}

		protected override void DoEffect(Arnold arnold)
		{
			CampaignManager.I.GainLives(mNumHotDogs);
		}

		public override bool CanUseItem(Arnold arnold)
		{
			bool baseCanUse = base.CanUseItem(arnold);
			bool hasAnyLives = mNumHotDogs > 0;
			bool canHaveLives = CampaignManager.I.GetLives() < CampaignManager.I.GetMaxLives();

			return baseCanUse && hasAnyLives && canHaveLives;
		}

		public override string GetTitle()
		{
			string baseText = base.GetTitle();
			if (mNumHotDogs > 0)
			{
				baseText = string.Format("{0} ({1})", baseText, mNumHotDogs);
			}
			return baseText;
		}

		public override bool RegenerateAfterDeath()
		{
			return false;
		}

		public override bool CanUseInShop()
		{
			return true;
		}
	}
}
