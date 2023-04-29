using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AridArnold.Screens.Fade
{
	class ScreenWipe : ScreenFade
	{
		CardinalDirection mDirection;

		public ScreenWipe(CardinalDirection direction, float speed, bool forwards) : base(speed, forwards)
		{
			mDirection = direction;
		}

		protected override void DrawAtTime(DrawInfo info, float time)
		{
			switch (mDirection)
			{
				case CardinalDirection.Down:
					MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, GameScreen.GAME_AREA_WIDTH, (int)(GameScreen.GAME_AREA_HEIGHT * time)), Color.Black, DrawLayer.Front);
					break;
				case CardinalDirection.Up:
					MonoDraw.DrawRectDepth(info, new Rectangle(0, (int)(GameScreen.GAME_AREA_HEIGHT * (1.0f - time)), GameScreen.GAME_AREA_WIDTH, GameScreen.GAME_AREA_HEIGHT), Color.Black, DrawLayer.Front);
					break;
				case CardinalDirection.Right:
					MonoDraw.DrawRectDepth(info, new Rectangle(0, 0, (int)(GameScreen.GAME_AREA_WIDTH * time), GameScreen.GAME_AREA_HEIGHT), Color.Black, DrawLayer.Front);
					break;
				case CardinalDirection.Left:
					MonoDraw.DrawRectDepth(info, new Rectangle((int)(GameScreen.GAME_AREA_HEIGHT * (1.0f - time)), 0, GameScreen.GAME_AREA_WIDTH, GameScreen.GAME_AREA_HEIGHT), Color.Black, DrawLayer.Front);
					break;
			}
		}



	}
}
