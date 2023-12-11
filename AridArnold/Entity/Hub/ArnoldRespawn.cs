namespace AridArnold
{
	class ArnoldRespawn : Entity
	{
		CardinalDirection mGravityDir;
		WalkDirection mStartDir;

		public ArnoldRespawn(Vector2 pos, CardinalDirection gravityDir, WalkDirection startDir) : base(pos)
		{
			mGravityDir = gravityDir;
			mStartDir = startDir;
		}

		public override void Update(GameTime gameTime)
		{
			if (ArnoldExists() == false)
			{
				Arnold newArnold = new Arnold(mPosition);
				newArnold.SetGravity(mGravityDir);
				newArnold.SetPrevWalkDirection(mStartDir);

				EntityManager.I.QueueRegisterEntity(newArnold);
			}
		}

		bool ArnoldExists()
		{
			int entityNum = EntityManager.I.GetEntityNum();
			for (int i = 0; i < entityNum; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if (entity is Arnold)
				{
					return true;
				}
			}

			return false;
		}

		public override Rect2f ColliderBounds()
		{
			return new Rect2f(mPosition, mPosition);
		}

		public override void Draw(DrawInfo info)
		{
		}

		public override void LoadContent()
		{
		}
	}
}
