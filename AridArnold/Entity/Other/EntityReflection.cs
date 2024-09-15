

namespace AridArnold
{
	/// <summary>
	/// Reflection of an entity, created by dual mirror tile.
	/// </summary>
	internal class EntityReflection : MovingEntity
	{
		#region rConstants

		#endregion rConstants



		#region rMembers

		PlatformingEntity mEntityToReflect;
		DualMirrorTile mParentTile;
		Vector2 mReflectionCentre;
		Vector2 mReflectionNormal;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create reflection of entity along an axis.
		/// </summary>
		public EntityReflection(PlatformingEntity entityToReflect, DualMirrorTile parent) : base(Vector2.Zero)
		{
			mEntityToReflect = entityToReflect;
			mReflectionCentre = parent.GetCentre();
			mReflectionNormal = parent.GetReflectionNormal();
			mReflectionNormal.Normalize();

			mParentTile = parent;

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

			Rect2f ourBounds = ColliderBounds();
			Rect2f futureBounds = ourBounds + mEntityToReflect.VelocityToDisplacement(gameTime);
			Rectangle tileBounds = TileManager.I.PossibleIntersectTiles(ourBounds + futureBounds);

			for (int x = tileBounds.X; x <= tileBounds.X + tileBounds.Width; x++)
			{
				for (int y = tileBounds.Y; y <= tileBounds.Y + tileBounds.Height; y++)
				{
					Tile tile = TileManager.I.GetTile(x, y);
					if (tile.pEnabled == false)
					{
						continue;
					}

					EntityManager.I.AddColliderSubmission(new ReflectedTileSubmission(tile, mEntityToReflect, mReflectionNormal, mReflectionCentre));
				}
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Mimic reflected entity
		/// </summary>
		public override void OrderedUpdate(GameTime gameTime)
		{
			mVelocity = mEntityToReflect.GetVelocity();
			mPrevVelocity = mEntityToReflect.GetPrevVelocity();
		}



		/// <summary>
		/// Dummy function
		/// </summary>
		public override void ReactToCollision(Vector2 collisionNormal)
		{
			// Shouldn't actually be colliding with anything.
			throw new NotImplementedException();
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



		/// <summary>
		/// Dead.
		/// </summary>
		public override void Kill()
		{
			if(mParentTile is null)
			{
				// We can get killed by 2 things in 1 frame, so this might have already been triggered.
				return;
			}
			mParentTile.SignalReflectionDeath();
			mParentTile = null;
			EntityManager.I.QueueDeleteEntity(this);

			Animator explodeAnimator = MonoData.I.LoadAnimator("Shared/Coin/Explode.max");
			FXManager.I.AddFX(new AnimationFX(GetPos(), explodeAnimator, DrawLayer.TileEffects));
			SFXManager.I.PlaySFX(AridArnoldSFX.ArnoldDeath, 0.2f);

			base.Kill();
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





		#region rUtility

		/// <summary>
		/// Passes interaction layer to base entity
		/// </summary>
		public override InteractionLayer GetInteractionLayer()
		{
			return mEntityToReflect.GetInteractionLayer();
		}

		#endregion rUtility
	}
}
