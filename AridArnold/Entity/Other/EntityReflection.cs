

namespace AridArnold
{
	/// <summary>
	/// Reflection of an entity, created by dual mirror tile.
	/// </summary>
	internal class EntityReflection : Entity
	{
		#region rConstants

		#endregion rConstants



		#region rMembers

		PlatformingEntity mEntityToReflect;
		Vector2 mReflectionCentre;
		Vector2 mReflectionNormal;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create reflection of entity along an axis.
		/// </summary>
		public EntityReflection(PlatformingEntity entityToReflect, Vector2 centre, Vector2 normal) : base(Vector2.Zero)
		{
			mEntityToReflect = entityToReflect;
			mReflectionCentre = centre;
			mReflectionNormal = normal;
			mReflectionNormal.Normalize();

			//Canonical direction
			mReflectionNormal.X = -Math.Abs(mReflectionNormal.X);
			mReflectionNormal.Y = -Math.Abs(mReflectionNormal.Y);

			SetReflectedPosition();
		}



		/// <summary>
		/// Load textures
		/// </summary>
		public override void LoadContent()
		{
			// Load nothing
		}

		#endregion rInit


		#region rUpdate

		/// <summary>
		/// Update entity reflection
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			SetReflectedPosition();
			base.Update(gameTime);
		}



		/// <summary>
		/// Consider reflected position
		/// </summary>
		void SetReflectedPosition()
		{
			Rect2f collider = ColliderBounds();
			Vector2 reflectedPos = MonoMath.Reflect(mEntityToReflect.GetPos(), mReflectionNormal, mReflectionCentre);

			Vector2 colliderSizeVec = new Vector2(collider.Width, collider.Height);
			Vector2 offset = MonoMath.CompMult(colliderSizeVec, mReflectionNormal);

			reflectedPos += offset;

			SetPos(reflectedPos);
		}



		/// <summary>
		/// Get collider based on reflector
		/// </summary>
		public override Rect2f ColliderBounds()
		{
			Rect2f reflectCollider = mEntityToReflect.ColliderBounds();

			Vector2 toUs = GetPos() - reflectCollider.min;

			reflectCollider.min += toUs;
			reflectCollider.max += toUs;

			return reflectCollider;
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw reflection
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Color reflectColor = new Color(100, 100, 250, 150);
			Texture2D textureToDraw = mEntityToReflect.GetDrawTexture();
			Rect2f originalTextureRect = ColliderBounds();
			DrawLayer drawLayer = mEntityToReflect.GetDrawLayer();
			Color colorToDraw = reflectColor;

			// These need to be reflected
			CardinalDirection gravityDir = Util.ReflectCardinalDirection(mEntityToReflect.GetGravityDir(), mReflectionNormal);
			CardinalDirection prevWalkCardDir = Util.WalkDirectionToCardinal(mEntityToReflect.GetPrevWalkDirection(), gravityDir);
			prevWalkCardDir = Util.ReflectCardinalDirection(prevWalkCardDir, mReflectionNormal);
			WalkDirection prevDir = Util.CardinalToWalkDirection(prevWalkCardDir, gravityDir);

			MonoDraw.DrawPlatformer(info, originalTextureRect, textureToDraw, colorToDraw, gravityDir, prevDir, drawLayer);
		}

		#endregion rDraw
	}
}
