namespace AridArnold
{
	internal class PlantPot : ExtenderEntity
	{
		#region rInitialisation

		/// <summary>
		/// Create plant pot at point
		/// </summary>
		public PlantPot(Vector2 pos, int length) : base(pos, length)
		{
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mMiddleTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/TreeTrunk");
			mTopTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/TreeTop");
			mGhostTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/TreeGhost");
			mOffSeasonTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PotSummer");
			mOnSeasonTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PotWinter");

			mTexture = mOnSeasonTexture;
		}

		#endregion rInitialisation





		#region rUtility

		/// <summary>
		/// Enabled in "summer"
		/// </summary>
		protected override int GetOnSeason()
		{
			return 0;
		}

		#endregion rUtility
	}
}
