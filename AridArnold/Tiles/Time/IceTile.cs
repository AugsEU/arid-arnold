namespace AridArnold
{
	internal class IceTile : SquareTile
	{
		const float RELATIVE_DIST_THRESH = 10.2f;

		Texture2D mIceTexture;
		Texture2D mGhostTexture;

		public IceTile(Vector2 position) : base(position)
		{
		}

		public override void LoadContent()
		{
			mIceTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mountain/IceTile");
			mGhostTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mountain/IceGhost");
			RefreshTexture();
		}

		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			if (TimeZoneManager.I.GetCurrentTimeZone() != 0)
			{
				if (entity is PlatformingEntity)
				{
					PlatformingEntity platformingEntity = (PlatformingEntity)entity;
					if (IsOnIce(platformingEntity))
					{
						platformingEntity.SetIceWalking();
					}
				}

				return base.Collide(entity, gameTime);
			}

			return CollisionResults.None;
		}

		static public bool IsOnIce(PlatformingEntity entity, float feetShift = -2.0f)
		{
			// This function sucks.....
			Vector2[] feetPositions = entity.GetFeetCheckPoints(feetShift);

			bool anyIce = false;
			bool anySolid = false;

			foreach (Vector2 pos in feetPositions)
			{
				Type tileType = TileManager.I.GetTile(pos).GetType();
				if (tileType == typeof(IceTile)) anyIce = true;
				else if (tileType != typeof(AirTile)) anySolid = true;
			}

			return anyIce && !anySolid;
		}

		public override void Update(GameTime gameTime)
		{
			if (EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				RefreshTexture();
			}
			base.Update(gameTime);
		}

		void RefreshTexture()
		{
			mTexture = TimeZoneManager.I.GetCurrentTimeZone() == 0 ? mGhostTexture : mIceTexture;
		}
	}
}
