using System.Web.Mvc;

namespace Gandhi.Sample.Areas.gandi
{
	public class gandiAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "gandhi";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
			    "gandhi_default",
			    "gandhi/{controller}/{action}/{id}",
			    new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
