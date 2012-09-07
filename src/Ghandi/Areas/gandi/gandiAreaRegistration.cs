using System.Web.Mvc;

namespace Ghandi.Areas.gandi
{
	public class gandiAreaRegistration : AreaRegistration
	{
		public override string AreaName
		{
			get
			{
				return "gandi";
			}
		}

		public override void RegisterArea(AreaRegistrationContext context)
		{
			context.MapRoute(
			    "gandi_default",
			    "gandi/{controller}/{action}/{id}",
			    new { action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
