namespace AridArnold
{
	/// <summary>
	/// Manager that updates and draws all entities.
	/// </summary>
	internal class EntityManager : Singleton<EntityManager>
	{
		#region rConstants

		const int COLLISION_BUFFER_MAX_SIZE = 512; //Unused

		#endregion rConstants




		#region rMembers

		List<Entity> mRegisteredEntities = new List<Entity>();
		List<EntityCollision> mCollisionBuffer = new List<EntityCollision>();
		List<ColliderSubmission> mAuxiliaryColliders = new List<ColliderSubmission>();
		EntityUpdateSorter mUpdateSorter = new EntityUpdateSorter();

		#endregion rMembers




		#region rUpdate

		/// <summary>
		/// Update all entities
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			GatherEntityColliders();

			// Do unordered update. To do: Multi-threading if it turns out to be worth it.
			foreach (Entity entity in mRegisteredEntities)
			{
				entity.Update(gameTime);
			}

			mRegisteredEntities.Sort(mUpdateSorter);

			// Do ordered update.
			foreach (Entity entity in mRegisteredEntities)
			{
				entity.OrderedUpdate(gameTime);
			}

			ResolveEntityTouching();
		}

		#endregion rUpdate





		#region rCollision

		/// <summary>
		/// Gather entity collider
		/// </summary>
		void GatherEntityColliders()
		{
			mAuxiliaryColliders.Clear();

			foreach (Entity entity in mRegisteredEntities)
			{
				ColliderSubmission submission = entity.GetColliderSubmission();

				if(submission != null)
				{
					mAuxiliaryColliders.Add(submission);
				}
			}
		}



		/// <summary>
		/// Resolve all CollideWithEntity
		/// </summary>
		void ResolveEntityTouching()
		{
			// Not the best but the number of entities is small enough for optimisations to not be needed.
			for (int i = 0; i < mRegisteredEntities.Count - 1; i++)
			{
				Entity iEntity = mRegisteredEntities[i];

				Rect2f iRect = iEntity.ColliderBounds();

				for (int j = i + 1; j < mRegisteredEntities.Count; j++)
				{
					Entity jEntity = mRegisteredEntities[j];

					Rect2f jRect = jEntity.ColliderBounds();

					if (Collision2D.BoxVsBox(iRect, jRect))
					{
						//Both react.
						iEntity.CollideWithEntity(jEntity);
						jEntity.CollideWithEntity(iEntity);
					}
				}
			}
		}



		/// <summary>
		/// Do physics for a single entity. Returns a list of collided normals for reactions.
		/// </summary>
		public List<Vector2> UpdateCollisionEntity(GameTime gameTime, MovingEntity entity)
		{
			List<Vector2> collidedNormals = new List<Vector2>();
			while(GatherAllCollisions(gameTime, entity))
			{
				CollisionResults collisionResults = mCollisionBuffer[0].GetResult();

				Vector2 pushVec = collisionResults.normal * new Vector2(Math.Abs(entity.pVelocity.X), Math.Abs(entity.pVelocity.Y)) * (1.0f - collisionResults.t.Value) * 1.001f;
				entity.pVelocity += pushVec;

				mCollisionBuffer[0].PostCollisionReact(entity);
				collidedNormals.Add(collisionResults.normal);
			}

			return collidedNormals;
		}



		/// <summary>
		/// Gather collisions for an entity.
		/// </summary>
		public bool GatherAllCollisions(GameTime gameTime, MovingEntity entity)
		{
			mCollisionBuffer.Clear();

			//Gather all collisions
			TileManager.I.GatherCollisions(gameTime, entity, ref mCollisionBuffer);

			foreach(ColliderSubmission submission in mAuxiliaryColliders)
			{
				if(submission.CanCollideWith(entity))
				{
					EntityCollision collision = submission.GetEntityCollision(gameTime, entity);

					if(collision != null)
					{
						mCollisionBuffer.Add(collision);
					}
				}
			}
			

			if (mCollisionBuffer.Count > 0)
			{
				mCollisionBuffer.Sort(EntityCollision.COLLISION_SORTER);
				return true;
			}

			return false;
		}

		#endregion rCollision





		#region rEntityRegistry

		/// <summary>
		/// Register entity to this manager.
		/// </summary>
		/// <param name="entity">Entity to be registered</param>
		/// <param name="content">Monogame content manager</param>
		public void RegisterEntity(Entity entity, ContentManager content)
		{
			mRegisteredEntities.Add(entity);
			entity.LoadContent(content);
		}



		/// <summary>
		/// Remove entity from registry.
		/// </summary>
		/// <param name="entity">Entity to be removed</param>
		public void DeleteEntity(Entity entity)
		{
			mRegisteredEntities.Remove(entity);
		}



		/// <summary>
		/// Clear all entities.
		/// </summary>
		public void ClearEntities()
		{
			mRegisteredEntities.Clear();
		}

		#endregion rEntityRegistry





		#region rDraw

		/// <summary>
		/// Draw all entities
		/// </summary>
		/// <param name="info">Info needed to draw</param>
		public void Draw(DrawInfo info)
		{
			foreach (Entity entity in mRegisteredEntities)
			{
				entity.Draw(info);
			}
		}

		#endregion rDraw





		#region rUtility

		/// <summary>
		/// Get total number of entities.
		/// </summary>
		/// <returns>Total number of entities.</returns>
		public int GetEntityNum()
		{
			return mRegisteredEntities.Count;
		}



		/// <summary>
		/// Get entity at specific index. Mostly used when iterating. Indices may change.
		/// </summary>
		/// <param name="index">Index of entity you want to access.</param>
		/// <returns>Entity at specific index</returns>
		public Entity GetEntity(int index)
		{
			return mRegisteredEntities[index];
		}


		/// <summary>
		/// Are any entities of a certain type near this entity.
		/// </summary>
		public bool AnyNearMe(float distance, Entity nearEntity, Type type)
		{
			Vector2 nearEntityPos = nearEntity.GetCentrePos();
			distance = distance * distance;

			foreach (Entity entity in mRegisteredEntities)
			{
				if (entity.GetType() == type && !ReferenceEquals(nearEntity, entity))
				{
					float distanceToEntity = (nearEntityPos - entity.GetCentrePos()).LengthSquared();

					if (distanceToEntity < distance)
					{
						return true;
					}
				}
			}

			return false;
		}

		#endregion rUtility
	}
}
