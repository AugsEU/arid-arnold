namespace AridArnold
{
	class HubLevel : Level
	{
		const float TRANSITION_BORDER = 10.0f;

		Texture2D mTransitionArrows;

		int mTopID;
		int mRightID;
		int mBottomID;
		int mLeftID;

		/// <summary>
		/// Create a "level" in the hub world.
		/// </summary>
		public HubLevel(AuxData data, int id) : base(data, id)
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
		protected override void UpdateInternal(GameTime gameTime)
		{
			int numEntities = EntityManager.I.GetEntityNum();
			for (int i = 0; i < numEntities; i++)
			{
				Entity entity = EntityManager.I.GetEntity(i);
				if (entity is Arnold)
				{
					CheckRoomTransition((Arnold)entity);
				}
			}
		}



		/// <summary>
		/// Check the room transition
		/// </summary>
		void CheckRoomTransition(Arnold arnold)
		{
			Rect2f collider = arnold.ColliderBounds();

			HubRoomEdgeLoader loader = null;
			if (mRightID != 0)
			{
				if (arnold.GetCentrePos().X > GameScreen.GAME_AREA_WIDTH - TRANSITION_BORDER)
				{
					loader = new HubRoomEdgeLoader(mRightID, CardinalDirection.Left);
				}
			}

			if (mLeftID != 0 && loader is null)
			{
				if (arnold.GetCentrePos().X < TRANSITION_BORDER)
				{
					loader = new HubRoomEdgeLoader(mLeftID, CardinalDirection.Right);
				}
			}

			if (mTopID != 0 && loader is null)
			{
				if (arnold.GetCentrePos().Y < TRANSITION_BORDER)
				{
					loader = new HubRoomEdgeLoader(mTopID, CardinalDirection.Down);
				}
			}

			if (mBottomID != 0 && loader is null)
			{
				if (arnold.GetCentrePos().Y > GameScreen.GAME_AREA_HEIGHT - TRANSITION_BORDER)
				{
					loader = new HubRoomEdgeLoader(mBottomID, CardinalDirection.Up);
				}
			}

			if (loader is not null)
			{
				loader.AddPersistentEntities(arnold);
				CampaignManager.I.QueueLoadSequence(loader);
			}
		}

		/// <summary>
		/// Draw level BG and other elements
		/// </summary>
		public override void Draw(DrawInfo info)
		{
			Color arrowColors = mTheme.GetExitColor();
			for (int x = 0; x < GameScreen.GAME_AREA_WIDTH; x += mTransitionArrows.Height)
			{
				if (mTopID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows,
						new Vector2(x, mTransitionArrows.Width),
						null,
						arrowColors,
						MathF.PI * 1.5f, Vector2.Zero,
						1.0f, SpriteEffects.None, DrawLayer.Default);
				}

				if (mBottomID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows,
						new Vector2(x + mTransitionArrows.Height, GameScreen.GAME_AREA_HEIGHT - mTransitionArrows.Width),
						null,
						arrowColors,
						MathF.PI * 0.5f, Vector2.Zero,
						1.0f, SpriteEffects.None, DrawLayer.Default);
				}
			}

			for (int y = 0; y < GameScreen.GAME_AREA_HEIGHT; y += mTransitionArrows.Height)
			{
				if (mRightID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows,
						new Vector2(GameScreen.GAME_AREA_WIDTH - mTransitionArrows.Width, y),
						null,
						arrowColors,
						0.0f, Vector2.Zero,
						1.0f, SpriteEffects.None, DrawLayer.Default);
				}

				if (mLeftID != 0)
				{
					MonoDraw.DrawTexture(info, mTransitionArrows,
						new Vector2(mTransitionArrows.Width, y + mTransitionArrows.Height),
						null,
						arrowColors,
						MathF.PI, Vector2.Zero,
						1.0f, SpriteEffects.None, DrawLayer.Default);
				}
			}

			base.Draw(info);
		}


		public int GetExitID(CardinalDirection cardDir)
		{
			switch (cardDir)
			{
				case CardinalDirection.Up:
					return mTopID;
				case CardinalDirection.Right:
					return mRightID;
				case CardinalDirection.Down:
					return mBottomID;
				case CardinalDirection.Left:
					return mLeftID;
				default:
					break;
			}

			throw new NotImplementedException();
		}
	}
}
