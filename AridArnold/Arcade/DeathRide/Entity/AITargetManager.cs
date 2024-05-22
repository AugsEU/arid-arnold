namespace GMTK2023
{
	internal class AITargetManager : Singleton<AITargetManager>
	{
		const int MAX_TRIES = 100;
		static Rectangle TARGET_AREA = new Rectangle(50, 160, 850, 290);

		List<Vector2> mCurrentTargets;

		public void Init()
		{
			mCurrentTargets = new List<Vector2>();
		}


		public Vector2 GiveMeATarget()
		{
			float playableArea = (GameScreen.PLAYABLE_AREA.Width * GameScreen.PLAYABLE_AREA.Height);
			float tooNearRadius = MathF.Sqrt(playableArea / ((mCurrentTargets.Count + 1) * (MathF.PI)));

			// Try and find a point that isn't too close to the other points
			Vector2 point = GetRandomPoint();
			for (int i = 0; i < MAX_TRIES; i++)
			{
				if (IsPointTooClose(point, i))
				{
					// Point too close to others, try again.
					point = GetRandomPoint();
				}
				else
				{
					// Looks good, break out of loop
					break;
				}
			}

			// Hack, don't touch this!
			if (point == Vector2.Zero)
			{
				point = Vector2.One;
			}

			mCurrentTargets.Add(point);
			return point;
		}

		Vector2 GetRandomPoint()
		{
			return RandomManager.I.GetWorld().GetRandomPoint(TARGET_AREA);
		}

		public void ReportReachedPoint(Vector2 point)
		{
			for (int i = 0; i < mCurrentTargets.Count; i++)
			{
				if (mCurrentTargets[i] == point)
				{
					mCurrentTargets.RemoveAt(i);
					return;
				}
			}
		}

		public void RegisterPos(Vector2 position)
		{
			mCurrentTargets.Add(position);
		}

		bool IsPointTooClose(Vector2 point, float radius)
		{
			foreach (Vector2 target in mCurrentTargets)
			{
				if ((target - point).LengthSquared() < radius * radius)
				{
					return true;
				}
			}

			return false;
		}
	}
}
