﻿namespace AridArnold
{
	struct HighScore
	{
		public ulong mScore;
		public string mInitials;
		public bool mIsPlayer;
	}

	/// <summary>
	/// Represents a full cabinet with game.
	/// Handles scores and drawing the game within the cabinet.
	/// </summary>
	abstract class ArcadeCabinet
	{
		#region rTypes

		enum ArcadeCabScreen
		{
			TitleScreen,
			Gameplay,
			ScoreScreen
		}

		#endregion rTypes





		#region rConstants

		const int MAX_HIGH_SCORES = 8;
		const double CURSOR_BLINK_SPEED = 150.0;

		#endregion rConstants





		#region rMembers

		ArcadeGame mLoadedGame;
		List<HighScore> mHighScores;
		protected RenderTarget2D mGameOutput;
		ArcadeCabScreen mCurrScreen;
		Rect2f mScreenSpace;

		// Highscore submission
		ulong mPendingNewHighScore = 0;
		string mPendingNewInitials = "AAA";
		int mPendingCursorPos = 0;
		PercentageTimer mBlinkTimer;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Create arcade cab for specific game
		/// </summary>
		public ArcadeCabinet(ArcadeGame loadedGame, Rect2f screenSpace)
		{
			mLoadedGame = loadedGame;
			mHighScores = new List<HighScore>();
			mCurrScreen = ArcadeCabScreen.TitleScreen;
			mScreenSpace = screenSpace;
			mBlinkTimer = new PercentageTimer(CURSOR_BLINK_SPEED);
			mBlinkTimer.Start();
		}



		/// <summary>
		/// Reset the cabinet to default state
		/// </summary>
		public void ResetCabinet()
		{
			mCurrScreen = ArcadeCabScreen.TitleScreen;
			mPendingNewHighScore = 0;
			mLoadedGame.ResetGame();
		}

		protected abstract void SetDefaultScores();

		public void ResetScores()
		{
			mPendingNewHighScore = 0;
			mPendingCursorPos = 0;
			mHighScores.Clear();
			SetDefaultScores();
		}


		/// <summary>
		/// Add new high score, taking into account ordering.
		/// </summary>
		protected void AddHighScore(ulong score, string initials, bool player = false)
		{
			initials = initials.ToUpperInvariant();

			HighScore newScore = new HighScore();
			newScore.mScore = score;
			newScore.mInitials = initials;
			newScore.mIsPlayer = player;

			for(int i = 0; i < mHighScores.Count + 1; i++)
			{
				if (i == mHighScores.Count || newScore.mScore > mHighScores[i].mScore)
				{
					mHighScores.Insert(i, newScore);
					break;
				}
			}

			while(mHighScores.Count > MAX_HIGH_SCORES)
			{
				mHighScores.RemoveAt(mHighScores.Count - 1);
			}
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update cabinet
		/// </summary>
		public void Update(GameTime gameTime)
		{
			mBlinkTimer.Update(gameTime);

			if (mBlinkTimer.GetPercentageF() >= 1.0f)
			{
				mBlinkTimer.Reset();
			}

			switch (mCurrScreen)
			{
				case ArcadeCabScreen.TitleScreen:
					UpdateTitleScreen(gameTime);
					break;
				case ArcadeCabScreen.Gameplay:
					UpdateGameScreen(gameTime);
					break;
				case ArcadeCabScreen.ScoreScreen:
					UpdateScoreScreen(gameTime);
					break;
			}

			bool enteringScore = mCurrScreen == ArcadeCabScreen.ScoreScreen && mPendingNewHighScore > 0;
			bool gameAllowsQuit = mCurrScreen != ArcadeCabScreen.Gameplay || mLoadedGame.AllowQuit();

			bool quitPressed = InputManager.I.KeyPressed(InputAction.RestartLevel) || InputManager.I.KeyPressed(InputAction.Pause);

			if (quitPressed && gameAllowsQuit && !enteringScore)
			{
				mLoadedGame.ResetGame();
				MusicManager.I.StopMusic();
				ScreenManager.I.ActivateScreen(ScreenType.Game); // Go back to game screen.
			}
		}



		/// <summary>
		/// Update title screen.
		/// </summary>
		public void UpdateTitleScreen(GameTime gameTime)
		{
			if(InputManager.I.KeyPressed(InputAction.Confirm))
			{
				mCurrScreen = ArcadeCabScreen.Gameplay;
				mLoadedGame.ResetGame();
				string musicID = mLoadedGame.GetMusicID();
				MusicManager.I.RequestTrackPlay(musicID);

				SFXManager.I.EndAllSFX(120.0f);
			}
		}



		/// <summary>
		/// Update the main gameplay
		/// </summary>
		public void UpdateGameScreen(GameTime gameTime)
		{
			mLoadedGame.Update(gameTime);

			// Handle game over
			if (mLoadedGame.GetState() == ArcadeGame.ArcadeGameState.kGameOver)
			{
				mCurrScreen = ArcadeCabScreen.ScoreScreen;

				ulong newScore = mLoadedGame.GetScore();

				bool addNewHighScore = mHighScores.Count == 0 || mHighScores[mHighScores.Count - 1].mScore < newScore;
				
				if(addNewHighScore)
				{
					mPendingNewHighScore = newScore;
					mPendingNewInitials = "AAA";
					mPendingCursorPos = 0;
				}
			}
		}



		/// <summary>
		/// Update highscore screen.
		/// </summary>
		public void UpdateScoreScreen(GameTime gameTime)
		{
			if(mPendingNewHighScore > 0)
			{
				char selChar = mPendingNewInitials[mPendingCursorPos];

				if (InputManager.I.KeyPressed(InputAction.Confirm))
				{
					AddHighScore(mPendingNewHighScore, mPendingNewInitials, true);
					mPendingNewHighScore = 0;
				}
				else if (InputManager.I.KeyPressed(InputAction.ArnoldUp))
				{
					char newChar = MonoText.IncrementInAlphabet(selChar);
					MonoText.ReplaceChar(ref mPendingNewInitials, newChar, mPendingCursorPos);
				}
				else if (InputManager.I.KeyPressed(InputAction.ArnoldDown))
				{
					char newChar = MonoText.DecrementInAlphabet(selChar);
					MonoText.ReplaceChar(ref mPendingNewInitials, newChar, mPendingCursorPos);
				}
				else if (InputManager.I.KeyPressed(InputAction.ArnoldRight))
				{
					mPendingCursorPos++;
				}
				else if(InputManager.I.KeyPressed(InputAction.ArnoldLeft))
				{
					mPendingCursorPos--;
				}
				else if (InputManager.I.KeyPressed(InputAction.ArnoldRight))
				{
					mPendingCursorPos++;
				}

				mPendingCursorPos = (mPendingCursorPos+3) % 3;
			}
			else if (InputManager.I.KeyPressed(InputAction.Confirm))
			{
				mCurrScreen = ArcadeCabScreen.TitleScreen;
			}
		}

		#endregion rUpdate



		#region rDraw

		/// <summary>
		/// Draw the game frame to a texture
		/// </summary>
		public void DrawGameToTexture(DrawInfo info)
		{
			mGameOutput = mLoadedGame.DrawToRenderTarget(info);
		}



		/// <summary>
		/// Draw the cabinet
		/// </summary>
		/// <param name="info"></param>
		public void Draw(DrawInfo info)
		{
			DrawCabinetBG(info);

			switch (mCurrScreen)
			{
				case ArcadeCabScreen.TitleScreen:
					DrawTitleScreen(info);
					break;
				case ArcadeCabScreen.Gameplay:
					DrawGameScreen(info);
					break;
				case ArcadeCabScreen.ScoreScreen:
					DrawScoreScreen(info);
					break;
			}
		}



		/// <summary>
		/// Draw the actual game
		/// </summary>
		void DrawGameScreen(DrawInfo info)
		{
			float scale = mScreenSpace.Width / (float)mGameOutput.Width;

			MonoDraw.DrawTexture(info, mGameOutput, mScreenSpace.min, null, Color.White, 0.0f, Vector2.Zero, scale, SpriteEffects.None, DrawLayer.Background);
		}



		/// <summary>
		/// Draw the title screen
		/// </summary>
		protected abstract void DrawTitleScreen(DrawInfo info);



		/// <summary>
		/// Draw surrounding elements of cabinet
		/// </summary>
		protected abstract void DrawCabinetBG(DrawInfo info);



		/// <summary>
		/// Draw the score screen
		/// </summary>
		protected void DrawScoreScreen(DrawInfo info)
		{
			Vector2 textPos = mScreenSpace.min;
			textPos.X += mScreenSpace.Width * 0.5f;

			textPos.Y += mScreenSpace.Height * 0.1f;

			SpriteFont bigFont = FontManager.I.GetFont("Pixica-24");
			SpriteFont font = FontManager.I.GetFont("Pixica-12");

			string scoreTitle = LanguageManager.I.GetText("Arcade.HighScores");
			string insertCoin = mPendingNewHighScore > 0 ?  LanguageManager.I.GetText("Arcade.SubmitScore") : LanguageManager.I.GetText("Arcade.GoNext");

			// Draw title
			MonoDraw.DrawStringCentred(info, bigFont, textPos, Color.White, scoreTitle, DrawLayer.Background);

			// Draw footer
			Vector2 footerPos = textPos;
			footerPos.Y += mScreenSpace.Height * 0.8f;
			MonoDraw.DrawStringCentred(info, font, footerPos, Color.LightBlue, insertCoin, DrawLayer.Background);

			textPos.Y += mScreenSpace.Height * 0.2f;

			// Draw scores
			DrawScoreList(info, textPos.Y);

			// Draw high score submission
			if (mPendingNewHighScore > 0)
			{
				string newHS = LanguageManager.I.GetText("Arcade.NewHighScore") + mPendingNewHighScore.ToString();
				string insertName = LanguageManager.I.GetText("Arcade.SubmitName");

				footerPos.Y -= 50.0f;
				MonoDraw.DrawStringCentred(info, font, footerPos, Color.LightGray, newHS, DrawLayer.Background);

				footerPos.Y += 10.0f;
				MonoDraw.DrawStringCentred(info, font, footerPos, Color.LightGray, insertName, DrawLayer.Background);

				footerPos.Y += 10.0f;
				footerPos.X -= 15.0f;

				for (int x = 0; x < 3; x++)
				{
					string charToDraw = mPendingNewInitials[x].ToString();

					MonoDraw.DrawStringCentred(info, font, footerPos, Color.LightGray, charToDraw, DrawLayer.Background);

					if (x == mPendingCursorPos && mBlinkTimer.GetPercentage() <= 0.5f)
					{
						Vector2 cursorPos = footerPos + new Vector2(-5.0f, 5.0f);
						Rect2f cursorRect = new Rect2f(cursorPos, 10.0f, 3.0f);
						MonoDraw.DrawRect(info, cursorRect, Color.LightGray);
					}
					footerPos.X += 15.0f;
				}
			}
			else
			{
				string newScore = LanguageManager.I.GetText("Arcade.NewScore") + mLoadedGame.GetScore().ToString();

				footerPos.Y -= 50.0f;
				MonoDraw.DrawStringCentred(info, font, footerPos, Color.LightGray, newScore, DrawLayer.Background);
			}
		}


		protected void DrawScoreList(DrawInfo info, float y)
		{
			SpriteFont font = FontManager.I.GetFont("Pixica-24");

			Vector2 textPos = mScreenSpace.min;
			textPos.X += mScreenSpace.Width * 0.5f;
			textPos.Y = y;

			// Draw scores
			for (int i = 0; i < mHighScores.Count; i++)
			{
				Vector2 leftPos = textPos;
				Vector2 rightPos = textPos;

				leftPos.X -= mScreenSpace.Width * 0.15f;
				rightPos.X += mScreenSpace.Width * 0.15f;

				Color scoreCol = mHighScores[i].mIsPlayer ? Color.Gold : Color.LightGray;

				MonoDraw.DrawStringCentred(info, font, leftPos, scoreCol, mHighScores[i].mInitials, DrawLayer.Background);
				MonoDraw.DrawStringCentred(info, font, rightPos, scoreCol, mHighScores[i].mScore.ToString(), DrawLayer.Background);

				textPos.Y += 20.0f;
			}
		}

		#endregion rDraw





		#region rSerial

		/// <summary>
		/// Read from a binary file
		/// </summary>
		public void ReadBinary(BinaryReader br)
		{
			mHighScores.Clear();
			int numScores = br.ReadInt32();
			for(int i = 0; i < numScores; i++)
			{
				HighScore highScore = new HighScore();
				highScore.mScore = br.ReadUInt64();
				highScore.mInitials = br.ReadString();
				highScore.mIsPlayer = br.ReadBoolean();

				mHighScores.Add(highScore);
			}
		}



		/// <summary>
		/// Write to a binary file
		/// </summary>
		public void WriteBinary(BinaryWriter bw)
		{
			bw.Write((Int32)mHighScores.Count);
			for(int i = 0;i < mHighScores.Count;i++)
			{
				bw.Write((UInt64)mHighScores[i].mScore);
				bw.Write(mHighScores[i].mInitials);
				bw.Write(mHighScores[i].mIsPlayer);
			}
		}

		#endregion rSerial
	}
}
