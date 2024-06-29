namespace AridArnold
{
	/// <summary>
	/// Different categories for flags.
	///    First 32 bytes of flag is it's category. 
	///    The 32bits after are the "implementation", up to whoever is settting
	///    the flags to differentiate between flags in the same category.
	/// </summary>
	enum FlagCategory : UInt32
	{
		kWaterCollected = 0,
		kCurses,
		kKeyItems,
		kUnlockedGreatGate,
		kMaxFlagCategory
	}



	/// <summary>
	/// Different types of curses/blessings
	/// </summary>
	enum CurseFlagTypes : UInt32
	{
		kCurseGiven = 0,
		kBlessingLives,
		kBlessingMoney,
		kCurseLives,
		kCurseMoney,
		kMaxCurse
	}



	/// <summary>
	/// Different types of key items for progression.
	/// </summary>
	enum KeyItemFlagType : UInt32
	{
		kGatewayKey,
		kCoinPurse,
		kRippedJeans,
		kSerpentToken,
		kHorseToken,
		kDemonToken,
		kMaxKeyItems,
	}


	static class FlagTypeHelpers
	{
		public static Texture2D LoadKeyItemTexture(KeyItemFlagType itemFlag)
		{
			switch (itemFlag)
			{
				case KeyItemFlagType.kGatewayKey:
					return MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/GreatKey");
				case KeyItemFlagType.kCoinPurse:
					return MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/CoinPurse");
				case KeyItemFlagType.kRippedJeans:
					return MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/GenerationalJeans");
				case KeyItemFlagType.kSerpentToken:
					return MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/SerpentToken");
				case KeyItemFlagType.kHorseToken:
					return MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/HorseToken");
				case KeyItemFlagType.kDemonToken:
					return MonoData.I.MonoGameLoad<Texture2D>("UI/InGame/DemonToken");
			}

			throw new NotImplementedException();
		}


		public static string GetKeyItemNameID(KeyItemFlagType itemFlag)
		{
			switch (itemFlag)
			{
				case KeyItemFlagType.kGatewayKey:
					return "UI.KeyItem.GatewayKey";
				case KeyItemFlagType.kCoinPurse:
					return "UI.KeyItem.CoinPurse";
				case KeyItemFlagType.kRippedJeans:
					return "UI.KeyItem.RippedJeans";
				case KeyItemFlagType.kSerpentToken:
					return "UI.KeyItem.SerpentToken";
				case KeyItemFlagType.kHorseToken:
					return "UI.KeyItem.HorseToken";
				case KeyItemFlagType.kDemonToken:
					return "UI.KeyItem.DemonToken";
			}

			throw new NotImplementedException();
		}
	}
}
