namespace AridArnold
{
	/// <summary>
	/// Data about the world(set of levels)
	/// </summary>
	struct WorldData
	{
		public string name;

		//Style
		public Color worldColor;

		//Levels
		public Level[] mLevels;

		public WorldData(string _name, Color _worldColor, Level[] levels)
		{
			name = _name;

			//Style
			worldColor = _worldColor;

			mLevels = levels;
		}
	}
}
