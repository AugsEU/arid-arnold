

namespace AridArnold
{
	unsafe public struct LeafParticle : IParticle // Use interface bleh
	{
		public Vector2 mPos { get; set; }
		public Vector2 mPrevPos { get; set; }
		public Vector2 mWindMult { get; set; }

		public Texture2D mTexture { get; set; }

	}


	internal class LeafRect : ParticleRect<LeafParticle>
	{
		Texture2D mLeafTex;
		Texture2D mFluffTex;

		public LeafRect(Rectangle area, int xNum, int yNum, float mGravity = DEFAULT_GRAVITY) : base(area, xNum, yNum, mGravity, 10.0f)
		{
			mLeafTex = MonoData.I.MonoGameLoad<Texture2D>("BG/Mirror/Leaf");
			mFluffTex = MonoData.I.MonoGameLoad<Texture2D>("BG/Mirror/Fluff");

			MonoRandom drawRandom = RandomManager.I.GetDraw();

			for (int d = 0; d < mParticles.Length; d++)
			{
				int snowType = drawRandom.GetIntRange(0, 1000);
				if (snowType < 705)
				{
					mParticles[d].mTexture = mLeafTex;
					mParticles[d].mWindMult = new Vector2(1.15f, 1.2f);
				}
				else if (snowType < 805)
				{
					mParticles[d].mTexture = mLeafTex;
					mParticles[d].mWindMult = new Vector2(0.9f, 1.0f);
				}
				else
				{
					mParticles[d].mTexture = mFluffTex;
					mParticles[d].mWindMult = new Vector2(1.0f, 1.0f);
				}
			}
		}

		public override void Draw(DrawInfo info)
		{
			Rectangle normalRect = new Rectangle(0, 0, 4, 4);
			Rectangle turnRect = new Rectangle(4, 0, 4, 4);

			for (int d = 0; d < mParticles.Length; d++)
			{
				ref LeafParticle particle = ref mParticles[d];
				float deltaX = particle.mPos.X - particle.mPrevPos.X;
				SpriteEffects effect = deltaX < 0.0f ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				MonoDraw.DrawTexture(info, particle.mTexture, particle.mPos, MathF.Abs(deltaX) > 0.4f ? normalRect : turnRect, Color.White, 0.0f, Vector2.Zero, 1.0f, effect, DrawLayer.BackgroundElement);
			}
		}
	}
}
