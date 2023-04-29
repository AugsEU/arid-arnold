using AridArnold.Screens;

namespace AridArnold
{
	class HubLevel : Level
	{
		Texture2D mTransitionArrows;

		int mTopID;
		int mRightID;
		int mBottomID;
		int mLeftID;

		/// <summary>
		/// Create a "level" in the hub world.
		/// </summary>
		public HubLevel(AuxData data) : base(data)
		{
			// Strange order is to make editor UI make more sense.
			mTopID = data.GetIntParams()[0];
			mRightID = data.GetIntParams()[1];
			mBottomID = data.GetIntParams()[3];
			mLeftID = data.GetIntParams()[2];

			mTransitionArrows = MonoData.I.MonoGameLoad<Texture2D>("Shared/TransitionArrows");
		}



		/// <summary>
		/// Do update
		/// </summary>
		protected override LevelStatus UpdateInternal(GameTime gameTime)
		{
			int numEntities = EntityManager.I.GetEntityNum();
			for(int i = 0; i < numEntities; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if(entity is Arnold)
				{
					CheckRoomTransition((Arnold)entity);
				}
			}

			return LevelStatus.Continue;
		}



		/// <summary>
		/// Check the room transition
		/// </summary>
		void CheckRoomTransition(Arnold arnold)
		{

		}

		/// <summary>
		/// Draw level BG and other elements
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			for (int x = 0; x < GameScreen.GAME_AREA_WIDTH; x+=mTransitionArrows.Height)
			{
				if (mTopID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows, new Vector2(x, mTransitionArrows.Width), MathF.PI * 1.5f);
				}

				if(mBottomID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows, new Vector2(x + mTransitionArrows.Height, GameScreen.GAME_AREA_HEIGHT - mTransitionArrows.Width), MathF.PI * 0.5f);
				}
			}

			for (int y = 0; y < GameScreen.GAME_AREA_HEIGHT; y += mTransitionArrows.Height)
			{
				if (mRightID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows, new Vector2(GameScreen.GAME_AREA_WIDTH - mTransitionArrows.Width, y), 0.0f);
				}

				if (mLeftID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows, new Vector2(mTransitionArrows.Width, y + mTransitionArrows.Height), MathF.PI);
				}
			}

			base.Draw(info);
		}
	}
}
