using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {

            if (Sheet.partners != null)
                return;

            tsDB db = new tsDB();

            Sheet.partners = db.Fetch<Partner>("").ToDictionary(p => p.PartnerId, p => p._Partner);
            Sheet.sites = db.Fetch<Site>("").ToDictionary(s => s.SiteId, s => s._Site);
            Sheet.orders = db.Fetch<CostCenter>("").ToDictionary(c => c.CostCenterId, c => c._CostCenter);
            Sheet.workAreas = db.Fetch<WorkArea>("").ToDictionary(c => c.WorkAreaId, c => c._WorkArea);
            Sheet.accounts = db.Fetch<InternalNumber>("").ToDictionary(i => i.InternalNumberId, i => i.InternalOrder);
        }

        public ActionResult Index()
        {
            tsDB db = new tsDB();

            Worker emp = db.FirstOrDefault<Worker>("where ionname = @0", "steiner.ma");

            Sheet.descriptions = db.Fetch<Description>("where workerid = @0", emp.WorkerId).ToDictionary(d=>d.DescriptionId, d=>d._Description);
            Sheet.customers = db.Fetch<Customer>("where workerid = @0", emp.WorkerId).ToDictionary(c => c.CustomerId, c => c.CustomerName);

            Sheet ts = new Sheet() { employee = emp };

            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            DateTime init = DateTime.Today.AddDays(-14);
            ts.weekNumber = calendar.GetWeekOfYear(
                init,
                CalendarWeekRule.FirstDay,
                DayOfWeek.Monday
            );
            int nextSunday = init.DayOfWeek == DayOfWeek.Sunday ? 0 : (7 - (int) init.DayOfWeek);
            ts.sunday = init.AddDays(nextSunday).ToString("mm/dd/yyyy");

            if (ts.User == "")
                ts.User = "Guy Lister";
            ts.edit = new Week();

            return View(ts);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
