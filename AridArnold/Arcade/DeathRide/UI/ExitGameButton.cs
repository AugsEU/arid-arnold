namespace DeathRide
{
	internal class ExitGameButton : Button
	{
		public ExitGameButton(Vector2 topLeft, string text) : base(topLeft, text)
		{
		}

		protected override void DoAction()
		{
			// A TO DO: Make this return to Arid Arnold.
			//DeathRide.sInstance.Exit();
		}
	}
}
