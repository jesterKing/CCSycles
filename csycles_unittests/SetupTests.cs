using NUnit.Framework;
using System;
using ccl;
using System.IO;

namespace csycles_unittests
{
	[SetUpFixture]
	public class SetupTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			var path = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) ?? "";
			var userpath = Path.Combine(path, "userpath");

			CSycles.path_init(path, userpath);
			CSycles.initialise();
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			CSycles.shutdown();
		}
	}
}