
namespace AridArnold
{
	internal class EBestRunsDisplay : LayElement
	{
		const int MAX_LINES = 10;
		const float COL_WIDTH = 160.0f;

		SpriteFont mFont;
		SpriteFont mBigFont;
		List<(string, int)> mRunTimes;

		public EBestRunsDisplay(XmlNode rootNode) : base(rootNode)
		{
			mFont = FontManager.I.GetFont("Pixica-12");
			mBigFont = FontManager.I.GetFont("Pixica-24");
			PopulateRunTimes();
		}

		void PopulateRunTimes()
		{
			mRunTimes = new List<(string, int)>();

			try
			{
				string baseDirectory = "data/ghostData/";
				string[] files = Directory.GetFiles(baseDirectory, "*.ght", SearchOption.AllDirectories);

				foreach (string fileDir in files)
				{
					string relativePath = Path.GetRelativePath(baseDirectory, fileDir);
					string fileName = Path.GetFileNameWithoutExtension(fileDir);

					GhostFile ghostFile = new GhostFile(relativePath);
					ghostFile.Load();
					mRunTimes.Add((fileName, ghostFile.GetFrameCount()));
				}
			}
			catch
			{

			}
		}

		string[] GetScoreBoardStr()
		{
			string output = "";
			int totalTime = 0;

			foreach((string, int) runTime in mRunTimes)
			{
				string timingStr = GhostManager.I.FrameTimeToString(runTime.Item2);
				output += runTime.Item1 + " - " + timingStr + "\n";
				totalTime += runTime.Item2;
			}

			output += "Total: " + GhostManager.I.FrameTimeToString(totalTime) + "\n";

			// Terrible
			return output.Split("\n");
		}

		public override void Draw(DrawInfo info)
		{
			if(mRunTimes.Count == 0) return;

			Color strColor = Color.Gray;

			if(mRunTimes.Count == 31)
			{
				strColor = Color.Gold;
			}

			string[] board = GetScoreBoardStr();
			int numCols = (int)MathF.Ceiling((float)board.Length / MAX_LINES);
			int numRows = board.Length / numCols;

			Vector2 pos = GetPosition();
			pos.X -= (numCols-1) * COL_WIDTH / 2.0f;

			for (int c = 0; c < numCols; c++)
			{
				string colStr = "";
				for(int r = 0; r < numRows; r++)
				{
					int index = c * numRows + r;
					if(index >= board.Length)
					{
						break;
					}
					colStr += board[index] + "\n";
				}

				pos = MonoMath.Round(pos);
				MonoDraw.DrawStringCentred(info, mFont, pos, strColor, colStr);
				pos.X += COL_WIDTH;
			}

			pos = GetPosition();
			pos.Y -= 130.0f;

			MonoDraw.DrawStringCentred(info, mBigFont, pos, strColor, "Best times");
		}
	}
}
