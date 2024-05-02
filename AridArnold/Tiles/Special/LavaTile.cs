
namespace AridArnold
{
	class LavaTile : InteractableTile
	{
		Animator mLavaAnimation;

		public LavaTile(Vector2 position) : base(position)
		{
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Hell/Lava1");
			mLavaAnimation = MonoData.I.LoadAnimator("Tiles/Hell/LavaAnim1.max");
			mLavaAnimation.Play();
		}

		public override void Update(GameTime gameTime)
		{
			mLavaAnimation.Update(gameTime);
			base.Update(gameTime);
		}

		public override Texture2D GetTexture()
		{
			return mLavaAnimation.GetCurrentTexture();
		}
	}
}
