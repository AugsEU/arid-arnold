
namespace AridArnold
{
	internal class CC_ActorLeafFall : CC_ActorCommand
	{
		const float ANGLE_SPEED = 0.2f;
		const float FALL_SPEED = 6.0f;

		Vector2 mBasePosition;
		float mStopY;
		float mElapsedTime;
		float mAmplitude;
		bool mStopped = false;

		public CC_ActorLeafFall(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mElapsedTime = 0.0f;
			mFrameSpan.SetMax(int.MaxValue);
			mBasePosition = MonoParse.GetVector(cmdNode);
			mStopY = MonoParse.GetFloat(cmdNode["yStop"]);
			mAmplitude = MonoParse.GetFloat(cmdNode["amp"], 30.0f);
		}


		public override void Update(GameTime gameTime, int currentFrame)
		{
			if (mStopped) return;

			float dt = Util.GetDeltaT(gameTime);

			Vector2 displacement = Vector2.Zero;

			displacement.Y = FALL_SPEED * (mElapsedTime / 2.0f - MathF.Sin(mElapsedTime / 2.0f + MathF.PI));
			displacement.X = mAmplitude * MathF.Sin(mElapsedTime * ANGLE_SPEED);

			bool flip = (mElapsedTime * ANGLE_SPEED + MathF.PI * 1.5f) % MathF.Tau < MathF.PI;
			mTargetActor.SetSpriteEffect(flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

			Vector2 actorPos = mBasePosition + displacement;
			if (actorPos.Y > mStopY)
			{
				actorPos.Y = mStopY;
				mStopped = true;
			}
			mTargetActor.SetPosition(actorPos);
			mElapsedTime += dt;
		}
	}
}
