﻿namespace AridArnold
{
	/// <summary>
	/// Display giblet
	/// </summary>
	internal class GibletFX : FX
	{
		#region rConstants

		static Vector2[] PACK_POSITIONS = { new Vector2(1.0f, 1.0f), new Vector2(1.0f, -1.0f), new Vector2(-1.0f, 1.0f), new Vector2(-1.0f, -1.0f) };
		const float GRAVITY = 7.0f;
		const float SPIN_SPEED = 2.0f;

		#endregion rConstants





		#region rMembers

		Vector2 mPosition;
		Vector2 mSize;
		Vector2 mVelocity;
		float mAngle;

		Color mColor = Color.LightGray;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create water leak fx at point
		/// </summary>
		public GibletFX(Vector2 position, Vector2 size, Vector2 velocity, Color color)
		{
			mPosition = position;
			mVelocity = velocity;
			mColor = color;
			mSize = size;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update spawner
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);
			mVelocity.Y += GRAVITY * dt;
			mPosition += mVelocity * dt;

			mAngle += dt * SPIN_SPEED;
			mAngle = (mAngle % (MathF.PI * 2.0f));
		}



		/// <summary>
		/// We are never finished.
		/// </summary>
		/// <returns></returns>
		public override bool Finished()
		{
			return FXManager.I.OutsideFXRegion(mPosition);
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw nothing
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Rect2f rect = new Rect2f(mPosition, mPosition + mSize);

			MonoDraw.DrawRectRot(info, rect.ToRectangle(), mAngle, mColor, DrawLayer.TileEffects);
		}

		#endregion rDraw





		#region rStatic

		/// <summary>
		/// Spawn 4 giblet pack
		/// </summary>
		public static void SpawnGibletPack(Vector2 centre, float radius, Color col)
		{
			for(int i = 0; i < PACK_POSITIONS.Length; i++)
			{
				Vector2 pos = PACK_POSITIONS[i] * radius + centre;
				GibletFX newGiblet = new GibletFX(pos, new Vector2(4.0f, 4.0f), PACK_POSITIONS[i] * radius, col);
				FXManager.I.AddFX(newGiblet);
			}
		}

		#endregion rStatic
	}
}
