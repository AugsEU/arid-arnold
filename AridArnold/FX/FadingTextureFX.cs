
namespace AridArnold
{
	internal class FadingTextureFX : FX
	{
		PercentageTimer mFadeTimer;
		Texture2D mTexture;
		Vector2 mPosition;
		DrawLayer mDrawLayer;

		public FadingTextureFX(double fadeTime, Texture2D texture, Vector2 position, DrawLayer layer)
		{
			mFadeTimer = new PercentageTimer(fadeTime);
			mTexture = texture;
			mPosition = position;
			mDrawLayer = layer;
			mFadeTimer.Start();
		}


		public override void Draw(DrawInfo info)
		{
			float opacity = 1.0f - mFadeTimer.GetPercentageF();
			Color color = new Color(opacity, opacity, opacity, opacity);
			MonoDraw.DrawTexture(info, mTexture, mPosition, null, color, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.None, mDrawLayer);
		}

		public override bool Finished()
		{
			return mFadeTimer.GetPercentageF() >= 1.0f;
		}

		public override void Update(GameTime gameTime)
		{
		}
	}

	/// <summary>
	/// Bespoke texture fader for time shift effect
	/// </summary>
	internal class TimeShiftFaderFX : FX
	{
		FadingTextureFX mTextureFader;

		public TimeShiftFaderFX()
		{
			Camera gameCam = CameraManager.I.GetCamera(CameraManager.CameraInstance.GameAreaCamera);
			MonoDebug.Assert(gameCam is not null && gameCam.GetPrevRenderTarget() is not null);

			Texture2D previousFrame = MonoDraw.MemCopyTexture(Main.GetGraphicsDevice(), gameCam.GetPrevRenderTarget());
			mTextureFader = new FadingTextureFX(ShiftTimeCameraMove.TIME_TO_ROTATE * 80.0, previousFrame, Vector2.Zero, DrawLayer.Front);
		}

		public override void Draw(DrawInfo info)
		{
			mTextureFader.Draw(info);
		}

		public override bool Finished()
		{
			return mTextureFader.Finished();
		}

		public override void Update(GameTime gameTime)
		{
		}
	}
}
