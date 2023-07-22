using AridArnold.Tiles.Basic;

namespace AridArnold
{
	internal class StopWatchTile : InteractableTile
	{
		const float TIME_SPEED = 0.2f;
		static Color HAND_COLOR = new Color(11, 9, 10);

		bool mDebugEnabled = true;
		float mHandTime;

		public StopWatchTile(Vector2 position) : base(position)
		{
			mHandTime = RandomManager.I.GetWorld().GetFloatRange(0.0f, 12.0f);
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mountain/StopWatch");
		}


		public override void Update(GameTime gameTime)
		{
			float dt = Util.GetDeltaT(gameTime);

			mHandTime += dt * TIME_SPEED;
			mHandTime = mHandTime % 12.0f;

			base.Update(gameTime);
		}

		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold && mDebugEnabled)
			{
				Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);

				bool forwards = TimeZoneManager.I.GetCurrentPlayerAge() == 0;

				gameCam.QueueMovement(new ShiftTimeCameraMove(forwards));
				mDebugEnabled = false;
			}

			base.OnEntityIntersect(entity);
		}

		public override void DrawExtra(DrawInfo info)
		{
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
