namespace AridArnold
{
	internal class IceTile : SquareTile
	{
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
				return base.Collide(entity, gameTime);
			}

			return CollisionResults.None;
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
