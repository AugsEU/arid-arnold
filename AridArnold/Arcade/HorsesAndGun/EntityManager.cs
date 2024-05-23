using System.Collections.Generic;

namespace HorsesAndGun
{
	internal class EntityManager : Singleton<EntityManager>
	{
		private List<Entity> mRegisteredEntities = new List<Entity>();


		public void RegisterEntity(Entity entity, ContentManager content)
		{
			mRegisteredEntities.Add(entity);
			entity.LoadContent(content);
		}

		public void DeleteEntity(Entity entity)
		{
			mRegisteredEntities.Remove(entity);
		}

		public void ClearEntities()
		{
			mRegisteredEntities.Clear();
		}

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

			for (int i = 0; i < mRegisteredEntities.Count; i++)
			{
				if (mRegisteredEntities[i].DeleteMe())
				{
					DeleteEntity(mRegisteredEntities[i]);
					i--;
				}
			}
		}

		public int GetEntityNum()
		{
			return mRegisteredEntities.Count;
		}

		public Entity GetEntity(int index)
		{
			return mRegisteredEntities[index];
		}

		public void Draw(DrawInfo info)
		{
			foreach (Entity entity in mRegisteredEntities)
			{
				entity.Draw(info);
			}
		}
	}
}
