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
		public string wallTexture;
		public string platformTexture;

		//Levels
		public Level[] mLevels;

		public WorldData(string _name, Color _worldColor, string _wallTexture, string _platformTexture, Level[] levels)
		{
			name = _name;

			//Style
			worldColor = _worldColor;
			wallTexture = _wallTexture;
			platformTexture = _platformTexture;

			mLevels = levels;
		}
	}
}
