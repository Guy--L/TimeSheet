using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Web.Mvc;
using Postal;
using TimeSheet.Models;
using ClosedXML.Excel;

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
        /// Convenience wrapper to run queries with initialization and error handling
        /// </summary>
        /// <param name="q">Sql statement</param>
        private void dbExec(NPoco.Sql q)
        {
            if (q == null)
                return;

            tsDB db = new tsDB();

            try
            {
                db.Execute(q);
            }
            catch (Exception e)
            {
                Exception frame = new Exception(q.SQL, e);
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

        private DateTime endSunday(DateTime day)
        {
            return day.AddDays(day.DayOfWeek == DayOfWeek.Sunday ? 0 : (7 - (int)day.DayOfWeek));
        }

        /// <summary>
        /// If user types in date or clicks the week rewind or advance buttons, find week and recall it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PickDate(string id)
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;

            var newday = DateTime.Today;
            try
            {
                newday = DateTime.Parse(id);
                //var newmon = calendar.GetWeekOfYear(newday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                //Session["CurrentYear"] = newday.Year;
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("PickDate threw up", e));
                //var week = Session["CurrentWeek"] as int?;
                //if (!week.HasValue)
                //{
                //    var wk = new ISO_8601(DateTime.Today);
                //    week = wk.week;
                //}
            }
            finally
            {
                Session["CurrentSunday"] = endSunday(newday);
            }
            return RedirectToAction("Index");
        }

        public void SubmitEmail(Worker w, Sheet s)
        {
            if (string.IsNullOrWhiteSpace(w.ManagerIon))
                return;

            try
            {
                XLWorkbook wb = new XLWorkbook(Server.MapPath(@"~/Content/TimeSheet.xlsx"));
                var sheet = wb.Worksheet("Sheet1");

                sheet.Cell(1, 1).Value = w.LastName + ", " + w.FirstName;
                sheet.Cell(2, 1).Value = w.EmployeeNumber;
                sheet.Cell(3, 1).Value = s.sunday;
                sheet.Cell(4, 1).Value = s.weekNumber;

                for (int i = 0; i < 7; i++)
                {
                    sheet.Cell(6, 4 + i).Value = s.Headers[i].ToString("M/d");
                    sheet.Cell(7, 4 + i).Value = s.Stats[4 + i];
                }
                sheet.Cell(7, 11).Value = s.Stats[3];

                int rowy = 8;
                foreach (var hr in s.hours)
                {
                    sheet.Cell(rowy, 1).Value = hr.IsOvertime ? "Y" : "";
                    sheet.Cell(rowy, 2).Value = hr.Description;
                    sheet.Cell(rowy, 3).Value = hr.ChargNumber;
                    sheet.Cell(rowy, 4).Value = hr.Mon;
                    sheet.Cell(rowy, 5).Value = hr.Tue;
                    sheet.Cell(rowy, 6).Value = hr.Wed;
                    sheet.Cell(rowy, 7).Value = hr.Thu;
                    sheet.Cell(rowy, 8).Value = hr.Fri;
                    sheet.Cell(rowy, 9).Value = hr.Sat;
                    sheet.Cell(rowy, 10).Value = hr.Sun;
                    sheet.Cell(rowy, 11).Value = hr.SubTotal;
                    sheet.Cell(rowy, 12).Value = hr.NewRequest ? "Y" : "";
                    sheet.Cell(rowy, 13).Value = hr.Customer;
                    sheet.Cell(rowy, 14).Value = hr.WorkArea;
                    sheet.Cell(rowy, 15).Value = hr.Partner;
                    sheet.Cell(rowy, 16).Value = hr.Site;
                    rowy++;
                }

                //var named = Path.Combine(Server.MapPath(@"~/App_Data"), "ts" + w.LastName + ".xls");

                MemoryStream ms = new MemoryStream();
                wb.SaveAs(ms);
                ms.Seek(0, SeekOrigin.Begin);

                var timesheet = AttachmentHelper.CreateAttachment(ms, "ts" + w.LastName + ".xls", TransferEncoding.Base64);
                dynamic email = new Email("TimeSheet");
                email.Attach(timesheet);
                email.From = ConfigurationManager.AppSettings["sentfrom"];
                email.To = w.ManagerIon + "@pg.com";
                email.Employee = w.FirstName + " " + w.LastName;
                email.Ending = s.sunday;
                email.Demand = s.Stats[0];
                email.NonDemand = s.Stats[1];
                email.OverTime = s.Stats[2];
                email.Total = s.Stats[3];
                email.Percent = s.Stats[11];
                email.Send();
            }
            catch (Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Submission email: ", e));
            }
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
            var modeluser = new UserBase() {
                User = usr
                , Impersonating = false
                , IsAdmin = false
                , Reports = ""
            };

            string who = "where w.ionname = @0";
            if (string.IsNullOrWhiteSpace(usr))
                return RedirectToAction("Contact", new UserBase() { User = usr, Impersonating = false, IsAdmin = false });

            if (char.IsDigit(usr[0]))
                who = "where w.workerid = @0";
            else
            {
                string[] worker = usr.ToString().Split('\\');
                usr = worker[worker.Length - 1];
            }
            Worker emp = db.FirstOrDefault<Worker>(Worker.worker + who, usr);
            if (emp == null)
                return RedirectToAction("Contact", new UserBase() { User = usr, Impersonating = false, IsAdmin = false });

            Session["WorkerId"] = emp.WorkerId;

            Sheet ts = new Sheet() {
                employee = emp,
                User = Session["user"].ToString(),
                IsAdmin = emp.IsAdmin,
                IsManager = emp.IsManager,
                Impersonating = (Session["admin"] != null),
                Reports = ConfigurationManager.AppSettings["ReportServerURL"]
            };

            var sunday = Session["CurrentSunday"] as DateTime?;
            sunday = sunday ?? endSunday(DateTime.Today);

            if (id.HasValue)
            {
                sunday = sunday.Value.AddDays(id.Value * 7);
            }

            var iso = new ISO_8601(sunday.Value);
            ts.year = iso.year;
            ts.weekNumber = iso.week;

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
            ts.sunday = sunday.Value.ToString("MM/dd/yyyy");
            ts.Headers = Enumerable.Range(-6, 7).Select(n => sunday.Value.AddDays(n)).ToList();

            TempData["WeekHeaders"] = ts.Headers;
            Session["CurrentSunday"] = sunday.Value;

            var submitted = TempData["submit"] as bool?;
            if (submitted.HasValue && submitted.Value)
            {
                try
                {
                    SubmitEmail(emp, ts);
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("submission email failed", e));
                }
            }
            
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
            return RedirectToAction("Index");
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
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Save changes or additions made using _Hours modal form while adding new list items as needed
        /// </summary>
        /// <param name="hours">Flat object containing normal and overtime hours</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Save(Hrs hours)
        {
            hours.UpdateCachedLists();           // add new records for Customer, Description, Cost Center or Internal Number
            dbExec(hours.Save());
            return RedirectToAction("Index");
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

                Hrs hrs = new Hrs(normal??overtime);
                hrs.Columns = TempData["WeekHeaders"] as List<DateTime>;

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

                var sunday = Session["CurrentSunday"] as DateTime?;
                sunday = sunday ?? endSunday(DateTime.Today);
                var weekno = new ISO_8601(sunday.Value);            // sundays are never in iso week 53

                Week wk = new Week(workerid.Value, weekno.week, weekno.year);
                Hrs hrs = new Hrs(wk);
                hrs.Columns = TempData["WeekHeaders"] as List<DateTime>;

                if (id == 0)
                    return PartialView("_Hours", hrs);

                tsDB db = new tsDB();
                var prior = db.Fetch<Week>(string.Format(Week.get_prior, workerid.Value, id)).SingleOrDefault();

                prior.Submitted = null;
                hrs.CopyHeader(prior);
                hrs.WeekNumber = weekno.week;                  // stay on currently viewed week not the prior
                hrs.Year = weekno.year;
                hrs.NewRequest = false;
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
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Submit(int WorkerId, int weekNumber, int year)
        {
            dbExec(Week.Submit(WorkerId, weekNumber, year));
            TempData["submit"] = true;
            return RedirectToAction("Index", new { id = 1 });
        }

        public ActionResult Contact(UserBase ub)
        {
            return View(ub);
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
            var sunday = Session["CurrentSunday"] as DateTime?;
            if (!sunday.HasValue)
            {
                Session["CurrentSunday"] = endSunday(DateTime.Today);
            }
            return RedirectToAction("Index");
        }
    }
}
