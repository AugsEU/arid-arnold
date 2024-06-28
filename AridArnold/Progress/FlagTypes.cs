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
	enum KeyItemFlagTypes : UInt32
	{
		kGatewayKey,
	}
}
