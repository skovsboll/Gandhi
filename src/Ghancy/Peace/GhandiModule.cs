using Nancy;

namespace Ghancy.Peace
{
	public class GhandiModule : Nancy.NancyModule
	{
		public GhandiModule() : base("ghandi")
		{
			Get["page/{path}/preview"] = x => "Mjallo " + x.path;

			Get["page/{path}/edit"] = x => "Mjallo " + x.path;

			Post["page/{path}/edit"] = x => HttpStatusCode.Accepted;

			
		}
	}
}