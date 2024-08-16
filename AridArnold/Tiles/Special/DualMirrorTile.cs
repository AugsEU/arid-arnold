
namespace AridArnold
{
	/// <summary>
	/// Tile that creates copies of entities that touch it.
	/// </summary>
	internal class DualMirrorTile : SquareTile
	{
		#region rMembers

		Texture2D mInactiveTexture;
		Texture2D mActiveTexture;
		EntityReflection mChildReflection;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Dual mirror tile tha
		/// </summary>
		/// <param name="position"></param>
		public DualMirrorTile(CardinalDirection rotation, Vector2 position) : base(position)
		{
			mRotation = rotation;
			mChildReflection = null;
		}



		/// <summary>
		/// Load textures for this tile.
		/// </summary>
		public override void LoadContent()
		{
			mInactiveTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mirror/DualMirror");
			mActiveTexture = MonoData.I.MonoGameLoad<Texture2D>("Tiles/Mirror/DualMirrorActive");
			mTexture = mInactiveTexture;
			base.LoadContent();
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// When an entity touches us
		/// </summary>
		public override void OnTouch(MovingEntity entity, CollisionResults collisionResults)
		{
			if (mChildReflection is null && entity is PlatformingEntity)
			{
				Vector2 tileCentre = GetCentre();
				Vector2 reflectionNormal = GetReflectionNormal();
				PlatformingEntity platformingEntity = (PlatformingEntity)entity;

				mChildReflection = new EntityReflection(platformingEntity, this);
				EntityManager.I.QueueRegisterEntity(mChildReflection);

				SFXManager.I.PlaySFX(AridArnoldSFX.MirrorStep, 0.2f);
			}
			base.OnTouch(entity, collisionResults);
		}



		/// <summary>
		/// Call this when child reflection dies
		/// </summary>
		public void SignalReflectionDeath()
		{
			mChildReflection = null;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Get texture to draw
		/// </summary>
		public override Texture2D GetTexture()
		{
			return mChildReflection is not null ? mActiveTexture : mInactiveTexture;
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get vector pointing out of mirror
		/// </summary>
		public Vector2 GetReflectionNormal()
		{
			return Util.GetNormal(mRotation);
		}

		#endregion rUtility
	}
}
