
namespace AridArnold
{
	class EVitalSigns : UIPanelBase
	{
		struct LifeIconPack
		{
			public Texture2D mActiveIcon;
			public Texture2D mInactiveIcon;

			public LifeIconPack(string path)
			{
				mActiveIcon = MonoData.I.MonoGameLoad<Texture2D>("Arnold/ArnoldStand");
				mInactiveIcon = MonoDraw.MakeTextureGreyscale(Main.GetGraphicsDevice(), mActiveIcon);
			}
		}

		LifeIconPack mFullLifeYoung;
		LifeIconPack mFullLifeOld;
		Texture2D mEmptyLife;

		public EVitalSigns(XmlNode rootNode) : base(rootNode, "UI/InGame/VitalsSignsBG")
		{
			mFullLifeYoung = new LifeIconPack("Arnold/ArnoldStand");
			mFullLifeOld = new LifeIconPack("Arnold/ArnoldOldStand");
			mEmptyLife = MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/EmptyLifeArnold");
		}


		public override void Update(GameTime gameTime)
		{

			base.Update(gameTime);
		}



		public override void Draw(DrawInfo info)
		{
			base.Draw(info);

			bool isActive = CampaignManager.I.CanLoseLives();
			int currLives = CampaignManager.I.GetLives();
			int maxLives = CampaignManager.I.GetMaxLives();

			Vector2 drawPos = GetPosition();
			drawPos += new Vector2(130.0f, 10.0f);

			LifeIconPack? pack = GetCurrentIconPack();
			Texture2D fullIcon = isActive ? pack.Value.mActiveIcon : pack.Value.mInactiveIcon;

			for(int i = 0; i < maxLives; i++)
			{
				if(i < currLives)
				{
					DrawLifeFullIcon(info, drawPos, fullIcon);
				}
				else
				{
					MonoDraw.DrawTextureDepth(info, mEmptyLife, drawPos, GetDepth());
				}

				drawPos.Y += 32.0f;
			}
		}


		void DrawLifeFullIcon(DrawInfo info, Vector2 pos, Texture2D icon)
		{
			Rectangle sourceRect = new Rectangle(0, 0, 10, 7);
			MonoDraw.DrawTexture(info, icon, pos, sourceRect, Color.White, 0.0f, Vector2.Zero, 4.0f, SpriteEffects.None, GetDepth());
		}

		LifeIconPack? GetCurrentIconPack()
		{
			int age = TimeZoneManager.I.GetCurrentPlayerAge();

			if(age == 0)
			{
				return mFullLifeYoung;
			}

			return mFullLifeYoung;
		}
	}
}
