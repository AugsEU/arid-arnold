namespace AridArnold
{
	internal class FallingFarry : PlatformingEntity
	{
		Animator mFallingAnim;

		public FallingFarry(Vector2 pos) : base(pos, 0.0f, 0.0f, 0.3f)
		{
		}

		public override void LoadContent()
		{
			const float FT = 0.1f;
			mFallingAnim = new Animator(Animator.PlayType.Repeat,
											("NPC/FallinFarry/Farry1", FT),
											("NPC/FallinFarry/Farry2", FT),
											("NPC/FallinFarry/Farry3", FT),
											("NPC/FallinFarry/Farry4", FT));
			mFallingAnim.Play();

			mTexture = MonoData.I.MonoGameLoad<Texture2D>("NPC/FallinFarry/Farry1");
		}

		public override void Update(GameTime gameTime)
		{
			mFallingAnim.Update(gameTime);
			base.Update(gameTime);
		}

		protected override Texture2D GetDrawTexture()
		{
			return mFallingAnim.GetCurrentTexture();
		}
	}
}
