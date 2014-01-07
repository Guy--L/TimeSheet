using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace TimeSheet.Models
{
    public enum Template
    {
        Capital_Cost,
        Cross_Charge,
        Dash_Board
    };

    public class Export : UserBase
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public Template type { get; set; }

        private static string expenses_period = @"
            select w.*, r.LevelId, r.LastName, u.CustomerName, c.*, i.*, d.* from [Week] w
                join [Worker] r on r.WorkerId = w.WorkerId
                join [Partner] p on p.PartnerId = w.PartnerId
                join [Customer] u on u.CustomerId = w.CustomerId
                left join [CostCenter] c on c.CostCenterId = w.CostCenterId
                left join [InternalNumber] i on i.InternalNumberId = w.InternalNumberId
                join [Description] d on d.DescriptionId = w.DescriptionId
                where p.Partner != 'RDSS' and w.[AccountType] != @2 and
                    w.Submitted is not null and
                    @0 < 100*w.[Year] + w.[WeekNumber] and 
                    100*w.[Year] + w.[WeekNumber] < @1 ";

        private static string capital_period = @"
            select w.*, r.LevelId, r.LastName, u.CustomerName, d.* from [Week] w
                join [Worker] r on r.WorkerId = w.WorkerId
                join [Partner] p on p.PartnerId = w.PartnerId
                join [Customer] u on u.CustomerId = w.CustomerId
                join [Description] d on d.DescriptionId = w.DescriptionId
                where p.Partner != 'RDSS' and w.[AccountType] = @2 and
                    w.Submitted is not null and 
                    @0 < 100*w.[Year] + w.[WeekNumber] and 
                    100*w.[Year] + w.[WeekNumber] < @1
                order by w.capitalnumber ";

        private static string dashboard_usingview = @"
            select v.* from [HoursByWeek] v 
                where @0 < v.YearWeek and v.YearWeek < @1 and
                    v.SiteId in ({0}) and v.PartnerId in ({1}) and v.WorkAreaId in ({2}) ";

        private static string dashboard_period = @"
            select w.*, r.LevelId from [Week] w 
                join [Worker] r on r.WorkerId =  w.WorkerId
                where w.Submitted is not null and
                     w.SiteId in ({0}) and w.PartnerId in ({1}) and w.WorkAreaId in ({2}) and
                     @0 < 100*w.[Year] + w.[WeekNumber] and 
                     100*w.[Year] + w.[WeekNumber] < @1 ";

        public int expense(HSSFWorkbook wb)
        {
            int startyw = Week.YearWeek(start) - 1;
            int endyw = Week.YearWeek(end) + 1;

            ISheet sheet = wb.GetSheet("Journal Entry Form");
            int rowy = 35;

            ICellStyle old = sheet.GetRow(36).GetCell(1).CellStyle;
            ICellStyle style = wb.CreateCellStyle();
            style.CloneStyleFrom(old);
            style.FillPattern = FillPatternType.LEAST_DOTS;
            style.FillBackgroundColor = HSSFColor.ROSE.index;
        
            using (tsDB db = new tsDB())
            {
                try
                {
                    var level = db.Fetch<Level>("");
                    var ex = db.Fetch<Week, CostCenter, InternalNumber, Description>(expenses_period, startyw, endyw, ChargeTo.Capital_Number);

                    var tst = db.LastCommand;
                    var tot = db.LastSQL;

                    foreach (var x in ex)
                    {
                        decimal? rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).SingleOrDefault();
                        if (rate == null)
                            continue;
                        decimal charge = 0;
                        int nowyw = x.Year * 100 + x.WeekNumber;

                        charge = x.Charge(rate.Value, nowyw, startyw, endyw, start, end);
                        if (charge == 0) continue;

                        bool cc = x.AccountType.Value == (int)ChargeTo.Cost_Center;
                        IRow row = sheet.GetRow(rowy);

                        if (cc?(x.cc.LegalEntity == "0"):string.IsNullOrWhiteSpace(x.ino.LegalEntity))
                            row.Cells.ForEach(c => { c.CellStyle = style; });
                        else
                            row.GetCell(1).SetCellValue(cc ? x.cc.LegalEntity : x.ino.LegalEntity);

                        row.GetCell(2).SetCellValue(40);
                        row.GetCell(3).SetCellValue("52970002");
                        row.GetCell(cc ? 5 : 4).SetCellValue(cc ? x.cc._CostCenter : x.ino.InternalOrder);
                        row.GetCell(6).SetCellValue(charge.ToString("0.00"));
                        row.GetCell(11).SetCellValue(x.CustomerName + " / " + x.LastName + " / " + x.desc._Description);
                        rowy++;
                    }
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("expense export: " + db.LastCommand, e));
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
                try
                {
                    var level = db.Fetch<Level>();
                    var ex = db.Fetch<Week, Description>(capital_period, startyw, endyw, ChargeTo.Capital_Number);
                    var caps = ex.Select(c => c.CapitalNumber).Distinct();

                    foreach (var c in caps)
                    {
                        decimal capcharge = 0;
                        var samecaps = ex.Where(d => d.CapitalNumber == c);
                        foreach (var x in samecaps)
                        {
                            decimal rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).Single();
                            decimal charge = 0;
                            int nowyw = x.Year * 100 + x.WeekNumber;

                            charge = x.Charge(rate, nowyw, startyw, endyw, start, end);
                            if (charge == 0) continue;
                            capcharge += charge;
                        }
                        if (capcharge == 0) continue;
                        var custr = samecaps.Where(s => !string.IsNullOrWhiteSpace(s.CustomerName)).Select(s => s.CustomerName).FirstOrDefault();
                        var lastn = samecaps.Where(s => !string.IsNullOrWhiteSpace(s.LastName)).Select(s => s.LastName).FirstOrDefault();
                        var descr = samecaps.Where(s => !string.IsNullOrWhiteSpace(s.desc._Description)).Select(s => s.desc._Description).FirstOrDefault();
                       
                        IRow row = sheet.GetRow(rowy);
                        row.GetCell(1).SetCellValue(rowy - 22);
                        row.GetCell(2).SetCellValue(40);
                        row.GetCell(3).SetCellValue("001");
                        row.GetCell(4).SetCellValue("52970002");
                        row.GetCell(6).SetCellValue(capcharge.ToString("0.00"));
                        row.GetCell(9).SetCellValue(c);
                        row.GetCell(10).SetCellValue(((custr==null)?"":custr) + " / " + ((lastn==null)?"":lastn) + " / " + descr);
                        rowy++;
                    }
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("capital export: " + db.LastCommand, e));
                }

            }
            return rowy - 23;
        }

        private bool comp4(string a, string b)
        {
            int al = a.Length < 4 ? a.Length : 4;
            int bl = b.Length < 4 ? b.Length : 4;
            return a.Substring(0, al) == b.Substring(0, bl);
        }

        private string join(IEnumerable<int> ids)
        {
            return string.Join(",", ids.Select(c => c.ToString()).ToArray());
        }

        public void dashboard(HSSFWorkbook wb)
        {
            int startyw = Week.YearWeek(start) - 1;
            int endyw = Week.YearWeek(end) + 1;

            ISheet sheet = wb.GetSheetAt(1);
            IRow row = sheet.GetRow(13);
            IRow rowe, rowc;
            
            var areaV = row.Cells.Skip(2).Take(9).Select(c => c.StringCellValue.ToLower());
            List<string> partV = new List<string>(5);
            for (int i = 14; i < 19; i++)
                partV.Add(sheet.GetRow(i).GetCell(1).StringCellValue.ToLower());

            List<string> siteV = new List<string>(4);
            for (int i = 2; i < 6; i++)
                siteV.Add(wb.GetSheetName(i).ToLower());

            using (tsDB db = new tsDB())
            {
                var areas = db.Fetch<WorkArea>("");
                var sites = db.Fetch<Site>("");
                var partners = db.Fetch<Partner>("");
                var level = db.Fetch<Level>();

                var colorder = areaV.Select(n => areas.Find(a => comp4(a._WorkArea.ToLower(), n)).WorkAreaId).ToList();
                var roworder = partV.Select(n => partners.Find(a => comp4(a._Partner.ToLower(), n)).PartnerId).ToList();
                var shtorder = siteV.Select(n => sites.Find(a => comp4(a._Site.ToLower(), n)).SiteId).ToList();
                string query = string.Format(dashboard_period, join(shtorder), join(roworder), join(colorder));

                var data = db.Fetch<Week>(query, startyw, endyw);
                IEnumerable<Week> site;

                for (int i = 2; i < 6; i++)
                {
                    sheet = wb.GetSheetAt(i);
                    site = data.Where(d => d.SiteId == shtorder[i-2]);
                    for (int j = 14; j < 19; j++)
                    {
                        row = sheet.GetRow(j);
                        rowc = sheet.GetRow(j + 10);
                        rowe = sheet.GetRow(j + 21);

                        int tot = 0;
                        decimal totc = 0;
                        decimal tote = 0;

                        var partner = site.Where(e => e.PartnerId == roworder[j-14]);
                        for (int k = 3; k < 12; k++)
                        {
                            int count = partner.Count(f => f.WorkAreaId == colorder[k - 3]);
                            var capitals = partner.Where(p => p.AccountType == (int?)ChargeTo.Capital_Number && p.WorkAreaId == colorder[k - 3]);
                            var expenses = partner.Where(p => p.AccountType != (int?)ChargeTo.Capital_Number && p.WorkAreaId == colorder[k - 3]);
                            row.GetCell(k).SetCellValue(count==0?"":count.ToString());
                            tot += count;

                            decimal capital = 0;
                            foreach (var x in capitals)
                            {
                                decimal rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).Single();
                                decimal charge = 0;
                                int nowyw = x.Year * 100 + x.WeekNumber;

                                charge = x.Charge(rate, nowyw, startyw, endyw, start, end);
                                if (charge == 0) continue;
                                capital += charge;
                            }

                            rowc.GetCell(k).SetCellValue(capital == 0 ? "" : capital.ToString("0.00"));
                            totc += capital;

                            decimal expense = 0;
                            foreach (var x in expenses)
                            {
                                decimal rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).Single();
                                decimal charge = 0;
                                int nowyw = x.Year * 100 + x.WeekNumber;

                                charge = x.Charge(rate, nowyw, startyw, endyw, start, end);
                                if (charge == 0) continue;
                                expense += charge;
                            }

                            rowe.GetCell(k).SetCellValue(expense == 0 ? "" : expense.ToString("0.00"));
                            tote += expense;
                        }
                        row.GetCell(2).SetCellValue(tot == 0 ? "" : tot.ToString());
                        rowc.GetCell(2).SetCellValue(totc == 0 ? "" : totc.ToString("0.00"));
                        rowe.GetCell(2).SetCellValue(tote == 0 ? "" : tote.ToString("0.00"));
                    }
                }
            }
        }

    }
}