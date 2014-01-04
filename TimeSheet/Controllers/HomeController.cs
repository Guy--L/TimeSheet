using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using Postal;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public class HomeController : Controller
    {
        /// <summary>
        /// Convenience wrapper to run queries with initialization and error handling
        /// </summary>
        /// <param name="q">Sql statement</param>
        private void dbExec(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return;

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

        /// <summary>
        /// Static constructor to get static lists and set constants
        /// </summary>
        static HomeController()
        {
            Week.GetLists();
        }


        /// <summary>
        /// Touchup '0:00' to '00:00' and '' to '00:00'
        /// </summary>
        /// <param name="timespan"></param>
        /// <returns></returns>
        private string d(string timespan)
        {
            if (string.IsNullOrWhiteSpace(timespan)) return "00:00";
            if (timespan.Length == 4) return "0" + timespan;
            return timespan;    // not compleat
        }


        /// <summary>
        /// If user types in date or clicks the week rewind or advance buttons, find week and recall it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PickDate(string id)
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;

            try
            {
                var newday = DateTime.Parse(id);
                var wk = new ISO_8601(newday);
                var newmon = wk.week;
                return RedirectToAction("Index", new { id = newmon });
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("PickDate threw up", e));
                var week = Session["CurrentWeek"] as int?;
                if (!week.HasValue)
                {
                    var wk = new ISO_8601(DateTime.Today);
                    week = wk.week;
                }
                return RedirectToAction("Index", new { id = week });
            }
        }

        public void SubmitEmail(Worker w, Sheet s)
        {
            if (string.IsNullOrWhiteSpace(w.ManagerIon))
                return;

            FileStream fs = new FileStream(Server.MapPath(@"~/Content/TimeSheet.xls"), FileMode.Open, FileAccess.Read);  // Getting the complete workbook... 
            HSSFWorkbook wb = new HSSFWorkbook(fs, true);                
            


            MemoryStream ms = new MemoryStream();
            wb.Write(ms);

            dynamic email = new Email("TimeSheet");
            email.Attach(new Attachment(ms, "ts" + w.LastName + ".xls", "application/vnd.ms-excel"));
            email.From = ConfigurationManager.AppSettings["sentfrom"];
            email.To = w.ManagerIon;
            email.Employee = w.FirstName + " " + w.LastName;
            email.Ending = s.sunday;
            email.Demand = s.Stats[0];
            email.NonDemand = s.Stats[1];
            email.OverTime = s.Stats[2];
            email.Total = s.Stats[3];
            email.Send();
        }

        /// <summary>
        /// "Landing" page for timesheet
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Index(int? id)
        {
            tsDB db = new tsDB();

            string usr = Session["user"] as string;
            string who = "where ionname = @0";
            if (string.IsNullOrWhiteSpace(usr))
                return RedirectToAction("Contact", "User was not found");

            if (char.IsDigit(usr[0]))
                who = "where workerid = @0";
            else
            {
                string[] worker = usr.ToString().Split('\\');
                usr = worker[worker.Length - 1];
            }
            Worker emp = db.FirstOrDefault<Worker>(Worker.worker + who, usr);
            if (emp == null)
                return RedirectToAction("Contact", "User was not found");

            Session["WorkerId"] = emp.WorkerId;

            Sheet ts = new Sheet() { 
                employee = emp, 
                User = Session["user"].ToString(), 
                IsAdmin = emp.IsAdmin,
                IsManager = emp.IsManager,
                Impersonating = (Session["admin"] != null)
            };

            Calendar calendar = CultureInfo.InvariantCulture.Calendar;
            DateTime init = DateTime.Today;

            if (!id.HasValue)
                id = Session["CurrentWeek"] as int?;

            Session["CurrentYear"] = Session["CurrentYear"] ?? DateTime.Today.Year;         // problems at year boundaries
            ts.year = (int)Session["CurrentYear"];

            if (id.HasValue)
            {
                if (id.Value > 100)
                {
                    ts.year++;
                    id -= 100;
                }
                if (id.Value < 0)
                {
                    ts.year--;
                    id = -id;
                }
                init = Hrs.FirstDateOfWeek(ts.year, id.Value);
                ts.weekNumber = id.Value;
            }
            else
            {
                var week = new ISO_8601(DateTime.Today);
                ts.weekNumber = week.week;
            }

            ts.hours = Week.Get(emp.WorkerId, ts.weekNumber, ts.year);
            ts.Stats = Week.Stats(ts.hours);

            ts.numbers = ts.hours[0].internalNumbers;
            ts.centers = ts.hours[0].costCenters;

            ts.Submitted = ts.hours.Any(d => d.Submitted.HasValue);

            var idsEntered = ts.hours.Select(d => d.DescriptionId);
            var idsPrior = ts.hours[0].descriptions.Where(d => !idsEntered.Contains(d.DescriptionId) && d.IsActive);
            ts.CarryOver = idsPrior.Select(d => db.Fetch<Week>(string.Format(Week.get_prior, emp.WorkerId, d.DescriptionId)).SingleOrDefault()).ToList();
            ts.CarryOver.ForEach(c => { 
                if (c == null) return; 
                c.Year = -1;
                c.descriptions = ts.hours[0].descriptions;
                c.customers = ts.hours[0].customers;
                c.internalNumbers = ts.hours[0].internalNumbers;
                c.costCenters = ts.hours[0].costCenters;
            });

            int nextSunday = init.DayOfWeek == DayOfWeek.Sunday ? 0 : (7 - (int) init.DayOfWeek);
            init = init.AddDays(nextSunday);
            ts.sunday = init.ToString("MM/dd/yyyy");
            ts.Headers = Enumerable.Range(-6, 7).Select(n => init.AddDays(n)).ToList();

            Session["CurrentWeek"] = ts.weekNumber;
            Session["CurrentYear"] = init.Year;

            var submitted = TempData["submit"] as bool?;
            if (submitted.HasValue && submitted.Value)
                SubmitEmail(emp, ts);                
            
            return View(ts);
        }

            
        /// <summary>
        /// Mark this description as inactive (to hide it from empty timesheet rows)
        /// </summary>
        /// <param name="id">Description Id</param>
        /// <param name="id2">Week Number</param>
        /// <returns></returns>
        public ActionResult InActivate(int id, int id2)
        {
            dbExec(string.Format(Description.InActivate(id)));
            return RedirectToAction("Index", new { id = id2 });
        }


        /// <summary>
        /// Mark this description as active again (because it was selected from pulldown)
        /// </summary>
        /// <param name="id">Description Id</param>
        /// <param name="id2">Week Number</param>
        /// <returns></returns>
        public ActionResult Activate(int id, int id2)
        {
            dbExec(string.Format(Description.Activate(id)));
            return RedirectToAction("Index", new { id = id2 });
        }

        /// <summary>
        /// Save changes or additions made using _Hours modal form while adding new list items as needed
        /// </summary>
        /// <param name="hours">Flat object containing normal and overtime hours</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Hrs hours)
        {
            hours.AddIfNew();           // add new records for Customer, Description, Cost Center or Internal Number
            dbExec(hours.Save());
            return RedirectToAction("Index", new { id = hours.WeekNumber });
        }

        [HttpGet]
        public ActionResult Edit(int id, int id2)
        {
            try
            {
                var workerid = Session["WorkerId"] as int?;
                if (workerid == null || !workerid.HasValue)
                    throw new Exception("No workerid when creating blank hours from description");

                tsDB db = new tsDB();
                var sheet = db.Fetch<Week>(string.Format(Week.get_hours, id, id2));
                Week normal = sheet.Where(a => !a.IsOvertime).SingleOrDefault();
                Week overtime = sheet.Where(a => a.IsOvertime).SingleOrDefault();

                Hrs hrs = new Hrs(normal);

                hrs.WorkerId = workerid.Value;
                hrs.CopyHeader(normal??overtime);
                
                if (normal != null)
                {
                    hrs.Year = normal.Year;
                    hrs.WeekId = normal.WeekId;
                    hrs.nMon = d(normal.Mon);
                    hrs.nTue = d(normal.Tue);
                    hrs.nWed = d(normal.Wed);
                    hrs.nThu = d(normal.Thu);
                    hrs.nFri = d(normal.Fri);
                    hrs.nSat = d(normal.Sat);
                    hrs.nSun = d(normal.Sun);
                }
                if (overtime != null)
                {
                    hrs.Year = overtime.Year;
                    hrs.oWeekId = overtime.WeekId;
                    hrs.oMon = d(overtime.Mon);
                    hrs.oTue = d(overtime.Tue);
                    hrs.oWed = d(overtime.Wed);
                    hrs.oThu = d(overtime.Thu);
                    hrs.oFri = d(overtime.Fri);
                    hrs.oSat = d(overtime.Sat);
                    hrs.oSun = d(overtime.Sun);
                }
                if (hrs.PartnerId == Week.NonDemand)
                    hrs.TimeTypeId = hrs.CustomerId.Value;
                return PartialView("_Hours", hrs);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return null;
            }
        }

        [HttpGet]
        public ActionResult Create(int id)
        {
            var workerid = Session["WorkerId"] as int?;

            try
            {
                if (!workerid.HasValue)
                    throw new Exception("No workerid when creating blank hours from description");

                var weekno = Session["CurrentWeek"] as int?;
                if (!weekno.HasValue)
                    throw new Exception("No week in Home.Create");

                var year = Session["CurrentYear"] as int?;
                if (!year.HasValue)
                    throw new Exception("No year in Home.Create");

                Week wk = new Week(workerid.Value, weekno.Value, year.Value);
                Hrs hrs = new Hrs(wk);

                if (id == 0)
                    return PartialView("_Hours", hrs);

                // prior description selected

                tsDB db = new tsDB();
                
                var prior = db.Fetch<Week>(string.Format(Week.get_prior, workerid.Value, id)).SingleOrDefault();
                
                hrs.CopyHeader(prior);
                hrs.WeekNumber = weekno.Value;                  // stay on currently viewed week not the prior
                if (prior == null) hrs.DescriptionId = id;
                return PartialView("_Hours", hrs);
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return null;
            }
        }

        [HttpGet]
        public ActionResult Delete(int id, int id2)
        {
            dbExec(Week.Delete(id) + Week.Delete(id2));
            return RedirectToAction("Index", new { id = Session["CurrentWeek"] });
        }

        [HttpPost]
        public ActionResult Submit(int WorkerId, int weekNumber)
        {
            dbExec(Week.Submit(WorkerId, weekNumber));
            TempData["submit"] = true;
            return RedirectToAction("Index", new { id = weekNumber });
        }

        public ActionResult Contact(string id)
        {
            return View(id);
        }

        public ActionResult Report()
        {
            UserBase ub = new UserBase();
            ub.User = Session["user"].ToString();
            ub.IsAdmin = false;
            ub.IsManager = false;
            ub.Impersonating = false;
            return View(ub);
        }

        [HttpPost]
        public ActionResult Error(string errmsg)
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;

            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(string.Format("Client side error--{0}", errmsg)));
            var week = Session["CurrentWeek"] as int?;
            if (!week.HasValue)
            {
                var wk = new ISO_8601(DateTime.Today);
                week = wk.week;
            }
            return RedirectToAction("Index", new { id = week });
        }
    }
}
