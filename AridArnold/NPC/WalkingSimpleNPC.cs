
namespace AridArnold
{
	abstract class WalkingSimpleNPC : SimpleTalkNPC
	{
		protected Animator mWalkAnim;
		PercentageTimer mWalkTimer;

		public WalkingSimpleNPC(Vector2 pos, float walkFreq = 5000.0f) : base(pos)
		{
			mWalkTimer = new PercentageTimer(walkFreq * 2.0f);
			mWalkTimer.Start();
			mWalkSpeed = 3.0f;
		}

		bool WantsWalk()
		{
			return mWalkTimer.GetPercentageF() < 0.5f;
		}

		public override void Update(GameTime gameTime)
		{
			if (mWalkTimer.GetPercentageF() >= 1.0f)
			{
				mWalkTimer.Reset();
			}

			mWalkAnim.Update(gameTime);

			if (WantsWalk() && !IsTalking())
			{
				WalkAround();
			}
			else
			{
				SetWalkDirection(WalkDirection.None);
			}

			base.Update(gameTime);
		}

		void WalkAround()
		{
			bool canGoWhereFacing = false;

			switch (GetPrevWalkDirection())
			{
				case WalkDirection.Left:
					canGoWhereFacing = CheckSolid(-1, 1) && !CheckSolid(-1, 0);
					break;
				case WalkDirection.Right:
					canGoWhereFacing = CheckSolid(1, 1) && !CheckSolid(1, 0);
					break;
				case WalkDirection.None:
					break;
				default:
					break;
			}

			WalkDirection newWalkDir = GetPrevWalkDirection();

			if (!canGoWhereFacing)
			{
				newWalkDir = Util.InvertDirection(newWalkDir);
			}

			SetWalkDirection(newWalkDir);
			SetPrevWalkDirection(newWalkDir);
		}

		protected override Texture2D GetDrawTexture()
		{
			if (GetWalkDirection() != WalkDirection.None)
			{
				return mWalkAnim.GetCurrentTexture();
			}

			return base.GetDrawTexture();
		}
	}
}
