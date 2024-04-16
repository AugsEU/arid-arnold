namespace AridArnold
{
	internal class PillarPot : ExtenderEntity
	{
		#region rInitialisation

		/// <summary>
		/// Create plant pot at point
		/// </summary>
		public PillarPot(Vector2 pos, int length) : base(pos, length)
		{
		}



		/// <summary>
		/// Load content
		/// </summary>
		public override void LoadContent()
		{
			mMiddleTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PillarMiddle");
			mTopTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PillarTop");
			mGhostTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PillarGhost");
			mOffSeasonTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PillarBase");
			mOnSeasonTexture = MonoData.I.MonoGameLoad<Texture2D>("PlantPot/PillarRuin");

			mTexture = mOnSeasonTexture;
		}

		#endregion rInitialisation





		#region rUtility

		/// <summary>
		/// Enabled in "winter"
		/// </summary>
		protected override int GetOnSeason()
		{
			return 0;
		}

		#endregion rUtility
	}
}
