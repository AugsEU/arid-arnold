namespace AridArnold
{
	class CC_CameraLerp : CinematicCommand
	{
		CameraSpec mStartState;
		CameraSpec mEndState;

		public CC_CameraLerp(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			mStartState = MonoParse.GetCameraSpec(cmdNode["start"]);
			mEndState = MonoParse.GetCameraSpec(cmdNode["end"]);
		}

		public override void Draw(DrawInfo drawInfo)
		{
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			Camera screenCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.ScreenCamera);

			float t = GetActivePercent(currentFrame);

			CameraSpec newSpec = mStartState;
			newSpec.mPosition += (mEndState.mPosition - mStartState.mPosition) * t;
			newSpec.mRotation += (mEndState.mRotation - mStartState.mRotation) * t;
			newSpec.mZoom += (mEndState.mZoom - mStartState.mZoom) * t;

			screenCam.ForceNewSpec(newSpec);
		}
	}
}
