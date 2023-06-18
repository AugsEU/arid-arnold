using System;

namespace AridArnold
{
	internal class TimeMachine : Entity
	{
		#region rConstants

		static Vector2 CLOCK_MIDDLE_OFFSET = new Vector2(19.0f, 18.0f);
		const float CLOCK_RADIUS = 8.0f;

		#endregion rConstants





		#region rMembers



		#endregion rMembers




		#region rInit

		/// <summary>
		/// Create time machine at pos(bottom left)
		/// </summary>
		public TimeMachine(Vector2 pos) : base(pos)
		{
			mPosition.Y -= 45;
		}



		/// <summary>
		/// Load content for time machine
		/// </summary>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/TimeMachine");
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Get collider bounds
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mTexture);
		}

		#endregion rUpdate



		#region rDraw

		/// <summary>
		/// Draw the time machine
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			DateTime currentTime = DateTime.Now;

			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.Default);

			// Draw clock hands
			float minuteFraq = currentTime.Minute / 60.0f;
			float minuteAngle = ((0.75f) + minuteFraq) * 2.0f * MathF.PI;
			float hourAngle = ((0.75f) + ((currentTime.Hour + minuteFraq) / 12.0f)) * 2.0f * MathF.PI;

			Vector2 clockCentre = mPosition + CLOCK_MIDDLE_OFFSET;
			Vector2 minuteDelta = new Vector2(CLOCK_RADIUS, 0.0f);
			Vector2 hourDelta = new Vector2(CLOCK_RADIUS, 0.0f);

			minuteDelta = MonoMath.Rotate(minuteDelta, minuteAngle);
			hourDelta = MonoMath.Rotate(hourDelta, hourAngle);

			MonoDraw.DrawLine(info, clockCentre, minuteDelta + clockCentre, new Color(153, 98, 122), 2.0f, DrawLayer.Default);
			MonoDraw.DrawLine(info, clockCentre, hourDelta + clockCentre, new Color(229, 183, 201), 2.0f, DrawLayer.Default);
		}

		#endregion rDraw
	}
}
