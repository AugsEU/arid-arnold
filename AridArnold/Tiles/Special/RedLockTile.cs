namespace AridArnold
{
	internal class RedLockTile : SquareTile
	{
		public RedLockTile(Vector2 position) : base(position)
		{
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/RedLock");
		}

		public override void Update(GameTime gameTime)
		{
			if(EventManager.I.IsSignaled(EventType.RedKeyUsed))
			{
				mEnabled = false;
			}
			base.Update(gameTime);
		}
	}
}
