namespace HorsesAndGun.Tiles
{
	internal class DeathTile : BasicTile
	{
		public DeathTile(ContentManager content) : base(content, "Arcade/HorsesAndGun/Tiles/tile_death")
		{

		}
		public override void ApplyEffect(Horse horse, TrackManager trackManager)
		{
			horse.Kill();
		}
	}
}
