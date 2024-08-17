using System;

namespace HorsesAndGun.Tiles
{
	internal class ScoreTile : BasicTile
	{
		ulong mScoreToGive;

		public ScoreTile(ContentManager content, bool big) : base(content, GetPlusTex(big))
		{
			mScoreToGive = big ? 55u : 30u;
		}

		static string GetPlusTex(bool big)
		{
			return big ? "Arcade/HorsesAndGun/Tiles/big_score_tile" : "Arcade/HorsesAndGun/Tiles/small_score_tile";

			throw new NotImplementedException();
		}

		public override void ApplyEffect(Horse horse, TrackManager trackManager)
		{
			AridArnold.SFXManager.I.PlaySFX(AridArnold.AridArnoldSFX.HorseLevelUp, 0.75f);
			ScoreManager.I.AddCurrentScore(mScoreToGive);
		}
	}
}
