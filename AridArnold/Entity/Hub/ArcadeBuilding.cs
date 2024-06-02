
namespace AridArnold
{
	class ArcadeBuilding : Entity
	{
		static Vector2 SIGN_OFFSET = new Vector2(-8.0f, -42.0f);
		static Vector2 INTERIOR_OFFSET = new Vector2(2.0f, 24.0f);
		static Vector2 PROMPT_OFFSET = new Vector2(156.0f, 47.0f);

		Animator mInteriorAnim;
		Animator mSignAnim;
		TextInfoBubble mEnterBubble;

		int mArcadeInteriorID;
		Vector2 mArnoldWarpPoint;

		public ArcadeBuilding(Vector2 pos, int toID, float arnoldX, float arnoldY) : base(pos)
		{
			mPosition.Y -= 70.0f;
			mEnterBubble = new TextInfoBubble(mPosition + PROMPT_OFFSET, "InGame.ArcadeEnter");

			mArcadeInteriorID = toID;
			mArnoldWarpPoint = new Vector2(arnoldX, arnoldY);
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

			mEnterBubble.Update(gameTime, mPlayerNear);
			if (mPlayerNear && InputManager.I.KeyPressed(AridArnoldKeys.Confirm))
			{
				EnterArcadeBuilding();
			}

			base.Update(gameTime);
		}



		/// <summary>
		/// Hacky function to enter the arcade buidling.
		/// </summary>
		void EnterArcadeBuilding()
		{
			HubDirectLoader toHub = new HubDirectLoader(mArcadeInteriorID);
			Arnold arnold = EntityManager.I.FindArnold(); // OK because in hub.

			if (arnold is not null)
			{
				toHub.AddPersistentEntities(arnold);
				arnold.SetPos(mArnoldWarpPoint);
				arnold.SetVelocity(Vector2.Zero);
			}

			CampaignManager.I.QueueLoadSequence(toHub);
		}


		public override Rect2f ColliderBounds()
		{
			Vector2 basePos = mPosition + new Vector2(155.0f, 66.0f);
			Vector2 maxPos = basePos + new Vector2(14.0f, 20.0f);
			Rect2f collider = new Rect2f(basePos, maxPos);

			return collider;
		}



		public override void Draw(DrawInfo info)
		{
			MonoDraw.DrawTextureDepth(info, mInteriorAnim.GetCurrentTexture(), mPosition + INTERIOR_OFFSET, DrawLayer.SubEntity);
			MonoDraw.DrawTextureDepth(info, mTexture, mPosition, DrawLayer.SubEntity);
			MonoDraw.DrawTextureDepth(info, mSignAnim.GetCurrentTexture(), mPosition + SIGN_OFFSET, DrawLayer.SubEntity);
			mEnterBubble.Draw(info);
		}
	}
}
