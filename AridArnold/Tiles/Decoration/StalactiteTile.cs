using AridArnold.Tiles.Basic;

namespace AridArnold
{
    class StalactiteTile : AirTile
	{
		static Vector2 DROP_DISPLACEMENT = new Vector2(10.0f, 13.0f);
		static Color DROP_MAIN_COLOR = new Color(0, 46, 101);
		static Color DROP_SECOND_COLOR = new Color(0, 33, 75);

		MonoTimer mTimer;
		float mDropTime;
		float mDropDistance;

		/// <summary>
		/// Tile with start position
		/// </summary>
		/// <param name="position">Start position</param>
		public StalactiteTile(Vector2 position) : base(position)
		{
			mTimer = new MonoTimer();
			mTimer.Start();

			mDropTime = RandomManager.I.GetDraw().GetFloatRange(0.0f, 5500.0f);
		}

		public override void FinishInit()
		{
			mDropDistance = CalculateDropDistance();
			base.FinishInit();
		}

		float CalculateDropDistance()
		{
			float start = sTILE_SIZE - DROP_DISPLACEMENT.Y;

			Point tilePt = mTileMapIndex;
			tilePt.Y += 1;

			while(!TileManager.I.IsTileSolid(tilePt) && tilePt.Y < 50)
			{
				tilePt.Y += 1;
			}

			int numTiles = tilePt.Y - mTileMapIndex.Y - 1;

			if (numTiles == 0)
			{
				return 0.0f;
			}

			return start + (tilePt.Y - mTileMapIndex.Y - 1) * 16.0f;
		}


		public override void Update(GameTime gameTime)
		{
			if((float)mTimer.GetElapsedMs() > mDropTime)
			{
				AddDewDrop();
			}

			base.Update(gameTime);
		}

		void SetDropTime()
		{
			mDropTime = RandomManager.I.GetDraw().GetFloatRange(3500.0f, 10500.0f);
		}


		void AddDewDrop()
		{
			if(mDropDistance == 0.0f)
			{
				return;
			}

			FXManager.I.AddDrop(mPosition + DROP_DISPLACEMENT, mDropDistance, DROP_MAIN_COLOR, DROP_SECOND_COLOR);

			mTimer.Reset();
			SetDropTime();
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Buk/StalagTile");
		}
	}
}
