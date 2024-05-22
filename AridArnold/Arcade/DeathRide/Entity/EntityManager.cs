namespace GMTK2023
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
			foreach (Entity entity in mRegisteredEntities)
			{
				if (!entity.IsEnabled()) continue;
				entity.Update(gameTime);
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

			foreach (Entity entity in mQueuedDeletes)
			{
				DeleteEntity(entity);
			}

			mQueuedDeletes.Clear();
			mQueuedRegisters.Clear();
		}

		#endregion rUpdate





		#region rCollision

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



		public T GetNearestOfType<T>(Vector2 position) where T : class
		{
			float smallestDistSq = float.MaxValue;
			T retValue = null;
			foreach (Entity entity in mRegisteredEntities)
			{
				if (entity is T)
				{
					float distSq = (position - entity.GetCentrePos()).LengthSquared();
					if (distSq < smallestDistSq)
					{
						smallestDistSq = distSq;
						retValue = (T)Convert.ChangeType(entity, typeof(T));
					}
				}
			}

			return retValue;
		}



		/// <summary>
		/// Get persistent entities.
		/// </summary>
		public List<Entity> GetAllPersistent()
		{
			List<Entity> returnList = new List<Entity>();

			foreach (Entity entity in mRegisteredEntities)
			{
				if (entity.PersistLevelEntry())
				{
					returnList.Add(entity);
				}
			}

			return returnList;
		}

		#endregion rUtility
	}
}
