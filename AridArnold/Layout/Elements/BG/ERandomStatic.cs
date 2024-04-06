
namespace AridArnold
{
	internal class ERandomStatic : LayElement
	{
		MonoTimer mScreenRefreshTimer;
		float mScreenRefreshRate;
		Rectangle mScreenSpan;
		Color mPixelColour;

		public ERandomStatic(XmlNode rootNode) : base(rootNode)
		{
			int width = MonoParse.GetInt(rootNode["width"]);
			int height = MonoParse.GetInt(rootNode["height"]);
			mPixelColour = MonoParse.GetColor(rootNode["on"], new Color(71, 59, 67));

			mScreenRefreshRate = MonoParse.GetFloat(rootNode["refresh"], 500.0f);

			Vector2 pos = GetPosition();

			mScreenSpan = new Rectangle((int)pos.X, (int)pos.Y, width, height);

			mScreenRefreshTimer = new MonoTimer();
			mScreenRefreshTimer.Start();
		}

		public override void Draw(DrawInfo info)
		{
			int randSeed = (int)(GetPosition().X + GetPosition().Y + mScreenRefreshTimer.GetElapsedMs() / mScreenRefreshRate);
			MonoRandom rand = new MonoRandom(randSeed);

			Point drawPoint = Point.Zero;

			int maxInc = Math.Max(mScreenSpan.Width / 2, 1);
			bool draw = false;
			while (drawPoint.Y < mScreenSpan.Height)
			{
				int rectSize = rand.GetIntRange(1, maxInc);

				if (drawPoint.X + rectSize > mScreenSpan.Width)
				{
					rectSize = mScreenSpan.Width - drawPoint.X;

					if (rectSize <= 0)
					{
						drawPoint.X = 0;
						drawPoint.Y += 1;
						continue;
					}
				}

				Rectangle drawRect = new Rectangle(mScreenSpan.X + drawPoint.X, mScreenSpan.Y + drawPoint.Y, rectSize, 1);
				if (draw)
				{
					MonoDraw.DrawRectDepth(info, drawRect, mPixelColour, GetDepth());
				}

				draw = !draw;

				drawPoint.X += rectSize;
			}
		}
	}
}
