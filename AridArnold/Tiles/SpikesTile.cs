namespace AridArnold
{
	class SpikesTile : InteractableTile
	{
		#region rInitialisation

		/// <summary>
		/// Spike tile constructor
		/// </summary>
		/// <param name="rotation">Orientation of spikes</param>
		public SpikesTile(CardinalDirection rotation, Vector2 position) : base(position)
		{
			mRotation = rotation;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Spikes");
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Kill Arnold if he intersects this. TO DO: Expand this to other entities.
		/// </summary>
		/// <param name="entity">Entity that intersected us</param>
		/// <param name="bounds">Our tile bounds</param>
		public override void OnEntityIntersect(Entity entity)
		{
			if (entity is Arnold)
			{
				//TO DO: Is an event needed here?
				EventManager.I.SendEvent(EventType.KillPlayer, new EArgs(this));
			}
		}



		/// <summary>
		/// Get bounds of this tile. Note that it is smaller than the spikes themselves to make it fairer.
		/// </summary>
		/// <param name="topLeft">Top left position of tile.</param>
		/// <param name="sideLength">Side length of tile</param>
		/// <returns>Collision rectangle</returns>
		protected override Rect2f CalculateBounds()
		{
			const float smallerFactor = 6.2f;
			float sideLength = sTILE_SIZE;
			Vector2 position = mPosition;

			switch (mRotation)
			{
				case CardinalDirection.Up:
					position.Y += smallerFactor;
					position.X += smallerFactor / 2.0f;
					break;
				case CardinalDirection.Right:
					position.Y += smallerFactor / 2.0f;
					break;
				case CardinalDirection.Left:
					position.X += smallerFactor;
					position.Y += smallerFactor / 2.0f;
					break;
				case CardinalDirection.Down:
					position.X += smallerFactor / 2.0f;
					break;
			}

			sideLength -= smallerFactor;

			return new Rect2f(position, position + new Vector2(sideLength, sideLength));
		}

		#endregion rCollision
	}
}
