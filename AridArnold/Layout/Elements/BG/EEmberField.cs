
using System;

namespace AridArnold
{
	internal class EEmberField : LayElement
	{
		const int kNumSnowRect = 20;

		EmberRect[] mEmberRects;

		public EEmberField(XmlNode rootNode) : base(rootNode)
		{
			Rectangle emberSizeRect = MonoParse.GetRectangle(rootNode);

			mEmberRects = new EmberRect[kNumSnowRect];

			for (int i = 0; i < mEmberRects.Length; i++)
			{
				mEmberRects[i] = new EmberRect(emberSizeRect, 8, 8);
			}
		}


		public override void Update(GameTime gameTime)
		{
			foreach (EmberRect rect in mEmberRects)
			{
				rect.Update(gameTime);
			}
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			foreach (EmberRect rect in mEmberRects)
			{
				rect.Draw(info);
			}
			base.Draw(info);
		}
	}
}
