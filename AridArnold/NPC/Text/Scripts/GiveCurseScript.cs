
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

			// Say thanks.
			string curseText = LanguageManager.I.GetText(GetCurseStringID(curse));
			GetSmartTextBlock().AppendTextAtHead(curseText);
		}


		static CurseFlagTypes GetRandomCurseBlessing()
		{
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
