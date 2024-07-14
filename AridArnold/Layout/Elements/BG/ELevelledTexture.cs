namespace AridArnold
{
	internal class ELevelledTexture : ETexture
	{
		public ELevelledTexture(XmlNode node, Layout parent) : base(node, parent)
		{
		}

		protected override float GetRotation()
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			return -gameCam.GetCurrentSpec().mRotation;
		}
	}
}
