

namespace AridArnold
{
	/// <summary>
	/// Item that plants a tree
	/// </summary>
	class TreeSeed : Item
	{
		const float SEED_PLANT_TIME = 300.0f;

		Point mStartPoint;
		CardinalDirection mGrowDir;
		PercentageTimer mTimer;
		bool mPlaceMid = true;

		public TreeSeed(int price) : base("Items.TreeSeedTitle", "Items.TreeSeedDesc", price)
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Items/Tree/Seed");
			mTimer = new PercentageTimer(SEED_PLANT_TIME);
		}

		public override void Begin()
		{
			mPlaceMid = true;
			mTimer.FullReset();
			base.Begin();
		}

		public override void ActOnArnold(GameTime gameTime, Arnold arnold)
		{
			mTimer.Update(gameTime);

			// First update, grab params
			if(!mTimer.IsPlaying())
			{
				Vector2 arnCen = arnold.GetCentrePos();
				mStartPoint = TileManager.I.GetTileMapCoord(arnCen);
				mGrowDir = Util.InvertDirection(arnold.GetGravityDir());

				// Place base tile
				PlacePlatformTile(mStartPoint, "Items/Tree/TreeBase");
				mTimer.Start();

				mPlaceMid = true;
			}

			// Time to place a new tree or end task
			if(mTimer.GetPercentageF() >= 1.0f)
			{
				mStartPoint += Util.GetNormalPoint(mGrowDir);
				if(IsValidToPlace(mStartPoint))
				{
					if(mPlaceMid)
					{
						PlaceMidTile(mStartPoint, "Items/Tree/TreeMid");
					}
					else
					{
						PlacePlatformTile(mStartPoint, "Items/Tree/TreeExt");
					}

					mPlaceMid = !mPlaceMid;

					mTimer.Reset();
				}
				else
				{
					EndItem();
				}
			}

			base.ActOnArnold(gameTime, arnold);
		}

		public override bool CanUseItem(Arnold arnold)
		{
			Vector2 arnCen = arnold.GetCentrePos();
			mStartPoint = TileManager.I.GetTileMapCoord(arnCen);

			if(!IsValidToPlace(mStartPoint))
			{
				return false;
			}

			return base.CanUseItem(arnold);
		}

		private void PlacePlatformTile(Point pos, string texture)
		{
			Vector2 tilePos = TileManager.I.GetTileTopLeft(pos);
			AnimatedPlatformTile baseTile = new AnimatedPlatformTile(mGrowDir, tilePos, texture);
			PlaceTile(baseTile);
		}

		private void PlaceMidTile(Point pos, string texture)
		{
			Vector2 tilePos = TileManager.I.GetTileTopLeft(pos);
			AirTile baseTile = new DecorTile(tilePos, mGrowDir, texture);
			PlaceTile(baseTile);
		}

		private void PlaceTile(Tile tile)
		{
			Rect2f newBounds = tile.GetBounds();

			DustUtil.EmitInSquare(newBounds, 10, 20, DustUtil.MIRROR_COLORS);

			SFXManager.I.PlaySFX(new SpacialSFX(AridArnoldSFX.TreePlace, newBounds.Centre, 0.8f));

			TileManager.I.PlaceTile(tile);
		}

		private bool IsValidToPlace(Point p)
		{
			if(!TileManager.I.IsInTileMap(p))
			{
				return false;
			}

			Tile tileAtPoint = TileManager.I.GetTile(mStartPoint);
			if (tileAtPoint is not AirTile)
			{
				return false;
			}

			return true;
		}
	}
}
