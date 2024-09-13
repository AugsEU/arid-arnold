
using static AridArnold.EMField;

namespace AridArnold
{
	class SequenceWinLoader : ReturnToHubSuccessLoader
	{
		/// <summary>
		/// Information to display
		/// </summary>
		struct WinInfoStatistic
		{
			public string mName;
			public Color mColor;
			public string mValue;

			public WinInfoStatistic(string name, Color col, string value)
			{
				mName = name;
				mColor = col;
				mValue = value;
			}
		}

		const double STAGGER_TIME = 450.0;

		MonoTimer mTextStaggerTimer;
		List<WinInfoStatistic> mStats;

		public SequenceWinLoader() : base()
		{
			mTextStaggerTimer = new MonoTimer();
			mStats = new List<WinInfoStatistic>();
		}

		protected override void UpdateEffects(GameTime gameTime)
		{
			if(!mTextStaggerTimer.IsPlaying())
			{
				mTextStaggerTimer.Start();
				PopulateWinStats();
				CameraManager.I.ResetAllCameras();
			}

			mTextStaggerTimer.Update(gameTime);

			if (InputManager.I.AnyGangPressed(BindingGang.SysConfirm))
			{
				FinishEffectsStage();
			}
		}

		void PopulateWinStats()
		{
			// Water
			{
				int waterDiff = CollectableManager.I.GetSequenceEndDiff(CollectableCategory.WaterBottle);
				if (waterDiff != 0)
				{
					string waterTitle = LanguageManager.I.GetText("UI.Sequence.WaterGet");
					mStats.Add(new WinInfoStatistic(waterTitle, new Color(59, 100, 226), MonoText.IntToDiff(waterDiff)));
				}
			}

			// Key
			{
				int keyDiff = CollectableManager.I.GetSequenceEndDiff(CollectableCategory.Key);
				if (keyDiff != 0)
				{
					string keyTitle = LanguageManager.I.GetText("UI.Sequence.KeyGet");
					mStats.Add(new WinInfoStatistic(keyTitle, new Color(224, 160, 33), MonoText.IntToDiff(keyDiff)));
				}
			}

			// Money
			{
				ushort moneyID = CampaignManager.I.GetCurrCoinID();
				int moneyDiff = CollectableManager.I.GetSequenceEndDiff(moneyID);
				if (moneyDiff != 0)
				{
					string moneyTitle = LanguageManager.I.GetText("UI.Sequence.Money");
					string moneyStr = string.Format("{0}$", MonoText.IntToDiff(moneyDiff));

					mStats.Add(new WinInfoStatistic(moneyTitle, new Color(127, 127, 127), moneyStr));
				}
			}

			// Item refund
			{
				Item currItem = ItemManager.I.GetActiveItem();
				if (currItem is not null && currItem.RefundAtSequenceEnd())
				{
					int price = currItem.GetPrice();

					if (price > 0)
					{
						string refundTitle = LanguageManager.I.GetText("UI.Sequence.ItemRefund");
						string retundMoneyStr = string.Format("{0}$", MonoText.IntToDiff(currItem.GetPrice()));

						mStats.Add(new WinInfoStatistic(refundTitle, new Color(127, 127, 127), retundMoneyStr));
					}
				}
			}
		}

		protected override void DrawEffects(DrawInfo info)
		{
			if (!mTextStaggerTimer.IsPlaying())
			{
				// Hack to make sure setup is done.
				return;
			}

			SpriteFont bigFont = FontManager.I.GetFont("Pixica", 36, true);
			SpriteFont font = FontManager.I.GetFont("Pixica", 24, false);
			Color textColor = new Color(127, 127, 127);

			Vector2 pos = new Vector2(GameScreen.GAME_AREA_WIDTH * 0.5f, 50.0f);

			// Title
			string titleText = LanguageManager.I.GetText("UI.Sequence.SequenceComplete");
			MonoDraw.DrawStringCentredShadow(info, bigFont, pos, textColor, titleText, DrawLayer.Front);
			Rectangle dividingLine = new Rectangle(16, 76, GameScreen.GAME_AREA_WIDTH - 32, 2);
			MonoDraw.DrawRectShadow(info, dividingLine, textColor, textColor * 0.2f, 2.0f, DrawLayer.Front);
			pos.Y += 80.0f;

			// Stats
			DrawAllStats(info, pos);

			pos.Y = 500.0f;
			string nextText = LanguageManager.I.GetText("UI.Sequence.GoNext");
			MonoDraw.DrawStringCentredShadow(info, font, pos, textColor, nextText, DrawLayer.Front);

			base.DrawEffects(info);
		}

		void DrawAllStats(DrawInfo info, Vector2 pos)
		{
			const float STAT_OFFSET = 30.0f;
			for (int i = 0; i < mStats.Count; i++)
			{
				if(mTextStaggerTimer.GetElapsedMs() < (i + 1) * (STAGGER_TIME))
				{
					break;
				}

				DrawStat(info, pos, mStats[i]);
				pos.Y += STAT_OFFSET;
			}
		}

		void DrawStat(DrawInfo info, Vector2 pos, WinInfoStatistic stat)
		{
			Vector2 statOffset = new Vector2(110.0f, 0.0f);

			SpriteFont font = FontManager.I.GetFont("Pixica", 24, false);
			Color textColor = new Color(127, 127, 127);

			MonoDraw.DrawStringCentredShadow(info, font, pos - statOffset, stat.mColor, stat.mName, DrawLayer.Front);
			MonoDraw.DrawStringCentredShadow(info, font, pos + statOffset, textColor, stat.mValue, DrawLayer.Front);
		}
	}
}
