using System;
using System.Diagnostics;
using Nancy.Hosting.Self;

namespace Ghancy
{
	public static class WebServer
	{
		public static void Main()
		{
			string url = "http://localhost:8099";

			var host = new NancyHost(new Uri(url));
			host.Start();
			Process.Start(url+ "/ghandi/mayn");
			Console.WriteLine("Running. Press enter to stop.");
			Console.ReadLine();
			host.Stop();
		}
	}
}
