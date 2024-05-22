namespace GMTK2023
{
	internal class ExitGameButton : Button
	{
		public ExitGameButton(Vector2 topLeft, string text) : base(topLeft, text)
		{
		}

		protected override void DoAction()
		{
			GMTK2023.sInstance.Exit();
		}
	}
}
