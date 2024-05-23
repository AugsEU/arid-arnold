namespace HorsesAndGun.Tiles
{
	internal class FastReloadTile : BasicTile
	{
		public FastReloadTile(ContentManager content) : base(content, "Arcade/HorsesAndGun/Tiles/fast_reload")
		{

		}
		public override void ApplyEffect(Horse horse, TrackManager trackManager)
		{
			MainGameScreen mainGameScreen = (MainGameScreen)ScreenManager.I.GetScreen(ScreenType.MainGame);

			if (mainGameScreen != null)
			{
				mainGameScreen.FastReload();
			}
		}
	}
}
