namespace HorsesAndGun
{
	internal class DiceQueue
	{
		const int NUM_DICE = 8;

		Dice[] mDiceArray;

		public DiceQueue()
		{
			mDiceArray = new Dice[NUM_DICE];

			for (int i = 0; i < mDiceArray.Length; i++)
			{
				mDiceArray[i] = new Dice();
			}
		}

		public Dice PopDice()
		{
			Dice dice = new Dice(mDiceArray[0]);

			for (int i = 0; i < mDiceArray.Length - 1; i++)
			{
				mDiceArray[i].Value = mDiceArray[i + 1].Value;
			}

			mDiceArray[NUM_DICE - 1].Roll();

			return dice;
		}

		public Dice PeekDice(int index)
		{
			return mDiceArray[index];
		}

		public int GetDiceNum()
		{
			return mDiceArray.Length;
		}
	}
}
