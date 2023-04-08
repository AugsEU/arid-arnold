using Microsoft.Xna.Framework;

namespace AridArnold
{
	/// <summary>
	/// BG for mirror world. A bit bodge but it's contained so who cares?
	/// </summary>
	internal class MirrorBG : LayElement
	{
		const float WIND_DECREASE = 0.02f;
		const float GUST_CHANCE = 15.0f;
		const float GUST_MAX_STRENGTH = 5.0f;
		const float LEAF_FREQ = 2.0f;

		Texture2D mLeafLateralTex;
		Texture2D mLeafVerticalTex;

		List<FallingLeaf> mFallingLeaves;
		float mWindDirection;

		public MirrorBG(XmlNode rootNode) : base(rootNode)
		{
			mLeafLateralTex = MonoData.I.MonoGameLoad<Texture2D>("BG/Mirror/LeafLat");
			mLeafVerticalTex = MonoData.I.MonoGameLoad<Texture2D>("BG/Mirror/LeafVert");
			mFallingLeaves = new List<FallingLeaf>();

			FillScreenWithLeaves();
		}

		void FillScreenWithLeaves()
		{
			float height = TileManager.I.GetDrawHeight();
			float inc = 5.0f / LEAF_FREQ; // Magic formula

			for (float y = 0.0f; y < height; y += inc)
			{
				AddLeaf(y);
			}
		}

		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			foreach(FallingLeaf leaf in mFallingLeaves)
			{
				leaf.Update(gameTime, mWindDirection);
			}

			if (MathF.Abs(mWindDirection) > 0.1f)
			{
				mWindDirection -= dt * WIND_DECREASE * MathF.Sign(mWindDirection);
			}
			else
			{
				bool isGust = RandomManager.I.GetDraw().PercentChance(GUST_CHANCE);

				if (isGust)
				{
					mWindDirection = RandomManager.I.GetDraw().GetFloatRange(-GUST_MAX_STRENGTH, GUST_MAX_STRENGTH);
				}
			}

			bool addLeaf = RandomManager.I.GetDraw().PercentChance(1.2f);

			if(addLeaf)
			{
				AddLeaf();
			}

			float height = TileManager.I.GetDrawHeight() + 20.0f;
			for(int i = 0; i < mFallingLeaves.Count; i++)
			{
				if (mFallingLeaves[i].mPos.Y > height)
				{
					mFallingLeaves.RemoveAt(i--);
				}
			}

			base.Update(gameTime);
		}

		void AddLeaf(float y = -2.0f)
		{
			float width = TileManager.I.GetDrawWidth();
			float x = RandomManager.I.GetDraw().GetFloatRange(-width, 2.0f * width);
			Vector2 pos = new Vector2(x, y);
			float vel = RandomManager.I.GetDraw().GetFloatRange(1.0f, 2.2f);
			mFallingLeaves.Add(new FallingLeaf(pos, vel, mLeafLateralTex, mLeafVerticalTex, mDepth));
		}

		public override void Draw(DrawInfo info)
		{
			foreach (FallingLeaf leaf in mFallingLeaves)
			{
				leaf.Draw(info);
			}

			base.Draw(info);
		}
	}

	class FallingLeaf
	{
		public Vector2 mPos;
		Vector2 mVelocity;
		Texture2D mLateralTexture;
		Texture2D mVerticalTexture;
		float mDepth;

		// Movement
		float mAngle;
		float mAmplitude;
		float mAngularVelocity;

		public FallingLeaf(Vector2 pos, float velocity, Texture2D lateral, Texture2D vertical, float depth)
		{
			mPos = pos;
			mLateralTexture = lateral;
			mVerticalTexture = vertical;

			mVelocity = new Vector2(0.0f, velocity);
			mDepth = depth;

			mAngle = RandomManager.I.GetDraw().GetFloatRange(0.0f, 2.0f * MathF.PI);
			DecideAngles();
		}


		public void Update(GameTime gameTime, float windDirection)
		{
			float dt = Util.GetDeltaT(gameTime);

			float prevPIMult = MathF.Floor(mAngle / MathF.PI);
			mAngle += dt * mAngularVelocity;
			float currPIMult = MathF.Floor(mAngle / MathF.PI);

			if (prevPIMult < currPIMult)
			{
				DecideAngles();
			}

			mVelocity.X = windDirection + MathF.Sin(mAngle) * mAmplitude;

			mPos += mVelocity * dt;
		}

		void DecideAngles()
		{
			mAmplitude = RandomManager.I.GetDraw().GetFloatRange(5.0f, 15.0f);
			mAngularVelocity = RandomManager.I.GetDraw().GetFloatRange(0.2f, 0.3f);
		}

		public void Draw(DrawInfo info)
		{
			Texture2D toDraw = mLateralTexture;
			SpriteEffects effects = SpriteEffects.None;

			if(mVelocity.X > 4.0f)
			{
				effects = SpriteEffects.FlipHorizontally;
			}
			else if(mVelocity.X < -4.0f)
			{
			}
			else
			{
				toDraw = mVerticalTexture;
			}

			MonoDraw.DrawTexture(info, toDraw, mPos, null, Color.White, 0.0f, Vector2.Zero, 1.0f, effects, mDepth);
		}
	}
}
