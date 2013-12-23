using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Personnel()
        {
            Personnel p = new Personnel();
            p.User = Session["user"].ToString();

            using (tsDB db = new tsDB())
            {
                var user = db.Single<Worker>("where workerid = @0", Session["WorkerId"]);
                p.IsAdmin = user.IsAdmin;
                p.IsManager = user.IsManager;
                var query = (user.IsManager && !user.IsAdmin) ? 
                                string.Format(Models.Worker.delegates, user.WorkerId) : Models.Worker.all;
                p.list = db.Fetch<Worker>(query);
            }
            return View(p);
        }

        public ActionResult Worker(int id)
        {
            WorkerView w = new WorkerView(id);
            w.User = Session["user"].ToString();
            w.IsAdmin = true;
            return View(w);
        }

        public ActionResult Impersonate(int id)
        {
            if (id > 0)
            {
                Session["admin"] = Session["user"];
                using (tsDB db = new tsDB())
                {
                    var user = db.FirstOrDefault<Worker>("where WorkerId = @0", id);
                    if (user == null)
                    {
                        Session.Remove("admin");
                        return RedirectToAction("Personnel", "Admin");
                    }
                    if (string.IsNullOrWhiteSpace(user.IonName)) 
                        Session["user"] = user.WorkerId.ToString();
                    else if (Session["user"].ToString().Contains(user.IonName))     // joker
                        Session.Remove("admin");
                    else
                        Session["user"] = user.WorkerId.ToString();
                }
            }
            else
            {
                Session["user"] = Session["admin"];
                Session.Remove("admin");
            }
            return RedirectToAction("Index", "Home");    
        }

        [HttpPost]
        public ActionResult SaveWorker(WorkerView wv)
        {
            wv.w.Save();
            return RedirectToAction("Personnel");
        }

        public ActionResult InternalNumbers()
        {
            InternalNumbers i = new InternalNumbers();
            i.User = Session["user"].ToString();
            using (tsDB db = new tsDB())
            {
                var user = db.Single<Worker>("where workerid = @0", Session["WorkerId"]);
                i.IsAdmin = user.IsAdmin;
                i.IsManager = user.IsManager;
                i.list = db.Fetch<InternalNumber>(string.Format(Models.InternalNumber.all, (int)ChargeTo.Internal_Number));
            }
            return View(i);
        }

        public ActionResult InternalNumber(int id)
        {
            NumberView w = new NumberView(id);
            w.User = Session["user"].ToString();
            w.IsAdmin = true;
            return View(w);
        }

        [HttpPost]
        public ActionResult SaveInternalNumber(NumberView nv)
        {
            nv.ino.Save();
            return RedirectToAction("InternalNumbers");
        }

        public ActionResult DeleteInternalNumber(InternalNumber ino)
        {
            ino.Delete();
            return RedirectToAction("InternalNumbers");
        }

        public ActionResult WorkAreas()
        {
            WorkAreas i = new WorkAreas();
            i.User = Session["user"].ToString();
            using (tsDB db = new tsDB())
            {
                var user = db.Single<Worker>("where workerid = @0", Session["WorkerId"]);
                i.IsAdmin = user.IsAdmin;
                i.IsManager = user.IsManager;
                i.list = db.Fetch<WorkArea>(string.Format(Models.WorkArea.all));
            }
            return View(i);
        }

        public ActionResult WorkArea(int id)
        {
            AreaView w = new AreaView(id);
            w.User = Session["user"].ToString();
            w.IsAdmin = true;
            return View(w);
        }

        [HttpPost]
        public ActionResult SaveWorkArea(AreaView nv)
        {
            nv.wa.Save();
            return RedirectToAction("WorkAreas");
        }

        public ActionResult Sites()
        {
            Sites i = new Sites();
            i.User = Session["user"].ToString();
            using (tsDB db = new tsDB())
            {
                var user = db.Single<Worker>("where workerid = @0", Session["WorkerId"]);
                i.IsAdmin = user.IsAdmin;
                i.IsManager = user.IsManager;
                i.list = db.Fetch<Site>(string.Format(Models.Site.all));
            }
            return View(i);
        }

        public ActionResult Site(int id)
        {
            SiteView w = new SiteView(id);
            w.User = Session["user"].ToString();
            w.IsAdmin = true;
            return View(w);
        }

        [HttpPost]
        public ActionResult SaveSite(SiteView nv)
        {
            nv.wa.Save();
            return RedirectToAction("Sites");
        }

        public ActionResult Partners()
        {
            Partners i = new Partners();
            i.User = Session["user"].ToString();
            using (tsDB db = new tsDB())
            {
                var user = db.Single<Worker>("where workerid = @0", Session["WorkerId"]);
                i.IsAdmin = user.IsAdmin;
                i.IsManager = user.IsManager;
                i.list = db.Fetch<Partner>(string.Format(Models.Partner.all));
            }
            return View(i);
        }

        public ActionResult Partner(int id)
        {
            PartnerView w = new PartnerView(id);
            w.User = Session["user"].ToString();
            w.IsAdmin = true;
            return View(w);
        }

        [HttpPost]
        public ActionResult SavePartner(PartnerView nv)
        {
            nv.wa.Save();
            return RedirectToAction("Partners");
        }

        public ActionResult CostCenters()
        {
            CostCenters i = new CostCenters();
            i.User = Session["user"].ToString();
            using (tsDB db = new tsDB())
            {
                var user = db.Single<Worker>("where workerid = @0", Session["WorkerId"]);
                i.IsAdmin = user.IsAdmin;
                i.IsManager = user.IsManager;
                i.list = db.Fetch<CostCenter>(string.Format(Models.CostCenter.all, (int)ChargeTo.Cost_Center));
            }
            return View(i);
        }

        public ActionResult CostCenter(int id)
        {
            CenterView w = new CenterView(id);
            w.User = Session["user"].ToString();
            w.IsAdmin = true;
            return View(w);
        }

        [HttpPost]
        public ActionResult SaveCostCenter(CenterView nv)
        {
            nv.cc.Save();
            return RedirectToAction("CostCenters");
        }

        public ActionResult DeleteCostCenter(CostCenter cc)
        {
            cc.Delete();
            return RedirectToAction("CostCenters");
        }

        public ActionResult ExportXLS()
        {
            Export xp = new Export();
            xp.User = Session["user"].ToString();
            xp.IsAdmin = true;
            xp.start = DateTime.Today.AddDays(-(double)DateTime.Today.DayOfWeek-6);
            xp.end = DateTime.Today.AddDays(-(double)DateTime.Today.DayOfWeek);
            return View(xp);
        }

        [HttpPost]
        public ActionResult GetXLS(Export xp)
        {
            var template = Enum.GetName(typeof(Template), xp.type);
            var tst = xp;
            try { // Opening the Excel template... 
                FileStream fs = new FileStream(Server.MapPath(@"~/Content/"+template+".xls"), FileMode.Open, FileAccess.Read);  // Getting the complete workbook... 
                HSSFWorkbook templateWorkbook = new HSSFWorkbook(fs, true);                                                     // Getting the worksheet by its name... 

                switch (xp.type)
                {
                    case Template.Capital_Cost:
                        xp.capital(templateWorkbook);
                        break;
                    case Template.Cross_Charge:
                        xp.expense(templateWorkbook);
                        break;
                    case Template.Dash_Board:
                        xp.dashboard(templateWorkbook);
                        break;
                }
                //HSSFSheet sheet = templateWorkbook.GetSheet("Sheet1"); // Getting the row... 0 is the first row. 
                //HSSFRow dataRow = sheet.GetRow(4); // Setting the value 77 at row 5 column 1 
                //dataRow.GetCell(0).SetCellValue(77); // Forcing formula recalculation... 
                //sheet.ForceFormulaRecalculation = true; 
                MemoryStream ms = new MemoryStream(); // Writing the workbook content to the FileStream... 
                templateWorkbook.Write(ms); 
                TempData["Message"] = "Excel report created successfully!"; // Sending the server processed data back to the user computer... 
                return File(ms.ToArray(), "application/vnd.ms-excel", template+".xls"); 
            } 
            catch(Exception ex) {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Export threw up", ex));
                return RedirectToAction("Index", "Home"); 
            } 
        }
    }
}
