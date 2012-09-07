using System.Web;
using HtmlAgilityPack;
using Fizzler.Systems.HtmlAgilityPack;

namespace Ghancy.Peace
{
	public class GhandiHttpHandler : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			var htmlDocument = new HtmlDocument();
			htmlDocument.Load("Love\\OneColumns");
			var main = htmlDocument.DocumentNode.QuerySelector("div[role=main]");
			main.InnerHtml = "I've been replaced, it seems!";
			htmlDocument.Save(context.Response.OutputStream);
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}
