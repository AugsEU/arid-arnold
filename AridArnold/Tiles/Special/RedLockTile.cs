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
			if (EventManager.I.IsSignaled(EventType.RedKeyUsed))
			{
				mEnabled = false;
				FXManager.I.AddAnimator(mPosition, "Shared/Coin/Explode.max", DrawLayer.TileEffects);
				SFXManager.I.PlaySFX(AridArnoldSFX.Unlock, 0.6f);
			}
			base.Update(gameTime);
		}
	}
}
