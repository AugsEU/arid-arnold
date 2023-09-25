using Microsoft.Xna.Framework;

namespace AridArnold
{
	internal class IceTile : SquareTile
	{
		const float RELATIVE_DIST_THRESH = 10.2f;

		Texture2D mIceTexture;
		Texture2D mGhostTexture;

		public IceTile(Vector2 position) : base(position)
		{
			EventManager.I.AddListener(EventType.TimeChanged, OnTimeChange);
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

		bool IsOnIce(PlatformingEntity entity)
		{
			// This function sucks.....
			Vector2[] feetPositions = entity.GetFeetCheckPoints();

			foreach(Vector2 pos in feetPositions)
			{
				if (TileManager.I.GetTile(pos).GetType() != typeof(IceTile))
					return false;
			}

			return true;
		}

		public void OnTimeChange(EArgs eArgs)
		{
			RefreshTexture();
		}

		void RefreshTexture()
		{
			mTexture = TimeZoneManager.I.GetCurrentTimeZone() == 0 ? mGhostTexture : mIceTexture;
		}
	}
}
