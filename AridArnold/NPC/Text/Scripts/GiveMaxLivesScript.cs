
namespace AridArnold
{
	class GiveMaxLivesScript : OneShotScript
	{
		int mWaterID;
		int mWaterNeeded;
		string mOutro; // Text to display after quenching.

		string mNotEnoughWater; // Sorry you don't have enough


		public GiveMaxLivesScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
			// Make sure args are correct
			MonoDebug.Assert(args.Length == 3);

			mWaterID = int.Parse(args[0], CultureInfo.InvariantCulture);
			mWaterNeeded = int.Parse(args[1], CultureInfo.InvariantCulture);

			mOutro = LanguageManager.I.GetText(args[2]);

			// Always the same string
			mNotEnoughWater = LanguageManager.I.GetText("NPC.Witch.NoWater");
		}

		public override void Update(GameTime gameTime)
		{
			// Accept automatically if no water required.
			if (mWaterNeeded == 0)
			{
				ForceConfirm();
			}
			base.Update(gameTime);
		}

		protected override void DoOneShot()
		{
			if (HasDoneThisAlready())
			{
				return;
			}

			uint numWaterInCollection = CollectableManager.I.GetNumCollected(CollectableCategory.WaterBottle);

			if(numWaterInCollection >= mWaterNeeded)
			{
				// Spend water
				UInt16 waterType = CollectableManager.GetCollectableID(CollectableCategory.WaterBottle);
				CollectableManager.I.IncPermanentCount(waterType, -mWaterNeeded);
				
				// Give life
				CampaignManager.I.GiveMaxLifeLevel();

				// Mark flag
				FlagsManager.I.SetFlag(FlagCategory.kWaterCollected, (uint)mWaterID, true);

				// Say thanks.
				GetSmartTextBlock().AppendTextAtHead(mOutro);
			}
			else
			{
				// Nope!
				GetSmartTextBlock().AppendTextAtHead(mNotEnoughWater);
			}
		}

		bool HasDoneThisAlready()
		{
			return FlagsManager.I.CheckFlag(FlagCategory.kWaterCollected, (uint)mWaterID);
		}

		public override bool HaltText()
		{
			return base.HaltText() && !HasDoneThisAlready();
		}


		public override bool IsFinished()
		{
			return base.IsFinished() || HasDoneThisAlready();
		}
	}
}
