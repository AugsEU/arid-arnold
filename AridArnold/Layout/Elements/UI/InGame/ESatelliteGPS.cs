namespace AridArnold
{
	class ESatelliteGPS : UIPanelBase
	{
		static Vector2 WORLD_TITLE_OFFSET = new Vector2(95.0f, 159.0f);
		static Vector2 COMPASS_OFFSET = new Vector2(95.0f, 75.0f);

		SpriteFont mFont;
		SpriteFont mSmallFont;

		Texture2D mCompassCentre;
		Texture2D mCompassArrow;

		Texture2D mSequenceTileOn;
		Texture2D mSequenceTileOff;
		Texture2D mSequenceArrowBegin;
		Texture2D mSequenceArrowMiddle;
		Texture2D mSequenceArrowLE;
		Texture2D mSequenceArrowLB;
		Texture2D mSequenceArrowEnd;

		public ESatelliteGPS(XmlNode rootNode, Layout parent) : base(rootNode, "UI/InGame/SatelliteBG", parent)
		{
			mFont = FontManager.I.GetFont("Pixica", 24, true);
			mSmallFont = FontManager.I.GetFont("Pixica", 12, true);

			mCompassCentre = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/CompassCentre");
			mCompassArrow = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/CompassArrow");

			mSequenceTileOn = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceTileOn");
			mSequenceTileOff = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceTileOff");
			mSequenceArrowBegin = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceArrowStart");
			mSequenceArrowMiddle = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceArrowMid");
			mSequenceArrowLE = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceArrowLE");
			mSequenceArrowLB = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceArrowLB");
			mSequenceArrowEnd = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SequenceArrowEnd");
		}

		public override void Draw(DrawInfo info)
		{
			base.Draw(info);

			List<Level> levelSequence = CampaignManager.I.GetLevelSequence();
			Level currLevel = CampaignManager.I.GetCurrentLevel();

			DrawWorldTitle(info, currLevel);
			if (currLevel is HubLevel hubLevel)
			{
				DrawCompass(info, hubLevel);
			}
			else if(levelSequence is not null)
			{
				DrawLevelSequence(info, levelSequence, currLevel);
			}
		}

		void DrawWorldTitle(DrawInfo info, Level currLevel)
		{
			if (currLevel is not null)
			{
				string worldName = currLevel.GetTheme().GetDisplayName();
				Vector2 pos = GetPosition() + WORLD_TITLE_OFFSET;
				MonoDraw.DrawStringCentred(info, mFont, pos, PANEL_GOLD, worldName, DrawLayer.Bubble);
				if(currLevel.CanLoseLives())
				{
					pos.Y -= 20.0f;
					string levelName = Util.LevelIDToString(currLevel.GetID());
					MonoDraw.DrawStringCentred(info, mSmallFont, pos, PANEL_GOLD, levelName, DrawLayer.Bubble);
				}
			}
		}

		void DrawCompass(DrawInfo info, HubLevel hubLevel)
		{
			Vector2 position = COMPASS_OFFSET + GetPosition();
			position.X -= 0.5f * mCompassCentre.Width;
			position.Y -= 0.5f * mCompassCentre.Height;

			position = MonoMath.Round(position);

			MonoDraw.DrawTextureDepth(info, mCompassCentre, position, GetDepth());

			int fromID = hubLevel.GetID();
			CampaignMetaData meta = CampaignManager.I.GetCampaignMetaData();

			int topID = hubLevel.GetExitID(CardinalDirection.Up);
			int rightID = hubLevel.GetExitID(CardinalDirection.Right);
			int downID = hubLevel.GetExitID(CardinalDirection.Down);
			int leftID = hubLevel.GetExitID(CardinalDirection.Left);

			if (topID != 0 && !meta.IsTransitionHidden(fromID, topID))
			{
				MonoDraw.DrawTexture(info, mCompassArrow, position + new Vector2(-3.0f, 3.0f),
					null, Color.White, -MathF.PI * 0.5f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth());
			}

			if (rightID != 0 && !meta.IsTransitionHidden(fromID, rightID))
			{
				MonoDraw.DrawTexture(info, mCompassArrow, position + new Vector2(17.0f, -3.0f),
					null, Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth());
			}

			if (downID != 0 && !meta.IsTransitionHidden(fromID, downID))
			{
				MonoDraw.DrawTexture(info, mCompassArrow, position + new Vector2(23.0f, 17.0f),
					null, Color.White, MathF.PI * 0.5f, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth());
			}

			if (leftID != 0 && !meta.IsTransitionHidden(fromID, leftID))
			{
				MonoDraw.DrawTexture(info, mCompassArrow, position + new Vector2(3.0f, 23.0f),
					null, Color.White, MathF.PI, Vector2.Zero, 1.0f, SpriteEffects.None, GetDepth());
			}
		}

		void DrawLevelSequence(DrawInfo info, List<Level> levelSequence, Level currLevel)
		{
			const int MAX_LINE_LEN = 5;

			if(levelSequence.Count <= MAX_LINE_LEN)
			{
				DrawSequenceLine(info, GetPosition() + COMPASS_OFFSET, 0, levelSequence.Count, levelSequence, currLevel);
			}
			else
			{
				// HACK: Manually do 2 lines
				DrawSequenceLine(info, GetPosition() + COMPASS_OFFSET + new Vector2(0.0f, -20.0f), 0, MAX_LINE_LEN - 1, levelSequence, currLevel);
				DrawSequenceLine(info, GetPosition() + COMPASS_OFFSET + new Vector2(0.0f, 20.0f), MAX_LINE_LEN - 1, levelSequence.Count, levelSequence, currLevel);
			}
		}

		void DrawSequenceLine(DrawInfo info, Vector2 basePos, int start, int end, List<Level> levelSequence, Level currLevel)
		{
			int count = end - start;
			Texture2D startLineTex = start == 0 ? mSequenceArrowBegin : mSequenceArrowLB;
			Texture2D endLineTex = end == levelSequence.Count ? mSequenceArrowEnd : mSequenceArrowLE;

			float calcWidth = startLineTex.Width + count * (mSequenceTileOn.Width + 12.0f) + endLineTex.Width - 14.0f;
			Vector2 position = basePos;
			position.X -= calcWidth * 0.5f;
			position = MonoMath.Round(position);

			// Draw line
			MonoDraw.DrawTextureDepth(info, startLineTex, position, GetDepth());
			position.X += startLineTex.Width - 1.0f;

			for (int i = start; i < end; i++)
			{
				Level selectedLevel = levelSequence[i];
				Texture2D tileBase = object.ReferenceEquals(currLevel, selectedLevel) ? mSequenceTileOn : mSequenceTileOff;

				Texture2D levelIcon = LevelSequenceInfoBubble.GetIconForLevel(selectedLevel.GetAuxData().GetLevelType());
				MonoDraw.DrawTextureDepth(info, levelIcon, position + new Vector2(4.0f, -10.0f), GetDepth());

				MonoDraw.DrawTextureDepth(info, tileBase, position, GetDepth());
				position.X += tileBase.Width - 1.0f;

				if (i != end - 1)
				{
					MonoDraw.DrawTextureDepth(info, mSequenceArrowMiddle, position, GetDepth());
					position.X += mSequenceArrowMiddle.Width - 1.0f;
				}
			}

			MonoDraw.DrawTextureDepth(info, endLineTex, position, GetDepth());
		}
	}
}
