using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

namespace TimeSheet.Models
{
    public enum Template
    {
        Capital_Cost,
        Cross_Charge,
        Dash_Board
    };

    public class Export
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public Template type { get; set; }
        public string User { get; set; }
        public bool IsAdmin { get; set; }

        private static string expenses_period = @"
            select w.*, r.LevelId, r.LastName, u.CustomerName, c.*, i.*, d.* from [Week] w
                join [Worker] r on r.WorkerId = w.WorkerId
                join [Partner] p on p.PartnerId = w.PartnerId
                join [Customer] u on u.CustomerId = w.CustomerId
                join [CostCenter] c on c.CostCenterId = w.CostCenterId
                join [InternalNumber] i on i.InternalNumberId = w.InternalNumberId
                join [Description] d on d.DescriptionId = w.DescriptionId
                where p.Partner != 'RDSS' and w.[AccountType] != @2 and
                    @0 < 100*w.[Year] + w.[WeekNumber] and 
                    100*w.[Year] + w.[WeekNumber] < @1 ";

        private static string capital_period = @"
            select w.*, r.LevelId, u.*, d.* from [Week] w
                join [Worker] r on r.WorkerId = w.WorkerId
                join [Partner] p on p.PartnerId = w.PartnerId
                join [Customer] u on u.CustomerId = w.CustomerId
                join [Description] d on d.DescriptionId = w.DescriptionId
                where p.Partner != 'RDSS' and w.[AccountType] = @2 and
                    @0 < 100*w.[Year] + w.[WeekNumber] and 
                    100*w.[Year] + w.[WeekNumber] < @1 ";

        public int expense(HSSFWorkbook wb)
        {
            int startyw = Week.YearWeek(start)-1;
            int endyw = Week.YearWeek(end)+1;

            ISheet sheet = wb.GetSheet("Journal Entry Form");
            int rowy = 35;

            using (tsDB db = new tsDB())
            {
                try
                {
                    var level = db.Fetch<Level>("");
                    var ex = db.Fetch<Week, CostCenter, InternalNumber, Description>(expenses_period, startyw, endyw, ChargeTo.Capital_Number);

                    foreach (var x in ex)
                    {
                        decimal? rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).SingleOrDefault();
                        if (rate == null)
                            continue;
                        decimal charge = 0;
                        int testedge = x.Year * 100 + x.WeekNumber;

                        if (testedge == startyw + 1) charge = x.ChargeStart(rate.Value, start);
                        else if (testedge == endyw - 1) charge = x.ChargeEnd(rate.Value, end);
                        else charge = x.Charge(rate.Value);
                        if (charge == 0) continue;

                        bool cc = x.AccountType.Value == (int)ChargeTo.Cost_Center;
                        IRow row = sheet.GetRow(rowy);
                        row.GetCell(1).SetCellValue(cc ? x.cc.LegalEntity : x.ino.LegalEntity);
                        row.GetCell(2).SetCellValue(40);
                        row.GetCell(3).SetCellValue("52970002");
                        row.GetCell(cc ? 5 : 4).SetCellValue(cc ? x.cc._CostCenter : x.ino.InternalOrder);
                        row.GetCell(6).SetCellValue(charge.ToString("0.00"));
                        row.GetCell(11).SetCellValue(x.CustomerName + "-" + x.LastName + "-" + x.desc._Description);
                        rowy++;
                    }
                }
                catch(Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("expense query: "+db.LastCommand, e));
                }
            }
            return rowy - 35;
        }

        public int capital(HSSFWorkbook wb)
        {
            int startyw = Week.YearWeek(start) - 1;
            int endyw = Week.YearWeek(end) + 1;

            ISheet sheet = wb.GetSheet("Journal Entry Form");
            int rowy = 23;

            using (tsDB db = new tsDB())
            {
                try {
                var level = db.Fetch<Level>();
                var ex = db.Fetch<Week, Description>(capital_period, startyw, endyw, ChargeTo.Capital_Number);

                foreach (var x in ex)
                {
                    decimal rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).Single();
                    decimal charge = 0;
                    int testedge = x.Year * 100 + x.WeekNumber;

                    if (testedge == startyw + 1) charge = x.ChargeStart(rate, start);
                    else if (testedge == endyw - 1) charge = x.ChargeEnd(rate, end);
                    else charge = x.Charge(rate);
                    if (charge == 0) continue;

                    bool cc = x.AccountType.Value == (int)ChargeTo.Cost_Center;
                    IRow row = sheet.GetRow(rowy);
                    row.GetCell(1).SetCellValue(rowy - 22);
                    row.GetCell(2).SetCellValue(40);
                    row.GetCell(3).SetCellValue("001");
                    row.GetCell(4).SetCellValue("52970002");
                    row.GetCell(6).SetCellValue(charge.ToString("0.00"));
                    row.GetCell(9).SetCellValue(x.CapitalNumber);
                    row.GetCell(10).SetCellValue(x.CustomerName + "-" + x.LastName + "-" + x.desc._Description);
                    rowy++;
                }
                                }
                catch(Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("expense query: "+db.LastCommand, e));
                }

            }
            return rowy - 23;
        }
        
        public void dashboard(HSSFWorkbook wb)
        {

        }
    }
}