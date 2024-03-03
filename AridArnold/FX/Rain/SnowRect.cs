

namespace AridArnold
{
	unsafe public struct SnowParticle : IParticle // Use interface bleh
	{
		public Vector2 mPos { get; set; }
		public Vector2 mPrevPos { get; set; }
		public Vector2 mWindMult { get; set; }
		public Texture2D mTexture { get; set; }

	}


	internal class SnowRect : ParticleRect<SnowParticle>
	{
		Texture2D mSmallParticle;
		Texture2D mMediumParticle;
		Texture2D mLargeParticle;

		public SnowRect(Rectangle area, int xNum, int yNum, float mGravity = DEFAULT_GRAVITY) : base(area, xNum, yNum, mGravity)
		{
			mSmallParticle = MonoData.I.MonoGameLoad<Texture2D>("BG/Mountain/SnowParticleSmall");
			mMediumParticle = MonoData.I.MonoGameLoad<Texture2D>("BG/Mountain/SnowParticleBig");
			mLargeParticle = MonoData.I.MonoGameLoad<Texture2D>("BG/Mountain/SnowParticleMassive");

			MonoRandom drawRandom = RandomManager.I.GetDraw();

			for (int d = 0; d < mParticles.Length; d++)
			{
				int snowType = drawRandom.GetIntRange(0, 1000);
				if (snowType < 600)
				{
					mParticles[d].mTexture = mSmallParticle;
					mParticles[d].mWindMult = new Vector2(0.45f, 0.4f);
				}
				else if (snowType < 995)
				{
					mParticles[d].mTexture = mMediumParticle;
					mParticles[d].mWindMult = new Vector2(0.85f, 0.8f);
				}
				else
				{
					mParticles[d].mTexture = mLargeParticle;
					mParticles[d].mWindMult = new Vector2(0.4f, 1.3f);
				}
			}
		}

		public override void Draw(DrawInfo info)
		{
			for(int d = 0; d < mParticles.Length; d++)
			{
				MonoDraw.DrawTextureDepth(info, mParticles[d].mTexture, mParticles[d].mPos, DrawLayer.BackgroundElement);
			}
		}
	}
}
