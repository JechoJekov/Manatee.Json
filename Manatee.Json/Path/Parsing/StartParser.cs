﻿namespace Manatee.Json.Path.Parsing
{
	internal class StartParser : IJsonPathParser
	{
		public bool Handles(string input, int index)
		{
			return input[index] == '$' || input[index] == '@';
		}

		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path != null) return "Start token not valid in the middle of path.";

			path = new JsonPath();
			index++;
			return null;
		}
	}
}
