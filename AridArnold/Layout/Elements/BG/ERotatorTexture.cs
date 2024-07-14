namespace AridArnold
{
	internal class ERotatorTexture : ETexture
	{
		float mRotSpeed = 1.0f;

		public ERotatorTexture(XmlNode node, Layout parent) : base(node, parent)
		{
			mRotSpeed = MonoParse.GetFloat(node["mult"], 1.0f);
		}

		protected override float GetRotation()
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			return mRotSpeed * gameCam.GetCurrentSpec().mRotation;
		}
	}
}
