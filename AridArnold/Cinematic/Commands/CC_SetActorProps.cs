
namespace AridArnold
{
	internal class CC_SetActorProps : CC_ActorCommand
	{
		bool mHasAnim;
		Animator mNewAnimation; 
		Vector2? mPosition;
		DrawLayer? mDrawLayer;


		public CC_SetActorProps(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			string animPath = MonoParse.GetString(cmdNode["anim"]);

			mHasAnim = animPath.Length > 0;

			if(mHasAnim)
			{
				mNewAnimation = animPath == "null" ? null : MonoData.I.LoadAnimator(animPath);
			}

			mPosition = cmdNode["x"] is not null ? MonoParse.GetVector(cmdNode) : null;
			mDrawLayer = cmdNode["layer"] is not null ? MonoAlg.GetEnumFromString<DrawLayer>(MonoParse.GetString(cmdNode["layer"])) : null;
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			if(mPosition.HasValue)
			{
				mTargetActor.SetPosition(mPosition.Value);
			}
			
			if (mDrawLayer.HasValue)
			{
				mTargetActor.SetDrawLayer(mDrawLayer.Value);
			}

			if(mHasAnim)
			{
				mTargetActor.SetActiveAnimation(mNewAnimation);
			}
		}
	}
}
