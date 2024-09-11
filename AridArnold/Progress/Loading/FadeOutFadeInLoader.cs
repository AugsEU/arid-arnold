namespace AridArnold
{
	abstract class FadeOutFadeInLoader : LoadingSequence
	{
		#region rTypes

		protected enum LoadingState
		{
			FadeOut,
			DoEffects,
			PreLevelLoad,
			LoadingLevel,
			PostLevelLoad,
			FadeIn,
			Done
		}

		#endregion rTypes





		#region rMembers

		private LoadingState mLoadingState;
		protected FadeFX mFadeIn;
		protected FadeFX mFadeOut;
		int mNumUpdatesLoading;

		#endregion rMembers





		#region rInit

		public FadeOutFadeInLoader(int levelID) : base(levelID)
		{
			mLoadingState = LoadingState.FadeOut;
			mNumUpdatesLoading = 0;
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

				case LoadingState.DoEffects:
					UpdateEffects(gameTime);
					break;

				case LoadingState.PreLevelLoad:
					if(mNumUpdatesLoading == 0)
					{
						Main.LoadingScreenBegin();
					}
					else if(mNumUpdatesLoading >= 2)
					{
						mLoadingState = LoadingState.LoadingLevel;
						GC.Collect();
					}
					mNumUpdatesLoading++;
					break;

				case LoadingState.LoadingLevel:
					LevelLoadUpdate(gameTime);
					break;

				case LoadingState.PostLevelLoad:
					if(mNumUpdatesLoading == 1)
					{
						Main.LoadingScreenEnd();
					}
					else if (mNumUpdatesLoading >= 2)
					{
						mLoadingState = LoadingState.FadeIn;
					}
					mNumUpdatesLoading++;
					break;

				case LoadingState.FadeIn:
					UpdateFade(mFadeIn, gameTime);
					break;
			}
		}

		protected abstract void LevelLoadUpdate(GameTime gameTime);

		void UpdateFade(FadeFX fade, GameTime gameTime)
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

		protected void NotifyFinishedLoading()
		{
			mLoadingState = LoadingState.PostLevelLoad;
			mNumUpdatesLoading = 0;
		}

		protected virtual void UpdateEffects(GameTime gameTime)
		{
			// No effects...
			FinishEffectsStage();
		}

		protected void FinishEffectsStage()
		{
			mLoadingState = LoadingState.PreLevelLoad;
		}

		#endregion rUpdate





		#region rDraw

		public override void Draw(DrawInfo info)
		{
			FadeFX currFade = null;
			switch (mLoadingState)
			{
				case LoadingState.FadeOut:
					currFade = mFadeOut;
					break;
				case LoadingState.FadeIn:
					currFade = mFadeIn;
					break;
				case LoadingState.DoEffects:
					DrawEffects(info);
					break;
			}

			if (currFade is not null)
			{
				currFade.Draw(info);
			}
			else
			{
				MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, GameScreen.GAME_AREA_WIDTH, GameScreen.GAME_AREA_HEIGHT), Color.Black, DrawLayer.SequenceBG);
			}
		}

		protected virtual void DrawEffects(DrawInfo info)
		{
			// No effects.
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
			NotifyFinishedLoading();
		}

		protected abstract void DoLevelLoad();
	}
}

