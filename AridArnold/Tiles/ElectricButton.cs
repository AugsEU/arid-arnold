namespace AridArnold
{
	/// <summary>
	/// Button that turns on electricity.
	/// </summary>
	internal class ElectricButton : InteractableTile
	{
		#region rInitialisation

		Texture2D mDownTexture;
		bool mIsPressed;
		bool mWasPressed;

		/// <summary>
		/// Construct button at position
		/// </summary>
		public ElectricButton(CardinalDirection rot, Vector2 position) : base(position)
		{
			mRotation = rot;
			mIsPressed = false;
			mWasPressed = false;
		}

		/// <summary>
		/// Load all textures
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/button-up");
			mDownTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/button-down");

			TileManager.I.GetEMField().RegisterConductive(mTileMapIndex);
		}


		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Entity intersect
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			mIsPressed = true;
		}



		/// <summary>
		/// Update button
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if(mIsPressed)
			{
				TileManager.I.GetEMField().SetElectricity(mTileMapIndex, 4.0f);
				mWasPressed = true;
			}
			else
			{
				TileManager.I.GetEMField().SetElectricity(mTileMapIndex, -2.0f);
				mWasPressed = false;
			}

			mIsPressed = false;
			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get button texture.
		/// </summary>
		public override Texture2D GetTexture()
		{
			if(mWasPressed)
			{
				return mDownTexture;
			}

			return mTexture;
		}

		#endregion rDraw
	}




	/// <summary>
	/// Button that turns on electricity and stays down forever.
	/// </summary>
	internal class PermElectricButton : InteractableTile
	{
		#region rInitialisation

		Texture2D mDownTexture;
		bool mIsPressed;
		bool mWasPressed;

		/// <summary>
		/// Construct button at position
		/// </summary>
		public PermElectricButton(CardinalDirection rot, Vector2 position) : base(position)
		{
			mRotation = rot;
			mIsPressed = false;
			mWasPressed = false;
		}

		/// <summary>
		/// Load all textures
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/perma-up");
			mDownTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/perma-down");

			TileManager.I.GetEMField().RegisterConductive(mTileMapIndex);
		}


		#endregion rInitialisation





		#region rUpdate

		/// <summary>
		/// Entity intersect
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			mIsPressed = true;
		}



		/// <summary>
		/// Update button
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			if (mIsPressed)
			{
				TileManager.I.GetEMField().SetElectricity(mTileMapIndex, 4.0f);
				mWasPressed = true;
			}
			else
			{
				TileManager.I.GetEMField().SetElectricity(mTileMapIndex, -1.0f);
				mWasPressed = false;
			}

			base.Update(gameTime);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get button texture.
		/// </summary>
		public override Texture2D GetTexture()
		{
			if (mWasPressed)
			{
				return mDownTexture;
			}

			return mTexture;
		}

		#endregion rDraw
	}
}
