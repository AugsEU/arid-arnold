

namespace AridArnold
{
	unsafe public struct EmberParticle : IParticle // Use interface bleh
	{
		public Vector2 mPos { get; set; }
		public Vector2 mPrevPos { get; set; }
		public Vector2 mWindMult { get; set; }
		public Texture2D mTexture { get; set; }

	}


	internal class EmberRect : ParticleRect<SnowParticle>
	{
		Texture2D mSmallParticle;
		Texture2D mMediumParticle;

		public EmberRect(Rectangle area, int xNum, int yNum, float mGravity = -DEFAULT_GRAVITY) : base(area, xNum, yNum, mGravity, 1.2f)
		{
			mSmallParticle = MonoData.I.MonoGameLoad<Texture2D>("BG/Hell/EmberSmall");
			mMediumParticle = MonoData.I.MonoGameLoad<Texture2D>("BG/Hell/EmberMedium");

			MonoRandom drawRandom = RandomManager.I.GetDraw();

			for (int d = 0; d < mParticles.Length; d++)
			{
				int snowType = drawRandom.GetIntRange(0, 1000);
				if (snowType < 905)
				{
					mParticles[d].mTexture = mSmallParticle;
					mParticles[d].mWindMult = new Vector2(1.0f, 1.0f);
				}
				else
				{
					mParticles[d].mTexture = mMediumParticle;
					mParticles[d].mWindMult = new Vector2(0.8f, 0.8f);
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
