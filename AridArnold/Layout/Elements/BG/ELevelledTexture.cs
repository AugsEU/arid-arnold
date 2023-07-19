namespace AridArnold
{
	internal class ELevelledTexture : ETexture
	{
		public ELevelledTexture(XmlNode node) : base(node)
		{
		}

		protected override float GetRotation()
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			return -gameCam.GetCurrentSpec().mRotation;
		}
	}
}
