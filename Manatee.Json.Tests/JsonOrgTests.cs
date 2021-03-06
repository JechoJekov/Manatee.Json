﻿using System;
using System.Net;
using NUnit.Framework;

namespace Manatee.Json.Tests
{
	[TestFixture]
	public class JsonOrgTests
	{
		[Test]
		public void JsonCheckerTest()
		{
			var text = new WebClient().DownloadString("http://www.json.org/JSON_checker/test/pass1.json");
			Console.WriteLine($"{text}\n");

			// Just make sure the parser doesn't fail.
			var json = JsonValue.Parse(text);

			Console.WriteLine(json);
		}
	}
}
