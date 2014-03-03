using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using ClosedXML;
using ClosedXML.Excel;

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
            select w.*, r.LevelId, r.FacilityId from [Week] w 
                join [Worker] r on r.WorkerId =  w.WorkerId
                where w.Submitted is not null and
                     r.FacilityId in ({0}) and w.PartnerId in ({1}) and w.WorkAreaId in ({2}) and
                     @0 < 100*w.[Year] + w.[WeekNumber] and 
                     100*w.[Year] + w.[WeekNumber] < @1 ";


        public int expense(XLWorkbook xl)
        {
            int startyw = Week.YearWeek(start) - 1;
            int endyw = Week.YearWeek(end) + 1;

            int rowy = 36;

            using (tsDB db = new tsDB())
            {
                try
                {
                    var level = db.Fetch<Level>("");
                    var ex = db.Fetch<Week, CostCenter, InternalNumber, Description>(expenses_period, startyw, endyw, ChargeTo.Capital_Number);
                    var num = ConfigurationManager.AppSettings["ExpenseNumber"];
                    var password = ConfigurationManager.AppSettings["CrossChargeProtection"];

                    var sheet = xl.Worksheet("Journal Entry Form");

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

                        if (cc?(x.cc.LegalEntity == "0"):string.IsNullOrWhiteSpace(x.ino.LegalEntity)) 
                            sheet.Range(rowy, 2, rowy, 12).Style.Fill.BackgroundColor = XLColor.Pink;
                        else
                            sheet.Cell(rowy, 2).Value = (cc ? x.cc.LegalEntity : x.ino.LegalEntity);

                        sheet.Cell(rowy, 3).Value = 40;          
                        sheet.Cell(rowy, 4).Value = num;
                        sheet.Cell(rowy, (cc ? 6 : 5)).Value = (cc ? x.cc._CostCenter : x.ino.InternalOrder);
                        sheet.Cell(rowy, 7).Value = charge.ToString("0.00");
                        sheet.Cell(rowy, 11).Value = x.CustomerName + " / " + x.LastName + " / " + x.desc._Description;

                        rowy++;
                    }
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("expense export: " + db.LastCommand, e));
                }
            }
            return rowy - 34;
        }

        public int capital(XLWorkbook wb)
        {
            int startyw = Week.YearWeek(start) - 1;
            int endyw = Week.YearWeek(end) + 1;
            var num = ConfigurationManager.AppSettings["CapitalNumber"];

            int rowy = 24;

            using (tsDB db = new tsDB())
            {
                try
                {
                    var level = db.Fetch<Level>();
                    var ex = db.Fetch<Week, Description>(capital_period, startyw, endyw, ChargeTo.Capital_Number);
                    var caps = ex.Select(cp => cp.CapitalNumber).Distinct();

                    var sheet = wb.Worksheet("Journal Entry Form");

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

                        sheet.Cell(rowy, 2).Value = rowy - 23;
                        sheet.Cell(rowy, 3).Value = 40;
                        sheet.Cell(rowy, 4).Value = "001";
                        sheet.Cell(rowy, 6).Value = num;
                        sheet.Cell(rowy, 7).Value = capcharge.ToString("0.00");
                        sheet.Cell(rowy, 10).Value = c;
                        sheet.Cell(rowy, 11).Value = ((custr == null) ? "" : custr) + " / " + ((lastn == null) ? "" : lastn) + " / " + descr;

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
            int al = string.IsNullOrWhiteSpace(a)? 0 :(a.Length < 4 ? a.Length : 4);
            int bl = string.IsNullOrWhiteSpace(b)? 0 :(b.Length < 4 ? b.Length : 4);
            return a.Substring(0, al) == b.Substring(0, bl);
        }

        private string join(IEnumerable<int> ids)
        {
            return string.Join(",", ids.Select(c => c.ToString()).ToArray());
        }

        public void dashboard(XLWorkbook wb)
        {
            int startyw = Week.YearWeek(start) - 1;
            int endyw = Week.YearWeek(end) + 1;

            try
            {
                var sheet = wb.Worksheet(2);

                List<string> areaV = new List<string>(9);
                for (int i = 4; i < 13; i++)
                    areaV.Add((sheet.Cell(14, i).Value.ToString() ?? string.Empty).ToLower());

                List<string> partV = new List<string>(5);
                for (int i = 15; i < 20; i++)
                    partV.Add((sheet.Cell(i, 2).Value.ToString() ?? string.Empty).ToLower());

                List<string> siteV = new List<string>(4);
                for (int i = 3; i < 7; i++)
                    siteV.Add(wb.Worksheet(i).Name.ToLower());

                using (tsDB db = new tsDB())
                {
                    var areas = db.Fetch<WorkArea>("");
                    var sites = db.Fetch<Site>("");
                    var partners = db.Fetch<Partner>("");
                    var level = db.Fetch<Level>();

                    List<int> colorder = new List<int>();
                    foreach (var s in areaV)
                    {
                        var f = areas.Find(a => comp4(a._WorkArea.ToLower(), s));
                        if (f == null)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("dashboard export: cannot find " + s));
                            return;
                        }
                        colorder.Add(f.WorkAreaId);
                    }

                    List<int> roworder = new List<int>();
                    foreach (var s in partV)
                    {
                        var f = partners.Find(a => comp4(a._Partner.ToLower(), s));
                        if (f == null)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("dashboard export: cannot find " + s));
                            return;
                        }
                        roworder.Add(f.PartnerId);
                    }
                    
                    List<int> shtorder = new List<int>();
                    foreach (var s in siteV)
                    {
                        var f = sites.Find(a => comp4(a._Site.ToLower(), s));
                        if (f == null) {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("dashboard export: cannot find "+ s));
                            return;
                        }
                        shtorder.Add(f.SiteId);
                    }
                    //var shtorder = siteV.Select(n => sites.Find(a => comp4(a._Site.ToLower(), n)).SiteId).ToList();

                    string query = string.Format(dashboard_period, join(shtorder), join(roworder), join(colorder));

                    var data = db.Fetch<Week>(query, startyw, endyw);
                    IEnumerable<Week> site;

                    for (int i = 3; i < 7; i++)                                             // sheet
                    {
                        sheet = wb.Worksheet(i);
                        site = data.Where(d => d.FacilityId == shtorder[i - 3]);                  // row
                        for (int j = 15; j < 20; j++)
                        {
                            int tot = 0;
                            decimal totc = 0;
                            decimal tote = 0;

                            var partner = site.Where(e => e.PartnerId == roworder[j - 15]);
                            for (int k = 3; k < 12; k++)                                    // column 
                            {
                                int count = partner.Count(f => f.WorkAreaId == colorder[k - 3]);
                                var capitals = partner.Where(
                                    p => p.NewRequest &&
                                    p.AccountType == (int?)ChargeTo.Capital_Number &&
                                    p.WorkAreaId == colorder[k - 3]).ToList();

                                var expenses = partner.Where(p => p.AccountType != (int?)ChargeTo.Capital_Number && p.WorkAreaId == colorder[k - 3]).ToList();
                                sheet.Cell(j, k + 1).Value = (count == 0 ? "" : count.ToString());
                                tot += count;

                                decimal capital = 0;
                                foreach (var x in capitals.Where(xc => xc != null))
                                {
                                    decimal rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).Single();
                                    decimal charge = 0;
                                    int nowyw = x.Year * 100 + x.WeekNumber;

                                    charge = x.Charge(rate, nowyw, startyw, endyw, start, end);
                                    if (charge == 0) continue;
                                    capital += charge;
                                }
                                sheet.Cell(j + 10, k + 1).Value = (capital == 0 ? "" : capital.ToString("0.00"));
                                totc += capital;

                                decimal expense = 0;
                                foreach (var x in expenses.Where(xd => xd != null))
                                {
                                    decimal rate = level.Where(s => s.LevelId == x.LevelId).Select(r => x.IsOvertime ? r.OvertimeRate : r.RegularRate).Single();
                                    decimal charge = 0;
                                    int nowyw = x.Year * 100 + x.WeekNumber;

                                    charge = x.Charge(rate, nowyw, startyw, endyw, start, end);
                                    if (charge == 0) continue;
                                    expense += charge;
                                }
                                sheet.Cell(j + 21, k + 1).Value = (expense == 0 ? "" : expense.ToString("0.00"));
                                tote += expense;
                            }
                            sheet.Cell(j, 3).Value = (tot == 0 ? "" : tot.ToString());
                            sheet.Cell(j + 10, 3).Value = (totc == 0 ? "" : totc.ToString("0.00"));
                            sheet.Cell(j + 21, 3).Value = (tote == 0 ? "" : tote.ToString("0.00"));
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("dashboard export: ", e));
            }
        }
    }
}