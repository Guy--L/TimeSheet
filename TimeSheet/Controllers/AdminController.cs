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
