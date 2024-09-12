
namespace AridArnold
{
	internal class CC_ActorFloatAway : CC_ActorCommand
	{
		Vector2 mVelocity;
		Vector2 mAccel;

		public CC_ActorFloatAway(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mFrameSpan.SetMax(int.MaxValue);
			mAccel = MonoParse.GetVector(cmdNode["accelerate"]);
			mVelocity = new Vector2(0.0f, -2.0f);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			float dt = Util.GetDeltaT(gameTime);
			Vector2 pos = mTargetActor.GetPosition();

			mVelocity += mAccel * dt;

			pos += mVelocity * dt;

			mTargetActor.SetPosition(pos);
		}
	}
}
