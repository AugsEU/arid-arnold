namespace AridArnold
{
	internal class HubRoomEdgeLoader : HubTransitionLoader
	{
		const float FADE_SPEED = 0.5f;
		const float SPAWN_BORDER = 15.0f;

		CardinalDirection mArriveFrom;

		public HubRoomEdgeLoader(int levelID, CardinalDirection arriveFrom) : base(levelID)
		{
			mArriveFrom = arriveFrom;
			mFadeIn = new FadeFX(new ScreenWipe(arriveFrom), FADE_SPEED, false);
			CardinalDirection opposite = Util.InvertDirection(arriveFrom);
			mFadeOut = new FadeFX(new ScreenWipe(opposite), FADE_SPEED, true);
		}

		protected override void PostLevelLoad()
		{
			foreach (Entity entity in mPersistentEntities)
			{
				SpawnEntityAtEdge(entity);
			}
		}

		void SpawnEntityAtEdge(Entity entity)
		{
			Vector2 pos = entity.GetPos();
			Rect2f collider = entity.ColliderBounds();

			switch (mArriveFrom)
			{
				case CardinalDirection.Up:
					pos.Y = SPAWN_BORDER;
					break;
				case CardinalDirection.Right:
					pos.X = GameScreen.GAME_AREA_WIDTH - SPAWN_BORDER - collider.Width;
					break;
				case CardinalDirection.Down:
					pos.Y = GameScreen.GAME_AREA_HEIGHT - SPAWN_BORDER - collider.Height;
					break;
				case CardinalDirection.Left:
					pos.X = SPAWN_BORDER;
					break;
			}

			entity.SetPos(pos);

			EntityManager.I.InsertEntity(entity);
		}
	}
}
