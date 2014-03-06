using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TimeSheet.Models;
using TwitterBootstrapMVC;

namespace TimeSheet
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Bootstrap.Configure();
        }

        protected void Session_Start(object sender, EventArgs e)
        {
#if DEBUG
            var user = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
            if (user == "GUYLISTER3546\\guy")
                user = "lister.g.1";
#else
            var user = Thread.CurrentPrincipal.Identity.Name;
#endif
            //scheduleDB _db = new scheduleDB();
            HttpContext.Current.Session["user"] = user;
            //HttpContext.Current.Session["authority"] = _db.Fetch<User>(string.Format(Models.User.get_role, user)).FirstOrDefault();
        }
    }
}