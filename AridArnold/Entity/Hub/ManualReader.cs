
namespace AridArnold
{
	internal class ManualReader : Entity
	{
		TextInfoBubble mInfoBubble;

		public ManualReader(Vector2 pos) : base(pos)
		{
			mPosition.Y -= 5.0f;
			mInfoBubble = new TextInfoBubble(mPosition + new Vector2(5.0f, -4.0f), "InGame.ReadManuals");
		}

		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/ManualReader");
		}

		public override void Update(GameTime gameTime)
		{
			mInfoBubble.Update(gameTime, IsPlayerNear());
			if(IsPlayerNear() && InputManager.I.KeyPressed(InputAction.Confirm))
			{
				ScreenManager.I.ActivateScreen(ScreenType.GameManuals);
			}
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTexture(info, mTexture, mPosition);
			mInfoBubble.Draw(info);
		}
	}
}
