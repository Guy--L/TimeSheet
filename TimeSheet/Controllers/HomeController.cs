using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public static partial class HtmlExtensions
    {
        public static MvcHtmlString IdFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            return MvcHtmlString.Create(htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(ExpressionHelper.GetExpressionText(expression)));
        }
    }

    public class HomeController : Controller
    {
        private void dbExec(string q)
        {
            tsDB db = new tsDB();

            try
            {
                db.Execute(q);
            }
            catch (Exception e)
            {
                Exception frame = new Exception(q, e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(frame);
            }
        }

        public HomeController()
        {
            if (Week.partners != null)
                return;

            using (tsDB db = new tsDB())
            {
                Week.sites = db.Fetch<Site>("");
                Week.partners = db.Fetch<Partner>("");
                Week.internalNumbers = db.Fetch<InternalNumber>("");
                Week.workAreas = db.Fetch<WorkArea>("");
                Week.costCenters = db.Fetch<CostCenter>("");
            }
            Week.NonDemand = Week.partners.Where(p => p._Partner == "RDSS").Select(q => q.PartnerId).Single();
        }

        private static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }

        public ActionResult Index(int? id)
        {
            tsDB db = new tsDB();

            string[] worker = Sheet.user.Split('\\');
            Worker emp = db.FirstOrDefault<Worker>("where ionname = @0", worker[worker.Length-1]);
            if (emp == null)
                return RedirectToAction(Sheet.user.Replace('\\','_'), "Contact");

            Week.descriptions = db.Fetch<Description>("where workerid = @0", emp.WorkerId);
            Week.customers = db.Fetch<Customer>("where workerid = @0 or workerid = 0", emp.WorkerId);

            Sheet ts = new Sheet() { employee = emp };

            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            DateTime init = DateTime.Today;

            if (!id.HasValue)
                id = Session["CurrentWeek"] as int?;

            if (id.HasValue)
            {
                Session["CurrentYear"] = Session["CurrentYear"] ?? DateTime.Today.Year;
                init = FirstDateOfWeek((int)Session["CurrentYear"], id.Value);
                ts.weekNumber = id.Value;
            }
            else
            {
                ts.weekNumber = calendar.GetWeekOfYear(init, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            }

            ts.hours = db.Fetch<Week>(string.Format(Week.lst_week, emp.WorkerId, ts.weekNumber));
            ts.Stats = Week.Stats(ts.hours);

            int nextSunday = init.DayOfWeek == DayOfWeek.Sunday ? 0 : (7 - (int) init.DayOfWeek);
            init = init.AddDays(nextSunday);
            ts.sunday = init.ToString("MM/dd/yyyy");
            Sheet.headers = Enumerable.Range(-6, 7).Select(n => init.AddDays(n)).ToList();

            Session["CurrentWeek"] = ts.weekNumber;
            Session["CurrentYear"] = init.Year;

            return View(ts);
        }

        public ActionResult Edit(string id, int id2)
        {
            tsDB db = new tsDB();
            Hours hrs = new Hours();
            try
            {
                var sheet = db.Fetch<Week>(string.Format(Week.get_hours, id, id2));
                hrs.normal = sheet.Where(a => !a.IsOvertime).SingleOrDefault();
                hrs.overtime = sheet.Where(a => a.IsOvertime).SingleOrDefault();
                hrs.Headers = Sheet.headers;

                return PartialView("_Hours", hrs);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return null;
            }
        }

        public ActionResult Create()
        {
            Hours hrs = new Hours();
            return PartialView("_Hours", hrs);
        }

        [HttpPost]
        public ActionResult Save(Sheet hours)
        {
            tsDB _db = new tsDB();

            if (!string.IsNullOrWhiteSpace(hours.NewDescription))
            {
                hours.normal.DescriptionId = _db.ExecuteScalar<int>(Models.Description.Save(hours.employee.WorkerId, hours.NewDescription));
                hours.overtime.DescriptionId = hours.normal.DescriptionId;
            }
            if (!string.IsNullOrWhiteSpace(hours.NewCustomer))
            {
                hours.normal.CustomerId = _db.ExecuteScalar<int>(Models.Customer.Save(hours.employee.WorkerId, hours.NewCustomer));
                hours.overtime.CustomerId = hours.normal.CustomerId;
            }

            hours.normal.WeekNumber = hours.weekNumber;
            hours.overtime.WeekNumber = hours.weekNumber;
            hours.normal.NewRequest = hours.NewRequest;         // model binding didn't work for checkbox so we revert to this

            dbExec(hours.Save());

            return RedirectToAction("Index", new { id = hours.weekNumber });
        }

        [HttpPost]
        public ActionResult Submit(int id, int week)
        {
            dbExec(Week.Submit(id, week));
            return RedirectToAction("Index", new { id = week });
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
