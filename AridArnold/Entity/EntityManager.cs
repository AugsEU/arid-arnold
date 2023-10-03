namespace AridArnold
{
	/// <summary>
	/// Manager that updates and draws all entities.
	/// </summary>
	internal class EntityManager : Singleton<EntityManager>
	{
		#region rConstants

		// Update order ranges. E.g. all moving entities fall between UPDATE_MENTITY_MIN and UPDATE_MENTITY_MAX
		public const float UPDATE_MENTITY_MIN = 0.0f;
		public const float UPDATE_MENTITY_MAX = 1.0f;

		#endregion rConstants





		#region rMembers

		List<Entity> mRegisteredEntities = new List<Entity>();
		List<EntityCollision> mCollisionBuffer = new List<EntityCollision>();
		List<ColliderSubmission> mAuxiliaryColliders = new List<ColliderSubmission>();
		EntityUpdateSorter mUpdateSorter = new EntityUpdateSorter();

		List<Entity> mQueuedRegisters = new List<Entity>();
		List<Entity> mQueuedDeletes = new List<Entity>();

		#endregion rMembers




		#region rUpdate

		/// <summary>
		/// Update all entities
		/// </summary>
		/// <param name="gameTime">Frame time</param>
		public void Update(GameTime gameTime)
		{
			mAuxiliaryColliders.Clear();

			// Do unordered update. To do: Multi-threading if it turns out to be worth it.
			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;
				entity.Update(gameTime);
			}

			mRegisteredEntities.Sort(mUpdateSorter);

			// Do ordered update.
			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;
				entity.OrderedUpdate(gameTime);
			}

			ResolveEntityTouching();

			// Add/Removed queued entities
			FlushQueues();
		}



		/// <summary>
		/// Flush add/delete queues.
		/// </summary>
		void FlushQueues()
		{
			foreach (Entity entity in mQueuedRegisters)
			{
				RegisterEntity(entity);
			}

			foreach(Entity entity in mQueuedDeletes)
			{
				DeleteEntity(entity);
			}

			mQueuedDeletes.Clear();
			mQueuedRegisters.Clear();
		}

		#endregion rUpdate





		#region rCollision

		/// <summary>
		/// Add collider.
		/// </summary>
		public void AddColliderSubmission(ColliderSubmission colliderSubmission)
		{
			mAuxiliaryColliders.Add(colliderSubmission);
		}



		/// <summary>
		/// Get the collision this entity will hit next. Returns null if no more collisions.
		/// </summary>
		public EntityCollision GetNextCollision(GameTime gameTime, MovingEntity entity)
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
			
			//mCollisionBuffer.Sort(EntityCollision.COLLISION_SORTER);

			if (mCollisionBuffer.Count > 0)
			{
				return MonoAlg.GetMin(ref mCollisionBuffer, EntityCollision.COLLISION_SORTER);
			}

			return null;
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
				if (!iEntity.IsEnabled()) continue;

				Rect2f iRect = iEntity.ColliderBounds();

				for (int j = i + 1; j < mRegisteredEntities.Count; j++)
				{
					Entity jEntity = mRegisteredEntities[j];
					if (!jEntity.IsEnabled()) continue;

					Rect2f jRect = jEntity.ColliderBounds();

					if (Collision2D.BoxVsBox(iRect, jRect))
					{
						//Both react.
						iEntity.OnCollideEntity(jEntity);
						jEntity.OnCollideEntity(iEntity);
					}
				}
			}
		}

		#endregion rCollision





		#region rEntityRegistry

		/// <summary>
		/// Register entity to this manager.
		/// </summary>
		/// <param name="entity">Entity to be registered</param>
		public void RegisterEntity(Entity entity)
		{
			mRegisteredEntities.Add(entity);
			entity.LoadContent();
		}


		/// <summary>
		/// Insert entity without reloading it
		/// </summary>
		public void InsertEntity(Entity entity)
		{
			mRegisteredEntities.Add(entity);
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
			mQueuedRegisters.Clear();
		}



		/// <summary>
		/// Call this when adding entities at runtime
		/// </summary>
		public void QueueRegisterEntity(Entity entity)
		{
			mQueuedRegisters.Add(entity);
		}


		/// <summary>
		/// Call this when adding entities at runtime
		/// </summary>
		public void QueueDeleteEntity(Entity entity)
		{
			mQueuedDeletes.Add(entity);
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
				if (!entity.IsEnabled()) continue;
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
		public bool AnyNearMe(float distance, Entity nearEntity, params Type[] types)
		{
			Vector2 nearEntityPos = nearEntity.GetCentrePos();
			distance = distance * distance;

			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;
				foreach (Type type in types)
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
			}

			return false;
		}



		/// <summary>
		/// Returns a list of all entities within a certain tdistance
		/// </summary>
		public List<Entity> GetNearPos(float distance, Vector2 pos, params Type[] types)
		{
			List<Entity> returnList = new List<Entity>();
			distance = distance * distance;

			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;
				foreach (Type type in types)
				{
					if (entity.GetType() == type)
					{
						float distanceToEntity = (pos - entity.GetCentrePos()).LengthSquared();

						if (distanceToEntity < distance)
						{
							returnList.Add(entity);
						}
					}
				}
			}

			return returnList;
		}



		/// <summary>
		/// Get nearest entity to position of types
		/// </summary>
		public Entity GetNearestEntity(Vector2 pos, params Type[] types)
		{
			float nearestDistance = float.MaxValue;
			Entity nearestEntity = null;
			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;
				foreach (Type type in types)
				{
					if (entity.GetType() == type)
					{
						float distanceToEntity = (pos - entity.GetCentrePos()).LengthSquared();

						if (distanceToEntity < nearestDistance)
						{
							nearestDistance=distanceToEntity;
							nearestEntity = entity;
						}
					}
				}
			}

			return nearestEntity;
		}



		/// <summary>
		/// Returns a list of all entities of a certain types
		/// </summary>
		public List<Entity> GetAllOfType(params Type[] types)
		{
			List<Entity> returnList = new List<Entity>();
			
			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;

				foreach (Type type in types)
				{
					if (entity.GetType() == type)
					{
						returnList.Add(entity);
						break;
					}
				}
			}

			return returnList;
		}



		/// <summary>
		/// Get persistent entities.
		/// </summary>
		public List<Entity> GetAllPersistent()
		{
			List<Entity> returnList = new List<Entity>();

			foreach(Entity entity in mRegisteredEntities)
			{
				if(entity.PersistLevelEntry())
				{
					returnList.Add(entity);
				}
			}

			return returnList;
		}

		#endregion rUtility
	}
}
