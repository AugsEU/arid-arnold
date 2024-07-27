
namespace AridArnold
{
	internal class FireBarrel : Entity
	{
		Animator mFireAnim;
		PointSmokeEmitter mSmokeEmitter;

		public FireBarrel(Vector2 pos) : base(pos)
		{
			mPosition.Y -= 7.0f;
			Vector2 smokePos = mPosition + new Vector2(5.0f, 7.0f);
			mSmokeEmitter = new PointSmokeEmitter(smokePos);
		}

		public override void LoadContent()
		{
			mFireAnim = MonoData.I.LoadAnimator("FireBarrel/FireBarrel.max");
			mFireAnim.Play();
			mTexture = mFireAnim.GetTexture(0);
		}

		public override void Update(GameTime gameTime)
		{
			mFireAnim.Update(gameTime);
			mSmokeEmitter.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			Texture2D drawTexture = mFireAnim.GetCurrentTexture();
			MonoDraw.DrawTexture(info, drawTexture, mPosition);
		}

	}
}
