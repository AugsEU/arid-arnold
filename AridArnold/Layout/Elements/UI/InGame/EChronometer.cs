namespace AridArnold
{
	class EChronometer : UIPanelBase
	{
		static Vector2 GLOBAL_TIMER_OFFSET = new Vector2(95.0f, 110.0f);

		SpriteFont mFont;
		SpriteFont mBigFont;
		int mFrames;

		public EChronometer(XmlNode rootNode, Layout parent) : base(rootNode, "UI/InGame/ChronometerBG", parent)
		{
			mFont = FontManager.I.GetFont("PixicaMicro", 24);
			mBigFont = FontManager.I.GetFont("PixicaMicro", 24, true);
		}

		public override void Draw(DrawInfo info)
		{
			base.Draw(info);

			string globalTitle = LanguageManager.I.GetText("InGame.GlobalTime");
			string globalTimeStr = MonoText.GetTimeTextFromFrames(InputManager.I.GetNumberOfInputFrames());

			Vector2 pos = GetPosition() + GLOBAL_TIMER_OFFSET;
			DrawTimer(info, globalTitle, globalTimeStr, pos, PANEL_WHITE);
			pos.Y -= 36.0f;

			// Draw out level timer.
			string toBeatTitle = LanguageManager.I.GetText("InGame.LevelPB");
			string toBeatStr = GhostManager.I.GetTimeToBeat();
			DrawTimer(info, toBeatTitle, toBeatStr, pos, Color.Olive);

			pos.Y -= 36.0f;

			string timeTitle = LanguageManager.I.GetText("InGame.LevelTime");
			string timeStr = GhostManager.I.GetTime();
			DrawTimer(info, timeTitle, timeStr, pos, PANEL_WHITE);
		}


		public void DrawTimer(DrawInfo info, string title, string time, Vector2 pos, Color color)
		{
			if(time.Length == 0)
			{
				return;
			}

			MonoDraw.DrawStringCentred(info, mFont, pos, color, title, GetDepth());
			pos.Y += 12.0f;
			MonoDraw.DrawStringCentred(info, mFont, pos, color, time, GetDepth());
		}
	}
}
