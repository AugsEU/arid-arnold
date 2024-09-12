
namespace AridArnold
{
	internal class CC_PauseUntilConfirm : CinematicCommand
	{
		bool mWantPause;

		public CC_PauseUntilConfirm(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mWantPause = true;
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			if(InputManager.I.AnyGangPressed(BindingGang.SysConfirm))
			{
				mWantPause = false;
			}
			base.Update(gameTime, currentFrame);
		}

		public override bool RequestPause()
		{
			return mWantPause;
		}
	}
}
