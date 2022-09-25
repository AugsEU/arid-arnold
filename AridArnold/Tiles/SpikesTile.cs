namespace AridArnold
{
	class SpikesTile : InteractableTile
	{
		#region rInitialisation

		/// <summary>
		/// Spike tile constructor
		/// </summary>
		/// <param name="rotation">Orientation of spikes</param>
		public SpikesTile(CardinalDirection rotation) : base()
		{
			mRotation = rotation;
		}



		/// <summary>
		/// Load all textures and assets
		/// </summary>
		/// <param name="content">Monogame content manager</param>
		public override void LoadContent(ContentManager content)
		{
			mTexture = content.Load<Texture2D>("Tiles/spikes");
		}

		#endregion rInitialisation





		#region rCollision

		/// <summary>
		/// Kill Arnold if he intersects this. TO DO: Expand this to other entities.
		/// </summary>
		/// <param name="entity">Entity that intersected us</param>
		/// <param name="bounds">Our tile bounds</param>
		public override void OnEntityIntersect(Entity entity, Rect2f bounds)
		{
			if (entity is Arnold)
			{
				EArgs eArgs;
				eArgs.sender = this;

				//TO DO: Is an event needed here?
				EventManager.I.SendEvent(EventType.KillPlayer, eArgs);
			}
		}



		/// <summary>
		/// Get bounds of this tile. Note that it is smaller than the spikes themselves to make it fairer.
		/// </summary>
		/// <param name="topLeft">Top left position of tile.</param>
		/// <param name="sideLength">Side length of tile</param>
		/// <returns>Collision rectangle</returns>
		public override Rect2f GetBounds(Vector2 topLeft, float sideLength)
		{
			const float smallerFactor = 7.0f;

			switch (mRotation)
			{
				case CardinalDirection.Up:
					topLeft.Y += smallerFactor;
					topLeft.X += smallerFactor / 2.0f;
					break;
				case CardinalDirection.Right:
					topLeft.Y += smallerFactor / 2.0f;
					break;
				case CardinalDirection.Left:
					topLeft.X += smallerFactor;
					topLeft.Y += smallerFactor / 2.0f;
					break;
				case CardinalDirection.Down:
					topLeft.X += smallerFactor / 2.0f;
					break;
			}

			sideLength -= smallerFactor;

			return new Rect2f(topLeft, topLeft + new Vector2(sideLength, sideLength));
		}

		#endregion rCollision
	}
}
