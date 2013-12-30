using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPoco;

namespace TimeSheet.Models
{
    public enum ChargeTo
    {
        Internal_Number,
        Cost_Center,
        Capital_Number
    };

    public partial class Week
    {
        [Column] public int? PairId { get; set; }
        [Column] public int? LevelId { get; set; }
        [Column] public string LastName { get; set; }
        [Column] public string CustomerName { get; set; }
        public CostCenter cc { get; set; }
        public InternalNumber ino { get; set; }
        public Description desc { get; set; }

        public List<CostCenter> costCenters;
        public List<InternalNumber> internalNumbers;

        public List<WorkerCostCenter> workCenters;
        public List<WorkerInternalNumber> workNumbers;

        public List<Customer> customers;
        public List<Description> descriptions;

        public static List<WorkArea> workAreas;
        public static List<Site> sites;
        public static List<Partner> partners;

        public static int NonDemand;

        public static void GetLists()
        {
            if (partners != null && sites != null && workAreas != null)
                return;

            using (tsDB db = new tsDB())
            {
                sites = db.Fetch<Site>("");
                partners = db.Fetch<Partner>("");
                workAreas = db.Fetch<WorkArea>("");

                if (!sites.Any(s => s.SiteId == 0)) sites.Add(new Site { SiteId = 0, _Site = "" });
                if (!partners.Any(p => p.PartnerId == 0)) partners.Add(new Partner { PartnerId = 0, _Partner = "" });
                if (!workAreas.Any(w => w.WorkAreaId == 0)) workAreas.Add(new WorkArea { WorkAreaId = 0, _WorkArea = "" });
            }
            NonDemand = partners.Where(p => p._Partner == "RDSS").Select(q => q.PartnerId).Single();
        }

        public Week()
        { }

        public Week(int worker, int weekno, int year)
        {
            WeekNumber = weekno;
            WorkerId = worker;
            Year = year;
            GetLists(worker);
        }

        public Week(Hrs b, bool isNormal)
        {
            WorkerId = b.WorkerId;
            WeekId = isNormal ? b.WeekId : b.oWeekId;
            WeekNumber = b.WeekNumber;
            Year = b.Year;
            DescriptionId = b.DescriptionId;
            CustomerId = b.CustomerId;
            SiteId = b.SiteId;
            PartnerId = b.PartnerId;
            InternalNumberId = b.InternalNumberId??0;
            WorkAreaId = b.WorkAreaId;
            NewRequest = b.NewRequest;
            CostCenterId = b.CostCenterId??0;
            CapitalNumber = b.CapitalNumber;
            AccountType = b.AccountType;

            IsOvertime = !isNormal;

            if (isNormal)
            {
                Mon = b.nMon;
                Tue = b.nTue;
                Wed = b.nWed;
                Thu = b.nThu;
                Fri = b.nFri;
                Sat = b.nSat;
                Sun = b.nSun;
            }
            else
            {
                Mon = b.oMon;
                Tue = b.oTue;
                Wed = b.oWed;
                Thu = b.oThu;
                Fri = b.oFri;
                Sat = b.oSat;
                Sun = b.oSun;
            }
        }

        public static string[] Stats(List<Week> hrs)
        {
            decimal dem = 0, non = 0, ovt = 0;
            decimal? mon = 0, tue = 0, wed = 0, thu = 0, fri = 0, sat = 0, sun = 0;

            foreach (var w in hrs)
            {
                mon += w.Monday;
                tue += w.Tuesday;
                wed += w.Wednesday;
                thu += w.Thursday;
                fri += w.Friday;
                sat += w.Saturday;
                sun += w.Sunday;

                if (w.IsOvertime)
                    ovt += w.subTotal;
                else if (w.PartnerId == NonDemand)
                    non += w.subTotal;
                else
                    dem += w.subTotal;
            }
            return new string[11] { time2str(dem)
                                   , time2str(non)
                                   , time2str(ovt)
                                   , time2str(dem+non+ovt)
                                   ,time2str(mon)
                                   ,time2str(tue)
                                   ,time2str(wed)
                                   ,time2str(thu)
                                   ,time2str(fri)
                                   ,time2str(sat)
                                   ,time2str(sun)
            };
        }

        public string ChargeType
        {
            get
            {
                if (!AccountType.HasValue)
                    return "";
                return Enum.GetName(typeof(ChargeTo), AccountType.Value).Replace('_', ' ');
            }
        }

        public HtmlString ChargeNumber
        {
            get
            {
                if (!AccountType.HasValue)
                    return null;
                switch ((ChargeTo)AccountType.Value)
                {
                    case ChargeTo.Cost_Center:
                        if (CostCenterId == 0) return new HtmlString("");
                        var cc = costCenters.FirstOrDefault(i => i.CostCenterId == CostCenterId);
                        return new HtmlString((cc == null) ? "" : ("<i class='icon-bullseye'></i> " + cc._CostCenter));

                    case ChargeTo.Internal_Number:
                        if (InternalNumberId == 0) return new HtmlString("");
                        var inn = internalNumbers.FirstOrDefault(i => i.InternalNumberId == InternalNumberId);
                        return new HtmlString((inn == null) ? "" : ("<i class='icon-building'></i> " + inn.InternalOrder));

                    case ChargeTo.Capital_Number:
                        if (string.IsNullOrWhiteSpace(CapitalNumber)) return new HtmlString("");
                        return new HtmlString(("<i class='icon-money'></i> " + CapitalNumber));
                }
                return null;
            }
        }

        public string WorkArea
        {
            get
            {
                var w = workAreas.FirstOrDefault(i => i.WorkAreaId == (WorkAreaId ?? 0));
                if (w == null)
                    return "";
                return w._WorkArea;
            }
        }
        public string Customer
        {
            get
            {
                var c = customers.FirstOrDefault(i => i.CustomerId == (CustomerId ?? 0));
                if (c == null)
                    return "";
                return c.CustomerName;
            }
        }
        public string Description
        {
            get
            {
                var d = descriptions.FirstOrDefault(i => i.DescriptionId == DescriptionId);
                if (d == null)
                    return "";
                return d._Description;
            }
        }
        public string Site
        {
            get
            {
                var s = sites.FirstOrDefault(i => i.SiteId == (SiteId ?? 0));
                if (s == null)
                    return "";
                return s._Site;
            }
        }
        public string Partner
        {
            get
            {
                var partner = partners.FirstOrDefault(i => i.PartnerId == (PartnerId ?? 0));
                if (partner == null)
                    return "";
                return partner._Partner;
            }
        }

        public string ShortDescription { get { return string.IsNullOrWhiteSpace(Description) ? "" : (Description.Length > 20 ? Description.Substring(0, 20) : Description); } }

        public string Mon { get { return time2str(Monday); }    set { Monday = time2dec(value); } }
        public string Tue { get { return time2str(Tuesday); }   set { Tuesday = time2dec(value); } }
        public string Wed { get { return time2str(Wednesday); } set { Wednesday = time2dec(value); } }
        public string Thu { get { return time2str(Thursday); }  set { Thursday = time2dec(value); } }
        public string Fri { get { return time2str(Friday); }    set { Friday = time2dec(value); } }
        public string Sat { get { return time2str(Saturday); }  set { Saturday = time2dec(value); } }
        public string Sun
        {
            get { return time2str(Sunday); }
            set { Sunday = time2dec(value); }
        }

        public static string lst_week = @"
            select b.WeekId PairId, a.* from week a
                left join week b 
                    on a.CapitalNumber = b.CapitalNumber 
                    and a.CostCenterId = b.CostCenterId
                    and a.CustomerId = b.CustomerId
                    and a.DescriptionId = b.DescriptionId
                    and a.InternalNumberId = b.InternalNumberId
                    and a.AccountType = b.AccountType
                    and a.PartnerId = b.PartnerId
                    and a.SiteId = b.SiteId
                    and a.WorkAreaId = b.WorkAreaId
                    and a.NewRequest = b.NewRequest
                    and a.IsOverTime != b.IsOvertime
                    and a.workerid = b.workerid
                    and a.weeknumber = b.weeknumber
                    and a.year = b.year
                 where a.workerid = {0} and a.weeknumber = {1} ";

        private static string del_week = @" delete from week where weekid = {0} ";

        public static string Delete(int id)
        {
            return id == 0 ? "" : string.Format(del_week, id);
        }

        public static string get_prior = @"
            select top 1 * from week 
                where workerid = {0} and descriptionid = {1}
                order by year desc, weeknumber desc
        ";

        private static string time2str(decimal? hours)
        {
            if (!hours.HasValue || hours.Value == 0)
                return "";
            double hour = Math.Floor((double)hours);
            double minute = ((double)hours - hour) * 60d;
            TimeSpan ts = new TimeSpan((int)hour, (int)Math.Round(minute), 0);
            return string.Format("{0}:{1:00}", (int)ts.TotalHours, ts.Minutes);
        }

        private static decimal? time2dec(string hours)
        {
            if (string.IsNullOrWhiteSpace(hours))
                return null;
            decimal result = 0;
            try
            {
                result = Convert.ToDecimal(TimeSpan.Parse(hours).TotalHours);
            }
            catch (Exception e)
            {
                Exception frame = new Exception(hours, e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(frame);
            }
            return result;
        }

        public decimal subTotal
        {
            get
            {
                return (Monday ?? 0)
                    + (Tuesday ?? 0)
                    + (Wednesday ?? 0)
                    + (Thursday ?? 0)
                    + (Friday ?? 0)
                    + (Saturday ?? 0)
                    + (Sunday ?? 0);
            }
        }

        public string SubTotal { get { return time2str(subTotal); } }

        public string[] serializeDT()
        {
            var tot = subTotal;
            var description = descriptions[DescriptionId]._Description;
            var intern = InternalNumberId.HasValue ? internalNumbers[InternalNumberId.Value].InternalOrder : CapitalNumber;
            var costctr = CostCenterId.HasValue ? costCenters[CostCenterId.Value]._CostCenter : "";
            var customer = (CustomerId.HasValue && CustomerId.Value > 0) ? customers.First(v => v.CustomerId == CustomerId.Value).CustomerName : "";
            var workarea = WorkAreaId.HasValue ? Week.workAreas[WorkAreaId.Value]._WorkArea : "";
            var partner = PartnerId.HasValue ? Week.partners[PartnerId.Value]._Partner : "";
            var site = SiteId.HasValue ? Week.sites[SiteId.Value]._Site : "";
            var submitted = Submitted.HasValue ? Submitted.Value.ToString("MM/dd hh:mm") : "";

            return new string[26] {
                 WeekId.ToString()
                ,WeekNumber.ToString()
                ,IsOvertime.ToString()
                ,description
                ,intern 
                ,costctr                            // 5
                ,time2str(Monday)
                ,time2str(Tuesday)
                ,time2str(Wednesday)
                ,time2str(Thursday)
                ,time2str(Friday)                   // 10
                ,time2str(Saturday)
                ,time2str(Sunday)
                ,time2str(tot)
                ,customer
                ,workarea                           // 15
                ,NewRequest.ToString()
                ,partner
                ,site
                ,DescriptionId.ToString()
                ,CustomerId.ToString()              // 20
                ,InternalNumberId.ToString()
                ,WorkAreaId.ToString()
                ,PartnerId.ToString()
                ,SiteId.ToString()
                ,submitted.ToString()
            };
        }

        public bool Paired(Week b)
        {
            if (WeekId == 0 || b.WeekId == 0)
                return false;
            return WeekNumber == b.WeekNumber
                && Year == b.Year
                && DescriptionId == b.DescriptionId
                && CustomerId == b.CustomerId
                && SiteId == b.SiteId
                && PartnerId == b.PartnerId
                && InternalNumberId == b.InternalNumberId
                && WorkAreaId == b.WorkAreaId
                && NewRequest == b.NewRequest
                && CostCenterId == b.CostCenterId
                && AccountType == b.AccountType;
        }

        private void Copy(Week b)
        {
            WeekNumber = b.WeekNumber;
            Year = b.Year;
            DescriptionId = b.DescriptionId;
            CustomerId = b.CustomerId;
            SiteId = b.SiteId;
            PartnerId = b.PartnerId;
            InternalNumberId = b.InternalNumberId;
            WorkAreaId = b.WorkAreaId;
            NewRequest = b.NewRequest;
            CostCenterId = b.CostCenterId;
            CapitalNumber = b.CapitalNumber;
            AccountType = b.AccountType;
        }

        public void Match(Week b)
        {
            if (WeekId == 0)
            {
                if (b.WeekId != 0)
                {
                    Copy(b);
                }
            }
            else if (b.WeekId == 0 && WeekId != 0)
                b.Match(this);
            else if (WeekId != 0 && b.WeekId != 0)
                b.Copy(this);
        }

        public static string Submit(int worker, int week)
        {
            return string.Format(lock_week, DateTime.Now, worker, week);
        }

        public string SaveWeek()
        {
            if (WeekId == 0)
            {
                if (subTotal <= 0)
                    return "";

                return string.Format(ins_week
                       , WeekNumber
                       , Year
                       , WorkerId
                       , DescriptionId
                       , Comments
                       , IsOvertime ? 1 : 0
                       , Monday ?? 0
                       , Tuesday ?? 0
                       , Wednesday ?? 0
                       , Thursday ?? 0
                       , Friday ?? 0
                       , Saturday ?? 0
                       , Sunday ?? 0
                       , Submitted == null ? "null" : Submitted.ToString()
                       , NewRequest ? 1 : 0
                       , SiteId
                       , WorkAreaId
                       , PartnerId
                       , InternalNumberId
                       , CostCenterId
                       , CapitalNumber
                       , CustomerId ?? 0
                       , AccountType == null ? "null" : AccountType.ToString()
                       );
            }
            return string.Format(upd_week
                       , WeekId
                       , WeekNumber
                       , Year
                       , WorkerId
                       , DescriptionId
                       , Comments
                       , IsOvertime ? 1 : 0
                       , Monday ?? 0
                       , Tuesday ?? 0
                       , Wednesday ?? 0
                       , Thursday ?? 0
                       , Friday ?? 0
                       , Saturday ?? 0
                       , Sunday ?? 0
                       , Submitted == null ? "null" : Submitted.ToString()
                       , NewRequest ? 1 : 0
                       , SiteId
                       , WorkAreaId
                       , PartnerId
                       , InternalNumberId
                       , CostCenterId
                       , CapitalNumber
                       , CustomerId ?? 0
                       , AccountType == null ? "null" : AccountType.ToString()
                       );
        }

        private static string ins_week = @"
            INSERT INTO [TimeSheetDB].[dbo].[Week]
                       ([WeekNumber]
                       ,[Year]
                       ,[WorkerId]
                       ,[DescriptionId]
                       ,[Comments]
                       ,[IsOvertime]
                       ,[Monday]
                       ,[Tuesday]
                       ,[Wednesday]
                       ,[Thursday]
                       ,[Friday]
                       ,[Saturday]
                       ,[Sunday]
                       ,[Submitted]
                       ,[NewRequest]
                       ,[SiteId]
                       ,[WorkAreaId]
                       ,[PartnerId]
                       ,[InternalNumberId]
                       ,[CostCenterId]
                       ,[CapitalNumber]
                       ,[CustomerId]
                       ,[AccountType])
                 VALUES
                       ({0} --<WeekNumber, int,>
                       ,{1} --<Year, int,>
                       ,{2} --<WorkerId, int,>
                       ,{3} --<DescriptionId, int,>
                       ,'{4}' --<Comments, nvarchar(max),>
                       ,{5} --<IsOvertime, bit,>
                       ,{6} --<Monday, money,>
                       ,{7} --<Tuesday, money,>
                       ,{8} --<Wednesday, money,>
                       ,{9} --<Thursday, money,>
                       ,{10} --<Friday, money,>
                       ,{11} --<Saturday, money,>
                       ,{12} --<Sunday, money,>
                       ,{13} --<Submitted, datetime,>
                       ,{14} --<NewRequest, bit,>
                       ,{15} --<SiteId, int,>
                       ,{16} --<WorkAreaId, int,>
                       ,{17} --<PartnerId, int,>
                       ,{18} --<InternalNumberId, int,>
                       ,{19} --<CostCenterId, int,>
                       ,'{20}' --<CapitalNumber, nvarchar(50),>
                       ,{21} --<CustomerId, int,> 
                       ,{22}) ";

        private static string lock_week = @"
            update [week] set [submitted] = cast('{0}' as datetime)
            where [workerid] = {1} and [weeknumber] = {2}
        ";

        private static string unlock_week = @"
            update [week] set [submitted] = null
            where [workerid] = {1} and [weeknumber] = {2}            
        ";

        private static string upd_week = @"
            UPDATE [TimeSheetDB].[dbo].[Week]
               SET [WeekNumber]              = {1}
                       ,[Year]               = {2}
                       ,[WorkerId]           = {3}
                       ,[DescriptionId]      = {4}
                       ,[Comments]           = '{5}'
                       ,[IsOvertime]         = {6}
                       ,[Monday]             = {7}
                       ,[Tuesday]            = {8}
                       ,[Wednesday]          = {9}
                       ,[Thursday]           = {10}
                       ,[Friday]             = {11}
                       ,[Saturday]           = {12}
                       ,[Sunday]             = {13}
                       ,[Submitted]          = {14}
                       ,[NewRequest]         = {15}
                       ,[SiteId]             = {16}
                       ,[WorkAreaId]         = {17}
                       ,[PartnerId]          = {18}
                       ,[InternalNumberId]   = {19}
                       ,[CostCenterId]       = {20}
                       ,[CapitalNumber]      = '{21}'
                       ,[CustomerId]         = {22}
                       ,[AccountType]         = {23}
             WHERE WeekId = {0} ";

        public static string get_hours = @" select * from week where weekid = {0} or weekid = {1} ";

        internal static int YearWeek(DateTime d)
        {
            var wk = new ISO_8601(d);
            int week = wk.week;
            return d.Year * 100 + week;
        }

        private decimal[] array
        {
            get { return new decimal[] { Monday??0, Tuesday??0, Wednesday??0, Thursday??0, Friday??0, Saturday??0, Sunday??0}; }
        }

        internal decimal Charge(decimal rate, int now, int start, int end, DateTime sdate, DateTime edate)
        {
            decimal hours = 0;

            if (now == start + 1 && now == end - 1)
                hours = array.Skip(((int)sdate.DayOfWeek - 1) % 7).Take(((int)edate.DayOfWeek - 1) % 7 + 1).Sum();

            if (now == start + 1)
                hours = array.Skip(((int)sdate.DayOfWeek - 1) % 7).Sum();

            else if (now == end - 1)
                hours = array.Take(((int)edate.DayOfWeek - 1) % 7 + 1).Sum();

            else
                hours = array.Sum();

            return hours * rate;
        }

        internal void GetLists(int worker)
        {
            using (tsDB db = new tsDB())
            {
                descriptions = db.Fetch<Description>("where workerid = @0", worker);
                descriptions.Add(new Description { DescriptionId = 0, _Description = "" });
                customers = db.Fetch<Customer>("where workerid = @0 or workerid = 0", worker);
                customers.Add(new Customer { CustomerId = 0, CustomerName = "", WorkerId = 0 });         // for RDSS time
                customers.Add(new Customer { CustomerId = 0, CustomerName = "", WorkerId = worker });    // normal customers

                internalNumbers = db.Fetch<InternalNumber>("");
                internalNumbers.Add(new InternalNumber { InternalNumberId = 0, InternalOrder = "" });
                costCenters = db.Fetch<CostCenter>("");
                costCenters.Add(new CostCenter { CostCenterId = 0, _CostCenter = "" });

                workNumbers = db.Fetch<WorkerInternalNumber>("where workerid = @0", worker);
                workNumbers.Add(new WorkerInternalNumber { WorkerInternalNumberId = 0, InternalNumberId = 0, WorkerId = worker });
                workCenters = db.Fetch<WorkerCostCenter>("where workerid = @0", worker);
                workCenters.Add(new WorkerCostCenter { WorkerCostCenterId = 0, CostCenterId = 0, WorkerId = worker });
            }
        }

        internal static List<Week> Get(int worker, int week, int year)
        {
            List<Week> list;
            using (tsDB db = new tsDB())
            {
                list = db.Fetch<Week>(string.Format(Week.lst_week, worker, week));
                if (list == null || list.Count == 0)
                {
                    list = new List<Week>();
                    Week w = new Week(worker, week, year);
                    list.Add(w);
                }
                else
                {
                    list[0].GetLists(worker);
                    foreach (var wk in list.Skip(1))
                    {
                        wk.descriptions = list[0].descriptions;
                        wk.customers = list[0].customers;
                        wk.internalNumbers = list[0].internalNumbers;
                        wk.costCenters = list[0].costCenters;
                    }
                }
            }
            return list;
        }
    }
}