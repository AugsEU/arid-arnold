
namespace AridArnold
{
	class GiveCurseScript : OneShotScript
	{
		public GiveCurseScript(SmartTextBlock parentBlock, string[] args) : base(parentBlock, args)
		{
		}

		protected override void DoOneShot()
		{
			if (HasDoneThisAlready())
			{
				return;
			}

			CurseFlagTypes curse = GetRandomCurseBlessing();

			FlagsManager.I.SetFlag(FlagCategory.kCurses, (UInt32)curse, true);
			FlagsManager.I.SetFlag(FlagCategory.kCurses, (UInt32)CurseFlagTypes.kCurseGiven, true);

			switch (curse)
			{
				case CurseFlagTypes.kBlessingLives:
				case CurseFlagTypes.kBlessingMoney:
					SFXManager.I.PlaySFX(AridArnoldSFX.OneUp, 0.4f);
					break;
				case CurseFlagTypes.kCurseMoney:
				case CurseFlagTypes.kCurseLives:
					SFXManager.I.PlaySFX(AridArnoldSFX.ArnoldDeath, 0.4f);
					CampaignManager.I.RefreshCurrLives();
					break;
			}

			// Say thanks.
			string curseText = LanguageManager.I.GetText(GetCurseStringID(curse));
			GetSmartTextBlock().AppendTextAtHead(curseText);

			// Save game! Sorry, you are stuck with this blessing or curse.
			SaveManager.I.SaveProfile();
		}


		static CurseFlagTypes GetRandomCurseBlessing()
		{
			if (CampaignManager.I.IsSpeedrunMode())
			{
				// Speedrunners are never lucky
				return CurseFlagTypes.kCurseLives;
			}

			MonoRandom rng = RandomManager.I.GetWorld();
			
			int min = 1;
			int max = (int)CurseFlagTypes.kMaxCurse - 1;

			return (CurseFlagTypes)rng.GetIntRange(min, max);
		}


		static string GetCurseStringID(CurseFlagTypes curseType)
		{
			switch (curseType)
			{
				case CurseFlagTypes.kMaxCurse:
				case CurseFlagTypes.kCurseGiven:
					return "";
				case CurseFlagTypes.kBlessingLives:
					return "NPC.DevilHead.BlessingLives";
				case CurseFlagTypes.kBlessingMoney:
					return "NPC.DevilHead.BlessingMoney";
				case CurseFlagTypes.kCurseLives:
					return "NPC.DevilHead.CurseLives";
				case CurseFlagTypes.kCurseMoney:
					return "NPC.DevilHead.CurseMoney";
			}

			throw new NotImplementedException();
		}

		bool HasDoneThisAlready()
		{
			return FlagsManager.I.CheckFlag(FlagCategory.kCurses, (UInt32)CurseFlagTypes.kCurseGiven);
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
