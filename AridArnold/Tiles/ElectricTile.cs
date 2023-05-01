namespace AridArnold
{
	class ElectricTile : SquareTile
	{
		#region rMembers

		Animator mFullAnimation;
		Texture2D mMidTexture;
		float mCurrentElectricity;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Construct electric tile.
		/// </summary>
		public ElectricTile(Vector2 position) : base(position)
		{
			mCurrentElectricity = 0.0f;
		}



		/// <summary>
		/// Load electric textures.
		/// </summary>
		public override void LoadContent()
		{
			mFullAnimation = new Animator(Animator.PlayType.Repeat,
									("Tiles/Lab/ElectricFull1", 0.154f),
									("Tiles/Lab/ElectricFull2", 0.154f));
			mFullAnimation.Play();

			mMidTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/ElectricMid");
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/ElectricNone");

			TileManager.I.GetEMField().RegisterConductive(mTileMapIndex);
		}

		#endregion rInitialisation





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			mFullAnimation.Update(gameTime);

			//Electricity
			EMField emField = TileManager.I.GetEMField();
			mCurrentElectricity = emField.GetValue(mTileMapIndex).mElectric;

			EMField.ScanResults scan = emField.ScanAdjacent(mTileMapIndex);

			if (scan.mTotalConductive > 0)
			{
				scan.mTotalElectric /= scan.mTotalConductive;
				emField.SetElectricity(mTileMapIndex, scan.mTotalElectric);
			}

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		public override Texture2D GetTexture()
		{
			if(mCurrentElectricity > 0.75f)
			{
				return mFullAnimation.GetCurrentTexture();
			}
			else if(mCurrentElectricity > 0.25f)
			{
				return mMidTexture;
			}

			return mTexture;
		}

		#endregion rDraw
	}
}
