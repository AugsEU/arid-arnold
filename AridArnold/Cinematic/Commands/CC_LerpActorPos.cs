
namespace AridArnold
{
	internal class CC_LerpActorPos : CC_ActorCommand
	{
		Vector2 mStartPos;
		Vector2 mEndPos;

		public CC_LerpActorPos(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mStartPos = MonoParse.GetVector(cmdNode["start"]);
			mEndPos = MonoParse.GetVector(cmdNode["end"]);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			float t = GetActivePercent(currentFrame);
			Vector2 newPos = MonoMath.Lerp(mStartPos, mEndPos, t);
			mTargetActor.SetPosition(newPos);
		}
	}
}
