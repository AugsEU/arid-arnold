
namespace AridArnold
{
	internal class FireBarrel : Entity
	{
		Animator mFireAnim;

		public FireBarrel(Vector2 pos) : base(pos)
		{
			mPosition.Y -= 7.0f;
		}

		public override void LoadContent()
		{
			const float FT = 0.1f;
			mFireAnim = new Animator(Animator.PlayType.Repeat,
									("FireBarrel/Barrel1", FT),
									("FireBarrel/Barrel2", FT),
									("FireBarrel/Barrel3", FT),
									("FireBarrel/Barrel4", FT));
			mFireAnim.Play();
			mTexture = mFireAnim.GetTexture(0);
		}

		public override void Update(GameTime gameTime)
		{
			mFireAnim.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			Texture2D drawTexture = mFireAnim.GetCurrentTexture();
			MonoDraw.DrawTexture(info, drawTexture, mPosition);
		}

	}
}
