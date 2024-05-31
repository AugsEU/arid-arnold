
namespace AridArnold
{
	class ArcadeBuilding : Entity
	{
		static Vector2 SIGN_OFFSET = new Vector2(-8.0f, -42.0f);
		static Vector2 INTERIOR_OFFSET = new Vector2(2.0f, 24.0f);

		Animator mInteriorAnim;
		Animator mSignAnim;
		TextInfoBubble mEnterBubble;

		public ArcadeBuilding(Vector2 pos) : base(pos)
		{
			mPosition.Y -= 70.0f;
		}


		public override void LoadContent()
		{
			mTexture = MonoData.I.MonoGameLoad<Texture2D>("Arcade/Building/Exterior");
			mInteriorAnim = new Animator(Animator.PlayType.Repeat,
				("Arcade/Building/Interior1", 0.4f),
				("Arcade/Building/Interior2", 0.4f));

			mSignAnim = new Animator(Animator.PlayType.Repeat,
				("Arcade/Building/Sign1", 0.2f),
				("Arcade/Building/Sign2", 0.2f),
				("Arcade/Building/Sign3", 0.2f));

			mSignAnim.Play();
			mInteriorAnim.Play();
		}


		public override void Update(GameTime gameTime)
		{
			mInteriorAnim.Update(gameTime);
			mSignAnim.Update(gameTime);
			base.Update(gameTime);
		}

		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mInteriorAnim.GetCurrentTexture(), mPosition + INTERIOR_OFFSET, DrawLayer.SubEntity);
			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.SubEntity);
			MonoDraw.DrawTextureDepth(info, mSignAnim.GetCurrentTexture(), mPosition + SIGN_OFFSET, DrawLayer.SubEntity);
		}
	}
}
