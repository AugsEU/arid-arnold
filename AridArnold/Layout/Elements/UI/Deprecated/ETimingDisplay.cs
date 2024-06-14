namespace AridArnold
{
	class ETimingDisplay : LayElement
	{
		static Vector2 LEVEL_TIMER_OFFSET = new Vector2(95.0f, 25.0f);

		SpriteFont mFont;
		int mFrames;

		public ETimingDisplay(XmlNode rootNode) : base(rootNode)
		{
			mFont = FontManager.I.GetFont("PixicaMicro-24");
		}

		public override void Draw(DrawInfo info)
		{




			string timeStr = GhostManager.I.GetTime();
			string toBeatStr = GhostManager.I.GetTimeToBeat();

			Vector2 pos = GetPosition() + LEVEL_TIMER_OFFSET;
			MonoDraw.DrawStringCentred(info, mFont, pos, GetColor(), "Time:", GetDepth());
			pos.Y += 12.0f;
			MonoDraw.DrawStringCentred(info, mFont, pos, GetColor(), timeStr, GetDepth());
			if (toBeatStr != "")
			{
				pos.Y += 24.0f;
				MonoDraw.DrawStringCentred(info, mFont, pos, Color.Olive, "Time to beat:", GetDepth());
				pos.Y += 12.0f;
				MonoDraw.DrawStringCentred(info, mFont, pos, Color.Olive, toBeatStr, GetDepth());
			}

			base.Draw(info);
		}
	}
}
