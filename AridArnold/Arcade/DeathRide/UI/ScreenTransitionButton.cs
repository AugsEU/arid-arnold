namespace DeathRide.UI
{
	internal class ScreenTransitionButton : Button
	{
		ScreenType mScreen;

		public ScreenTransitionButton(Vector2 topLeft, string text, ScreenType screen) : base(topLeft, text)
		{
			mScreen = screen;
		}

		protected override void DoAction()
		{
			ScreenManager.I.ActivateScreen(mScreen);
		}
	}
}
