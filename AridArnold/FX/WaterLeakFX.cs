namespace AridArnold
{
	/// <summary>
	/// Spawns water drops continuously
	/// </summary>
	internal class WaterLeakFX : FX
	{
		#region rMembers

		Vector2 mPosition;
		MonoTimer mTimer;
		float mDropTime;
		float mDropDistance;
		Color mDropMainColor;
		Color mDropOtherColor;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create water leak fx at point
		/// </summary>
		public WaterLeakFX(Vector2 position, float dropDistance, Color dropMainColor, Color dropOtherColor)
		{
			mPosition = position;
			mDropDistance = dropDistance;
			mDropMainColor = dropMainColor;
			mDropOtherColor = dropOtherColor;

			mDropTime = RandomManager.I.GetDraw().GetFloatRange(0.0f, 5500.0f);
			mTimer = new MonoTimer();
			mTimer.Start();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update spawner
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mTimer.Update(gameTime);

			if ((float)mTimer.GetElapsedMs() > mDropTime)
			{
				AddDewDrop();
			}
		}



		/// <summary>
		/// Spawn a new drop
		/// </summary>
		void AddDewDrop()
		{
			if (mDropDistance == 0.0f)
			{
				return;
			}

			FXManager.I.AddDrop(mPosition, mDropDistance, mDropMainColor, mDropOtherColor);

			mTimer.Reset();
			SetDropTime();
		}



		/// <summary>
		/// Decide when the next drop will spawn
		/// </summary>
		void SetDropTime()
		{
			mDropTime = RandomManager.I.GetDraw().GetFloatRange(3500.0f, 10500.0f);
		}



		/// <summary>
		/// We are never finished.
		/// </summary>
		/// <returns></returns>
		public override bool Finished()
		{
			return false;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw nothing
		/// </summary>
		public override void Draw(DrawInfo info)
		{
		}

		#endregion rDraw
	}
}
