namespace AridArnold
{
	internal class HubTransitionLoader : LoadingSequence
	{
		#region rTypes

		enum LoadingState
		{
			FadeOut,
			LoadingLevel,
			FadeIn,
			Done
		}

		#endregion rTypes





		#region rMembers

		LoadingState mLoadingState;
		protected ScreenFade mFadeIn;
		protected ScreenFade mFadeOut;

		#endregion rMembers





		#region rInit

		public HubTransitionLoader(int levelID) : base(levelID)
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
					LoadAsHubLevel();
					mLoadingState=LoadingState.FadeIn;
					break;

				case LoadingState.FadeIn:
					UpdateFade(mFadeIn, gameTime);
					break;
			}
		}

		void UpdateFade(ScreenFade fade, GameTime gameTime)
		{
			if(fade is null || fade.Finished())
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

			if(currFade is not null)
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
}
