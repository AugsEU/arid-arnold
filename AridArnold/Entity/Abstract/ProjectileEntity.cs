using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AridArnold.Tiles.Basic;

namespace AridArnold
{
    abstract class ProjectileEntity : PlatformingEntity
	{
		#region rConstants

		const float SPEED_KILL_LIMIT = 32.0f;

		#endregion rConstants





		#region rTypes

		protected enum ProjectileState
		{
			FreeMotion,
			Exploding
		}

		#endregion rTypes





		#region rMembers

		protected Entity mParent;
		protected ProjectileState mState;
		protected Animator mExplodingAnim = null;
		protected Vector2 mExplosionCentre;
		protected Vector2 mExplosionNormal;

		#endregion rMembers





		#region rInitialisation

		/// <summary>
		/// Projectile at point
		/// </summary>
		/// <param name="pos"></param>
		public ProjectileEntity(Entity parent, Vector2 pos, float gravity = PlatformingEntity.DEFAULT_GRAVITY) : base(pos, 0.0f, 0.0f, gravity, 0.0f)
		{
			mParent = parent;
			mState = ProjectileState.FreeMotion;
			mExplosionCentre = Vector2.Zero;
			mExplosionNormal = Vector2.Zero;
		}

		#endregion rInitialisation






		#region rUpdate

		/// <summary>
		/// Update projectile.
		/// </summary>
		public override void Update(GameTime gameTime)
		{
			mExplodingAnim.Update(gameTime);

			bool shouldDelete = CheckOffScreen() || (mState == ProjectileState.Exploding && mExplodingAnim.IsPlaying() == false);

			base.Update(gameTime);

			if (shouldDelete) EntityManager.I.QueueDeleteEntity(this);
		}



		/// <summary>
		/// Update Projectile collision.
		/// </summary>
		/// <param name="gameTime">Frame time.</param>
		public override void OrderedUpdate(GameTime gameTime)
		{
			if(mState != ProjectileState.FreeMotion)
			{
				return;
			}

			Vector2 prevDisp = VelocityToDisplacement(gameTime);
			mPrevVelocity = mVelocity;
			List<EntityCollision> collisions = new List<EntityCollision>();
			TileManager.I.GatherCollisions(gameTime, this, ref collisions);

			if (collisions.Count > 0)
			{
				EntityCollision firstCollision = MonoAlg.GetMin(ref collisions, EntityCollision.COLLISION_SORTER);
				firstCollision.PostCollisionReact(this);

				// Collision reaction could change enabled status.
				if (IsEnabled())
				{
					CollisionResults colResult = firstCollision.GetResult();
					mExplosionCentre = mPosition + colResult.t.Value * VelocityToDisplacement(gameTime);

					Rect2f collider = ColliderBounds();
					if(colResult.normal.X < 0.0f)
					{
						mExplosionCentre.X += collider.Width;
					}
					else if(colResult.normal.Y < 0.0f)
					{
						mExplosionCentre.Y += collider.Height;
					}

					mExplosionNormal = colResult.normal;
					mState = ProjectileState.Exploding;
					mExplodingAnim.Play();
				}
			}
			else
			{
				ApplyVelocity(gameTime);
			}
		}



		/// <summary>
		/// Returns true if we are off screen.
		/// </summary>
		bool CheckOffScreen()
		{
			Rect2f collider = ColliderBounds();

			// Check X
			if (collider.max.X < -Tile.sTILE_SIZE || collider.min.X > TileManager.I.GetDrawWidth() + Tile.sTILE_SIZE * 4.0f)
			{
				return true;
			}

			// Check Y
			if (collider.max.Y < -Tile.sTILE_SIZE || collider.min.Y > TileManager.I.GetDrawHeight() + Tile.sTILE_SIZE * 4.0f)
			{
				return true;
			}

			return false;
		}



		/// <summary>
		/// React to collision.
		/// </summary>
		public override void ReactToCollision(Vector2 collisionNormal)
		{
		}



		/// <summary>
		/// Kill player.
		/// </summary>
		protected void KillPlayer(MovingEntity movingEntity)
		{
			if(movingEntity.GetVelocity().LengthSquared() > SPEED_KILL_LIMIT * SPEED_KILL_LIMIT)
			{
				// Entity is travelling too fast. Not fair to kill them.
				return;
			}

			//Kill the player on touching.
			movingEntity.Kill();
		}



		/// <summary>
		/// This entity can't be killed.
		/// </summary>
		public override void Kill()
		{
		}

		#endregion rUpdate
	}
}
