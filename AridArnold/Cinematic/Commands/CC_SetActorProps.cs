
namespace AridArnold
{
	internal class CC_SetActorProps : CC_ActorCommand
	{
		bool mSetTex;
		Texture2D mDrawTexture;
		Vector2? mPosition;
		DrawLayer? mDrawLayer;
		SpriteEffects? mEffects = null;


		public CC_SetActorProps(XmlNode cmdNode, GameCinematic parent) : base(cmdNode, parent)
		{
			string texturePath = MonoParse.GetString(cmdNode["tex"]);

			mSetTex = texturePath.Length > 0;

			if (mSetTex)
			{
				mDrawTexture = texturePath == "null" ? null : MonoData.I.MonoGameLoad<Texture2D>(texturePath);
			}

			mPosition = cmdNode["x"] is not null ? MonoParse.GetVector(cmdNode) : null;
			mDrawLayer = cmdNode["layer"] is not null ? MonoAlg.GetEnumFromString<DrawLayer>(MonoParse.GetString(cmdNode["layer"])) : null;

			XmlNode facingNode = cmdNode["facing"];

			if (facingNode is not null)
			{
				if (MonoParse.GetString(facingNode) == "left")
				{
					mEffects = SpriteEffects.FlipHorizontally;
				}
				else
				{
					mEffects = SpriteEffects.None;
				}
			}
		}

		public override void Update(GameTime gameTime, int currentFrame)
		{
			if (mPosition.HasValue)
			{
				mTargetActor.SetPosition(mPosition.Value);
			}

			if (mDrawLayer.HasValue)
			{
				mTargetActor.SetDrawLayer(mDrawLayer.Value);
			}

			if (mSetTex)
			{
				mTargetActor.SetDrawTexture(mDrawTexture);
			}

			if (mEffects.HasValue)
			{
				mTargetActor.SetSpriteEffect(mEffects.Value);
			}
		}
	}
}
