namespace AridArnold
{
	internal class StopWatchTile : InteractableTile
	{
		const float TIME_SPEED = 0.2f;
		static Color HAND_COLOR = new Color(11, 9, 10);

		float mHandTime;
		int mTimeZone;
		Texture2D mEnabledTexture;
		Texture2D mGhostTexture;

		public StopWatchTile(Vector2 position, int timeZone) : base(position)
		{
			mHandTime = RandomManager.I.GetWorld().GetFloatRange(0.0f, 12.0f);
			mTimeZone = timeZone;
		}

		public override void LoadContent()
		{
			mEnabledTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mountain/StopWatch");
			mGhostTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mountain/StopWatchGhost");
			RefreshTexture();
		}


		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mHandTime += TimeZoneManager.I.GetCurrentPlayerAge() == 0 ? dt * TIME_SPEED : -dt * TIME_SPEED;
			mHandTime = mHandTime % 12.0f;

			if (EventManager.I.IsSignaled(EventType.TimeChanged))
			{
				RefreshTexture();
			}

			base.Update(gameTime);
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (!IsInteractable())
			{
				return;
			}

			if (MonoAlg.TestFlag(entity.GetInteractionLayer(), InteractionLayer.kPlayer))
			{
				Arnold arnold = (Arnold)entity;
				arnold.SetPrevWalkDirFromVelocity();
				arnold.SetWalkDirection(WalkDirection.None);
				arnold.SetVelocity(Vector2.Zero);

				ChangeTime();
			}

			base.OnEntityIntersect(entity);
		}

		void ChangeTime()
		{
			// Going forwards or backwards
			bool forwards = TimeZoneManager.I.GetCurrentPlayerAge() == 0;
			int fromTime = TimeZoneManager.I.GetCurrentTimeZone();
			int toTime = TimeZoneManager.I.GetCurrentTimeZone() + (forwards ? 1 : -1);

			TimeZoneOverride? timeZoneOverride = CampaignManager.I.GetTimeOverride(fromTime, toTime);

			if (timeZoneOverride.HasValue)
			{
				HubTimeShiftLoader loader = new HubTimeShiftLoader(timeZoneOverride.Value.mDestinationLevel, forwards);
				CampaignManager.I.QueueLoadSequence(loader);
				//EventManager.I.SignalEndUpdateImmediate();
			}
			else
			{
				SimpleTimeChange(forwards);
			}
		}

		void SimpleTimeChange(bool forwards)
		{
			// Queue camera move
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			gameCam.QueueMovement(new ShiftTimeCameraMove(forwards));

			if (forwards)
			{
				TimeZoneManager.I.AgePlayer();
			}
			else
			{
				TimeZoneManager.I.AntiAgePlayer();
			}

			FXManager.I.AddFX(new TimeShiftFaderFX());

			RefreshTexture();
		}

		void RefreshTexture()
		{
			if (IsInteractable())
			{
				mTexture = mEnabledTexture;
			}
			else
			{
				mTexture = mGhostTexture;
			}
		}

		bool IsInteractable()
		{
			int currentTime = TimeZoneManager.I.GetCurrentTimeZone();
			return mTimeZone == currentTime || currentTime == -1;
		}

		public override void DrawExtra(DrawInfo info)
		{
			if (!IsInteractable())
			{
				return;
			}

			Vector2 centre = GetCentre() - new Vector2(0.5f, 0.5f);
			Vector2 hourHand = new Vector2(3.0f, 0.0f);
			Vector2 minuteHand = new Vector2(5.0f, 0.0f);

			float minutes = (mHandTime % 1.0f) * 2.0f * MathF.PI - MathF.PI * 0.5f;
			float hours = (mHandTime / 12.0f) * 2.0f * MathF.PI - MathF.PI * 0.5f;

			hourHand = MonoMath.Rotate(hourHand, hours);
			minuteHand = MonoMath.Rotate(minuteHand, minutes);

			MonoDraw.DrawLine(info, centre, centre + hourHand, HAND_COLOR, 2.0f, DrawLayer.Tile);
			MonoDraw.DrawLine(info, centre, centre + minuteHand, HAND_COLOR, 2.0f, DrawLayer.Tile);
		}
	}
}
