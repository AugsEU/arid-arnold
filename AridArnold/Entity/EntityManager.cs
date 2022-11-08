namespace AridArnold
{
	/// <summary>
	/// Manager that updates and draws all entities.
	/// </summary>
	internal class EntityManager : Singleton<EntityManager>
	{
		#region rMembers

		private List<Entity> mRegisteredEntities = new List<Entity>();

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
				entity.Update(gameTime);
			}

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

		#endregion rUpdate





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

			foreach(Entity entity in mRegisteredEntities)
			{
				if(entity.GetType() == type && !ReferenceEquals(nearEntity, entity))
				{
					float distanceToEntity = (nearEntityPos - entity.GetCentrePos()).LengthSquared();

					if(distanceToEntity < distance)
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
