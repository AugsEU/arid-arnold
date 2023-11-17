using AridArnold.Tiles.Basic;

namespace AridArnold
{
    class StalactiteTile : AirTile
	{
		static Vector2 DROP_DISPLACEMENT = new Vector2(10.0f, 11.0f);
		static Color DROP_MAIN_COLOR = new Color(0, 46, 101);
		static Color DROP_SECOND_COLOR = new Color(0, 33, 75);

		public StalactiteTile(Vector2 position) : base(position)
		{
		}

		public override void FinishInit()
		{
			float dropDistance = CalculateDropDistance();

			FXManager.I.AddFX(new WaterLeakFX(mPosition + DROP_DISPLACEMENT, dropDistance, DROP_MAIN_COLOR, DROP_SECOND_COLOR));

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


		/// <summary>
		/// Load all textures and assets
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Buk/StalagTile");
		}
	}
}
