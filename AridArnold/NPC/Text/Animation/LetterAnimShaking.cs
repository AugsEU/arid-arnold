using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold
{
	internal class LetterAnimShaking : LetterAnimColor
	{
		int mFrame;
		int mIntensity;

		public LetterAnimShaking(int intensity, int frame, Color color) : base(color)
		{
			mIntensity = intensity;
			mFrame = frame;
		}

		public override Vector2 GetPosition(float lifeTime)
		{
			float angle = MathF.PI * lifeTime / (mFrame * 16.6f);
			float randX = MathF.Sin(1.2432f * angle) * 1.2f;
			float randY = MathF.Cos(1.8547f * angle);

			float intensity = (mIntensity / 10.0f);

			return new Vector2(randX, randY) * intensity;
		}
	}
}
