using System;

namespace AridArnold
{
	/// <summary>
	/// BG for mirror world. TO DO: Use ParticleRect instead
	/// </summary>
	internal class EMirrorBG : LayElement
	{
		const int kNumLeafRect = 30;

		LeafRect[] mLeafRects;

		public EMirrorBG(XmlNode rootNode) : base(rootNode)
		{
			Rectangle leafSizeRect = MonoParse.GetRectangle(rootNode);

			mLeafRects = new LeafRect[kNumLeafRect];

			for (int i = 0; i < mLeafRects.Length; i++)
			{
				mLeafRects[i] = new LeafRect(leafSizeRect, 2, 2, 1.9f);
			}
		}

		public override void Update(GameTime gameTime)
		{
			foreach (LeafRect rect in mLeafRects)
			{
				rect.Update(gameTime);
			}
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			foreach (LeafRect rect in mLeafRects)
			{
				rect.Draw(info);
			}
			base.Draw(info);
		}
	}
}
