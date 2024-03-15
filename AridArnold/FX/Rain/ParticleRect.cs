using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace AridArnold
{
	public interface IParticle // Use interface bleh
	{
		public Vector2 mPos { get; set; }
		public Vector2 mPrevPos { get; set; }
		public Vector2 mWindMult { get; set; }
	}


	/// <summary>
	/// Draws rain in a rectangle
	/// </summary>
	abstract class ParticleRect<TParticle> : FX where TParticle : IParticle
	{
		#region rConstants

		protected const float DEFAULT_GRAVITY = 2.0f;
		protected const float DEFAULT_MAX_WIND_DEV = 5.0f;

		#endregion rConstants





		#region rMembers

		Rectangle mArea;
		Vector2 mWind;
		Vector2 mWindVelocity;
		float mMaxWindDeviation;
		protected TParticle[] mParticles;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create from params
		/// </summary>
		public ParticleRect(Rectangle area, int xNum, int yNum, float gravity = DEFAULT_GRAVITY, float maxWindDeviation = DEFAULT_MAX_WIND_DEV)
		{
			InitRainRect(area, xNum, yNum, gravity, maxWindDeviation);
		}



		/// <summary>
		/// Create from XML node
		/// </summary>
		public ParticleRect(XmlNode node)
		{
			InitRainRect(MonoParse.GetRectangle(node),
				MonoParse.GetInt(node["xNum"]),
				MonoParse.GetInt(node["yNum"]),
				MonoParse.GetFloat(node["gravity"], DEFAULT_GRAVITY),
				MonoParse.GetFloat(node["windDeviation"], DEFAULT_MAX_WIND_DEV));
		}



		/// <summary>
		/// Constructor
		/// </summary>
		void InitRainRect(Rectangle area, int xNum, int yNum, float gravity, float maxWindDeviation)
		{
			mArea = area;

			mWind = new Vector2(0.0f, gravity);
			mWindVelocity = Vector2.Zero;

			// Init raindrops spread evenly
			mParticles = new TParticle[xNum * yNum];

			mMaxWindDeviation = maxWindDeviation;

			MonoRandom rainRandom = RandomManager.I.GetDraw();
			Vector2 topLeftCorner = new Vector2(area.Left, area.Top);
			Vector2 rectSize = new Vector2(area.Width, area.Height);

			for (int y = 0; y < yNum; y++)
			{
				for (int x = 0; x < xNum; x++)
				{
					float xCoord = rainRandom.GetFloatRange(topLeftCorner.X, topLeftCorner.X + rectSize.X);
					float yCoord = rainRandom.GetFloatRange(topLeftCorner.Y, topLeftCorner.Y + rectSize.Y);

					mParticles[y * yNum + x].mPos = new Vector2(xCoord, yCoord);
					mParticles[y * yNum + x].mPrevPos = new Vector2(xCoord, yCoord);
				}
			}

			mWindVelocity.X = rainRandom.GetFloatRange(-5.0f, 5.0f);
			mWind.X = rainRandom.GetFloatRange(-5.0f, 5.0f);
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update rain with base physics
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			// Update wind
			mWindVelocity.X += dt * RandomManager.I.GetDraw().GetFloatRange(-mMaxWindDeviation, mMaxWindDeviation);
			mWindVelocity.X = Math.Clamp(mWindVelocity.X, -mMaxWindDeviation, mMaxWindDeviation);

			mWind += mWindVelocity * dt;
			mWind.X = Math.Clamp(mWind.X, -mMaxWindDeviation, mMaxWindDeviation);

			float downAngle = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera).GetCurrentSpec().mRotation;
			Vector2 windMove = dt * mWind;
			windMove = MonoMath.Rotate(windMove, -downAngle);

			// Do wind and wrap arounds
			for (int d = 0; d < mParticles.Length; d++)
			{
				Vector2 newPos = mParticles[d].mPos;
				mParticles[d].mPrevPos = newPos;
				newPos.X += windMove.X * mParticles[d].mWindMult.X;
				newPos.Y += windMove.Y * mParticles[d].mWindMult.Y;

				// Wrap around the edges of the screen.
				if (newPos.X < mArea.X)
				{
					newPos.X += mArea.Width;
				}
				else if (newPos.X > mArea.X + mArea.Width)
				{
					newPos.X -= mArea.Width;
				}
				else if (newPos.Y < mArea.Y)
				{
					newPos.Y += mArea.Height;
				}
				else if (newPos.Y > mArea.Y + mArea.Height)
				{
					newPos.Y -= mArea.Height;
				}

				mParticles[d].mPos = newPos;
			}
		}



		/// <summary>
		/// Never finished
		/// </summary>
		public override bool Finished()
		{
			return false;
		}

		#endregion rUpdate
	}
}
