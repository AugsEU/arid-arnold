using Microsoft.Xna.Framework;
using System.Data.SqlTypes;

namespace AridArnold
{
	class SpikesTile : InteractableTile
	{
		#region rConstants

		const float SIZE_DEC = 6.2f;

		#endregion rConstants





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
		/// Kill Arnold if he intersects this.
		/// </summary>
		public override void OnEntityIntersect(Entity entity)
		{
			if(ShouldWalkOnSpikes(entity))
			{
				return;
			}

			if (entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				entity.Kill();
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
			Vector2 position = mPosition;

			switch (mRotation)
			{
				case CardinalDirection.Up:
					position.Y += SIZE_DEC;
					position.X += SIZE_DEC / 2.0f;
					break;
				case CardinalDirection.Right:
					position.Y += SIZE_DEC / 2.0f;
					break;
				case CardinalDirection.Left:
					position.X += SIZE_DEC;
					position.Y += SIZE_DEC / 2.0f;
					break;
				case CardinalDirection.Down:
					position.X += SIZE_DEC / 2.0f;
					break;
			}

			float sideLength = sTILE_SIZE - SIZE_DEC;

			return new Rect2f(position, sideLength, sideLength);
		}



		/// <summary>
		/// Resolve collision with an entity
		/// </summary>
		public override CollisionResults Collide(MovingEntity entity, GameTime gameTime)
		{
			if(ShouldWalkOnSpikes(entity))
			{
				Rect2f entityCol = entity.ColliderBounds();
				Vector2 entityVecDisp = entity.VelocityToDisplacement(gameTime);
				Rect2f ourBounds = GetArmyBootsBounds();

				return Collision2D.MovingRectVsRect(entityCol, entityVecDisp, ourBounds);
			}

			return base.Collide(entity, gameTime);
		}

		#endregion rCollision





		#region rUtil

		/// <summary>
		/// Should this entity walk on spikes?
		/// </summary>
		private bool ShouldWalkOnSpikes(Entity entity)
		{
			Item activeItem = ItemManager.I.GetActiveItem();
			if (activeItem is not ArmyBoots)
			{
				// Not wearing boots
				return false;
			}

			if (entity is not PlatformingEntity)
			{
				// Not a platforming entity
				return false;
			}

			if (!entity.OnInteractLayer(InteractionLayer.kPlayer))
			{
				// Not a player.
				return false;
			}

			PlatformingEntity platformingEntity = (PlatformingEntity)entity;
			CardinalDirection gravDir = platformingEntity.GetGravityDir();
			CardinalDirection invDir = Util.InvertDirection(mRotation);

			if(gravDir != invDir)
			{
				// Not standing on us.
				return false;
			}

			return true;
		}



		/// <summary>
		/// Get bounds for walking on spikes with army boots
		/// </summary>
		private Rect2f GetArmyBootsBounds()
		{
			const float SIZE_DEC = 6.2f;
			Vector2 position = mPosition;

			float height = sTILE_SIZE;
			float width = sTILE_SIZE;
			switch (mRotation)
			{
				case CardinalDirection.Up:
				case CardinalDirection.Down:
					position.X += SIZE_DEC / 2.0f;
					width -= SIZE_DEC / 2.0f;
					break;
				case CardinalDirection.Right:
				case CardinalDirection.Left:
					position.Y += SIZE_DEC / 2.0f;
					height -= SIZE_DEC / 2.0f;
					break;
			}

			return new Rect2f(position, width, height);
		}

		#endregion rUtil.
	}
}
