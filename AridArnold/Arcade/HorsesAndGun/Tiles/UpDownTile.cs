using System;

namespace HorsesAndGun.Tiles
{
	internal class UpDownTile : BasicTile
	{
		bool mGoUp;

		public UpDownTile(ContentManager content, bool _up) : base(content, GetPlusTex(_up))
		{
			mGoUp = _up;
		}

		static string GetPlusTex(bool up)
		{
			return up ? "Arcade/HorsesAndGun/Tiles/tile_go_up" : "Arcade/HorsesAndGun/Tiles/tile_go_down";

			throw new NotImplementedException();
		}

		public override void ApplyEffect(Horse horse, TrackManager trackManager)
		{
			SoundManager.I.PlaySFX(SoundManager.SFXType.TileActivate, 0.75f);
			horse.QueueOrderFront(HorseOrderType.moveTrack, mGoUp ? -1 : 1);
		}
	}
}
