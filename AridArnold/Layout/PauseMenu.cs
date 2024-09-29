using static AridArnold.MainMenuScreen;

namespace AridArnold
{
	/// <summary>
	/// Class that is the pause menu.
	/// </summary>
	class PauseMenu
	{
		#region rTypes

		enum PauseSubMenu
		{
			kHub,
			kLevel,
			kOptions,
			kNumAreas
		}

		#endregion rTypes





		#region rConstants

		static Color BACKDROP_COLOR = new Color(0, 0, 0, 220);

		#endregion rConstants





		#region rMembers

		bool mOpen;
		Layout[] mMenus;
		PauseSubMenu mCurrentMenu;

		#endregion rMembers





		#region rInit

		/// <summary>
		/// Initialise pause menu
		/// </summary>
		public PauseMenu()
		{
			mMenus = new Layout[(int)PauseSubMenu.kNumAreas];
			mMenus[(int)PauseSubMenu.kHub]			= new Layout("Layouts/PauseMenuHub.mlo");
			mMenus[(int)PauseSubMenu.kLevel]		= new Layout("Layouts/PauseMenuLevel.mlo");
			mMenus[(int)PauseSubMenu.kOptions]		= new Layout("Layouts/PauseMenuOptions.mlo");
			mCurrentMenu = PauseSubMenu.kHub;
			mOpen = false;
		}

		#endregion rInit





		#region rUpdate

		/// <summary>
		/// Update pause menu
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if(!mOpen)
			{
				return;
			}

			GetCurrentSubMenu().Update(gameTime);
			ProcessAllMessages();
		}

		#endregion rUpdate





		#region rDraw

		/// <summary>
		/// Draw the pause menu
		/// </summary>
		public void Draw(DrawInfo info)
		{
			if(!mOpen)
			{
				return;
			}

			MonoDraw.DrawRectDepth(info, Screen.SCREEN_RECTANGLE, BACKDROP_COLOR, DrawLayer.PauseMenu);
			GetCurrentSubMenu().Draw(info);
		}

		#endregion rDraw





		#region rMessages

		/// <summary>
		/// Process all messages from the layout
		/// </summary>
		void ProcessAllMessages()
		{
			Layout currentSubMenu = GetCurrentSubMenu();
			ElementMsg? currentMsg = currentSubMenu.PopMessage();
			while (currentMsg != null)
			{
				ProcessMessage(currentMsg.Value);
				currentMsg = currentSubMenu.PopMessage();
			}
		}



		/// <summary>
		/// Process single message
		/// </summary>
		void ProcessMessage(ElementMsg msg)
		{
			string msgHeader = msg.mHeader;
			string msgStr = msg.mMessage;

			switch (msgHeader)
			{
				case "go": // Go to sub-screen
					PauseSubMenu newSubMenu = MonoEnum.GetEnumFromString<PauseSubMenu>(msgStr);
					GoToSubMenu(newSubMenu);
					break;
				case "re": // return to hub or start of game
					if (msgStr == "hub")
					{
						Close();
						CampaignManager.I.QueueLoadSequence(new ReturnToHubFailureLoader());
					}
					else if(msgStr == "game")
					{
						Close();
						// CampaignManager.I.RestartCampaign();
					}
					break;
				case "cl":
					Close();
					break;
				case "sc": // Load new screen
					Close();
					SaveManager.I.SaveProfile();
					ScreenType newScreenType = MonoEnum.GetEnumFromString<ScreenType>(msgStr);
					ScreenManager.I.ActivateScreen(newScreenType);
					break;
			}
		}

		#endregion rMessages





		#region rUtil

		/// <summary>
		/// Get which sub menu we are on
		/// </summary>
		private Layout GetCurrentSubMenu()
		{
			return mMenus[(int)mCurrentMenu];
		}



		/// <summary>
		/// New sub menu
		/// </summary>
		private void GoToSubMenu(PauseSubMenu newArea)
		{
			// Bit of a hack but we don't allow going to the hub area in a level so we translate it here...
			if (newArea == PauseSubMenu.kHub)
			{
				AuxData.LevelType currLevelType = CampaignManager.I.GetCurrentLevelType();

				if (currLevelType == AuxData.LevelType.Hub)
				{
					newArea = PauseSubMenu.kHub;
				}
				else
				{
					newArea = PauseSubMenu.kLevel;
				}
			}

			mCurrentMenu = newArea;

			switch (newArea)
			{
				case PauseSubMenu.kLevel:
				case PauseSubMenu.kHub:
					GetCurrentSubMenu().InstantSetSelectedElement("resumeBtn");
					break;
				case PauseSubMenu.kOptions:
					GetCurrentSubMenu().InstantSetSelectedElement("fullScreenOpt");
					break;
			}
		}



		/// <summary>
		/// Open the menu
		/// </summary>
		public void Open()
		{
			if (mOpen) return;

			mOpen = true;
			SFXManager.I.EndAllSFX(320.0f);
			SFXManager.I.PlaySFX(AridArnoldSFX.MenuSelect, 0.2f);
			GoToSubMenu(PauseSubMenu.kHub);

			// Prevent pause buffering.
			GhostManager.I.StopRecording();
		}



		/// <summary>
		/// Close the menu
		/// </summary>
		public void Close()
		{
			if (!mOpen) return;

			// Save in case the global settings have changed.
			SaveManager.I.SaveGlobalSettings();

			mOpen = false;
		}



		/// <summary>
		/// Is the pause menu open?
		/// </summary>
		public bool IsOpen()
		{
			return mOpen;
		}

		#endregion rUtil
	}
}
