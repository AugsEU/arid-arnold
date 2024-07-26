
namespace AridArnold
{
	internal class HubTimeShiftLoader : LoadingSequence
	{
		bool mHasLoaded = false;
		bool mForwards;

		public HubTimeShiftLoader(int levelID, bool forwards) : base(levelID)
		{
			mForwards = forwards;
		}

		public override void Update(GameTime gameTime)
		{
			if (!mHasLoaded)
			{
				mHasLoaded = true;

				if (mForwards)
				{
					TimeZoneManager.I.AgePlayer();
				}
				else
				{
					TimeZoneManager.I.AntiAgePlayer();
				}

				LoadAsHubLevel();

				TimeShiftFaderFX faderFX = new TimeShiftFaderFX();

				// Queue camera move
				Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
				gameCam.QueueMovement(new ShiftTimeCameraMove(mForwards, faderFX));

				FXManager.I.AddFX(faderFX);
			}
		}

		public override void Draw(DrawInfo info)
		{
		}

		public override bool Finished()
		{
			return mHasLoaded;
		}


	}
}
