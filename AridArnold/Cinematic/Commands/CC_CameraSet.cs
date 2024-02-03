namespace AridArnold
{
	internal class CC_CameraSet : CinematicCommand
	{
		CameraSpec mCamState;

		public CC_CameraSet(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mCamState = MonoParse.GetCameraSpec(cmdNode);
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);

			screenCam.ForceNewSpec(mCamState);
		}

		public override void Draw(DrawInfo drawInfo)
		{
		}
	}
}
