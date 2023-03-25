namespace AridArnold
{
	class TeleportPipe : SquareTile
	{
		#region rMembers

		Entity mHoldingEntity;
		CardinalDirection mArrivalDirection;
		Texture2D mBaseTexture;

		#endregion rMembers



		#region rInitialisation

		/// <summary>
		/// Teleportation pipe
		/// </summary>
		public TeleportPipe(Vector2 position) : base(position)
		{
			mHoldingEntity = null;
			mArrivalDirection = CardinalDirection.Down;
		}


		/// <summary>
		/// Load textures for teleportation pipe
		/// </summary>
		public override void LoadContent(ContentManager content)
		{
			mBaseTexture = content.Load<Texture2D>("Tiles/WW7/TeleportPipe");

			base.LoadContent(content);
		}

		#endregion rInitialisation





		#region rUpdate

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get teleportation pipe texture
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mBaseTexture;
		}

		#endregion rDraw
	}
}
