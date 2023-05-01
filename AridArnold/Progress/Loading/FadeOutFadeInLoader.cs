namespace AridArnold
{
	abstract class FadeOutFadeInLoader : LoadingSequence
	{
		#region rTypes

		protected enum LoadingState
		{
			FadeOut,
			LoadingLevel,
			FadeIn,
			Done
		}

		#endregion rTypes





		#region rMembers

		protected LoadingState mLoadingState;
		protected ScreenFade mFadeIn;
		protected ScreenFade mFadeOut;

		#endregion rMembers





		#region rInit

		public FadeOutFadeInLoader(int levelID) : base(levelID)
		{
			mLoadingState = LoadingState.FadeOut;
		}

		#endregion rInit





		#region rUpdate

		public override void Update(GameTime gameTime)
		{
			switch (mLoadingState)
			{
				case LoadingState.FadeOut:
					UpdateFade(mFadeOut, gameTime);
					break;

				case LoadingState.LoadingLevel:
					// Good time to collect garbage
					GC.Collect();
					LevelLoadUpdate(gameTime);
					break;

				case LoadingState.FadeIn:
					UpdateFade(mFadeIn, gameTime);
					break;
			}
		}

		protected abstract void LevelLoadUpdate(GameTime gameTime);

		void UpdateFade(ScreenFade fade, GameTime gameTime)
		{
			if (fade is null || fade.Finished())
			{
				mLoadingState++;
				return;
			}

			fade.Update(gameTime);
		}


		public override bool Finished()
		{
			return mLoadingState == LoadingState.Done;
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			ScreenFade currFade = null;
			switch (mLoadingState)
			{
				case LoadingState.FadeOut:
					currFade = mFadeOut;
					break;
				case LoadingState.FadeIn:
					currFade = mFadeIn;
					break;
			}

			if (currFade is not null)
			{
				currFade.Draw(info);
			}
			else
			{
				MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, GameScreen.GAME_AREA_WIDTH, GameScreen.GAME_AREA_HEIGHT), Color.Black, DrawLayer.Front);
			}
		}

		#endregion rDraw
	}

	abstract class FadeOutFadeInSimpleLoader : FadeOutFadeInLoader
	{
		public FadeOutFadeInSimpleLoader(int levelID) : base(levelID)
		{
		}

		protected override void LevelLoadUpdate(GameTime gameTime)
		{
			DoLevelLoad();
			mLoadingState = LoadingState.FadeIn;
		}

		protected abstract void DoLevelLoad();
	}
}

