﻿namespace AridArnold
{
	internal class TimeMachine : Entity
	{
		#region rConstants

		const float CLOCK_RADIUS = 8.0f;

		#endregion rConstants





		#region rMembers

		int mLevelID;
		bool mIsForwards;
		Vector2 mClockOffset;
		Color mHourColor;
		Color mMinuteColor;
		TextInfoBubble mUsePrompt;

		#endregion rMembers




		#region rInit

		/// <summary>
		/// Create time machine at pos(bottom left)
		/// </summary>
		public TimeMachine(Vector2 pos, int levelToLoadID, int isForwards) : base(pos)
		{
			mLevelID = levelToLoadID;
			mIsForwards = isForwards != 0;

			Vector2 offset = Vector2.Zero;

			if (mIsForwards)
			{
				offset.X = 30.0f;
				offset.Y = -40.0f;
			}
			else
			{
				offset.Y = -40.0f;
			}

			mUsePrompt = new TextInfoBubble(pos + offset, "InGame.TimeMachine");
		}



		/// <summary>
		/// Load content for time machine
		/// </summary>
		public override void LoadContent()
		{
			if (mIsForwards)
			{
				// Past
				mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Lab/TimeMachine");
				mClockOffset = new Vector2(19.0f, 18.0f);
				mHourColor = new Color(229, 183, 201);
				mMinuteColor = new Color(153, 98, 122);
				mPosition.Y -= 45;
			}
			else
			{
				// Future
				mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/WW7/TimeMachine");
				mClockOffset = new Vector2(33.0f, 43.0f);
				mHourColor = new Color(84, 117, 192);
				mMinuteColor = new Color(20, 65, 148);
				mPosition.Y -= 46;
			}
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mUsePrompt.Update(gameTime, IsPlayerNear());

			base.Update(gameTime);
		}


		/// <summary>
		/// When player presses enter
		/// </summary>
		protected override void OnPlayerInteract()
		{
			// Load the level directly, maybe we could shake and play an animation?
			if (mIsForwards)
			{
				TimeZoneManager.I.TimeTravel();
			}
			else
			{
				TimeZoneManager.I.AntiTimeTravel();
			}
			CampaignManager.I.QueueLoadSequence(new HubDirectLoader(mLevelID));
			base.OnPlayerInteract();
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

			Vector2 clockCentre = mPosition + mClockOffset;
			Vector2 minuteDelta = new Vector2(CLOCK_RADIUS, 0.0f);
			Vector2 hourDelta = new Vector2(CLOCK_RADIUS, 0.0f);

			minuteDelta = MonoMath.Rotate(minuteDelta, minuteAngle);
			hourDelta = MonoMath.Rotate(hourDelta, hourAngle);

			MonoDraw.DrawLine(info, clockCentre, minuteDelta + clockCentre, mMinuteColor, 2.0f, DrawLayer.Default);
			MonoDraw.DrawLine(info, clockCentre, hourDelta + clockCentre, mHourColor, 2.0f, DrawLayer.Default);

			mUsePrompt.Draw(info);
		}

		#endregion rDraw
	}
}
