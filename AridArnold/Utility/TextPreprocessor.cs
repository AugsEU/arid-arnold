namespace AridArnold
{
	static class TextPreprocessor
	{
		static MonoRandom sTextRandom = new MonoRandom();

		public static string Process(string text)
		{
			// Very slow, maybe find a better way?
			string outputStr = "";

			bool inEscapeSequence = false;
			string escapeSequenceStr = "";

			for (int i = 0; i < text.Length; i++)
			{
				char c = text[i];

				if(c == '{')
				{
					inEscapeSequence = true;
					escapeSequenceStr = "";
				}

				if (!inEscapeSequence)
				{
					outputStr += c;
				}
				else
				{
					escapeSequenceStr += c;
				}

				if(c == '}')
				{
					inEscapeSequence = false;
					outputStr += AddSubstituion(escapeSequenceStr);
				}
			}

			return outputStr;
		}

		static string AddSubstituion(string subCode)
		{
			subCode = subCode.Substring(1, subCode.Length - 2);
			switch (subCode)
			{
				case "GrillVogel":
					return GenerateGrillVogelName();
			}

			throw new NotImplementedException("Did not recognise substitution code:" + subCode);
		}


		static string GenerateGrillVogelName()
		{
			/*const*/ char[] kFirstNameStarts = { 'W', 'T', 'P', 'D', 'F', 'G', 'K', 'V', 'B', 'S' };
			/*const*/ char[] kSecondNameStarts = { 'W', 'R', 'T', 'Y', 'P', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'V', 'B', 'N', 'M' };

			int firstNameIdx = sTextRandom.GetIntRange(0, kFirstNameStarts.Length - 1);
			int secondNameIdx = sTextRandom.GetIntRange(0, kSecondNameStarts.Length - 1);

			return kFirstNameStarts[firstNameIdx] + "rill " + kSecondNameStarts[secondNameIdx] + "ogel";
		}
	}
}
