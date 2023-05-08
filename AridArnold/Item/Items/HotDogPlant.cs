namespace AridArnold
{
	class HotDogPlant : Item
	{
		int mNumHotDogs;
		Texture2D[] mPlantTextures;
		Level mCurrLevel;

		public HotDogPlant()
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
			if(mNumHotDogs < 4 && !object.ReferenceEquals(newLevel, mCurrLevel) && newLevel is not null)
			{
				mNumHotDogs++;
				mTexture = mPlantTextures[mNumHotDogs];
				mCurrLevel = newLevel;
			}

			base.Update(gameTime);
		}

		public override int GetPrice()
		{
			return 5;
		}

		public override void UseItem()
		{
			CampaignManager.I.GainLives(mNumHotDogs);
		}

		public override bool CanUseItem()
		{
			return mNumHotDogs > 0;
		}
	}
}
