using System;

namespace HorsesAndGun.Tiles
{
	internal class PlusTile : BasicTile
	{
		int mMoveAmount;

		public PlusTile(ContentManager content, int _amount) : base(content, GetPlusTex(_amount))
		{
			mMoveAmount = _amount;
		}

		static string GetPlusTex(int amount)
		{
			switch (amount)
			{
				case 1: return "Arcade/HorsesAndGun/Tiles/plus_one_tile";
				case 2: return "Arcade/HorsesAndGun/Tiles/plus_two_tile";
				case 3: return "Arcade/HorsesAndGun/Tiles/plus_three_tile";
			}

			throw new NotImplementedException();
		}

		public override void ApplyEffect(Horse horse, TrackManager trackManager)
		{
			AridArnold.SFXManager.I.PlaySFX(AridArnold.AridArnoldSFX.HorseLevelUp, 0.75f);

			horse.QueueOrderFront(HorseOrderType.moveTile, mMoveAmount);
		}
	}
}
