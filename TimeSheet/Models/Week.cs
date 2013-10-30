using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public partial class Week
    {
        public static List<CostCenter> costCenters;
        public static List<InternalNumber> internalNumbers;
        public static List<WorkArea> workAreas;
        public static List<Customer> customers;
        public static List<Description> descriptions;
        public static List<Site> sites;
        public static List<Partner> partners;

        public static int NonDemand;

        public Week()
        { }

        public Week(Hrs b, bool isNormal)
        {
            WorkerId = b.WorkerId;
            WeekId = isNormal?b.WeekId:b.oWeekId;
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

            foreach(var w in hrs)
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
        
        public enum ChargeTo
        {
            Internal_Number,
            Cost_Center,
            Capital_Number
        };

        public string ChargeType
        {
            get {
                if (!AccountType.HasValue)
                    return "";
                return Enum.GetName(typeof(ChargeTo), AccountType.Value).Replace('_', ' ');
            }
        }

        public string ChargeNumber
        {
            get
            {
                if (!AccountType.HasValue)
                    return "";
                switch ((ChargeTo)AccountType.Value)
                {
                    case ChargeTo.Cost_Center:
                        var cc = costCenters.FirstOrDefault(i => i.CostCenterId == CostCenterId);
                        return (cc == null) ? "" : cc._CostCenter;

                    case ChargeTo.Internal_Number:
                        var inn = internalNumbers.FirstOrDefault(i => i.InternalNumberId == InternalNumberId);
                        return (inn == null) ? "" : inn.InternalOrder;

                    case ChargeTo.Capital_Number:
                        return CapitalNumber;
                }
                return "";
            }
        }
        
        public string WorkArea          { get {
            return workAreas.FirstOrDefault(i => i.WorkAreaId == (WorkAreaId??0))._WorkArea; } }
        public string Customer          { get {
            var c = customers.FirstOrDefault(i => i.CustomerId == (CustomerId??0));
            if (c == null)
                return "";
            return c.CustomerName; }
        }
        public string Description       { get {
            return descriptions.FirstOrDefault(i => i.DescriptionId == DescriptionId)._Description; } }
        public string Site              { get {
            return sites.FirstOrDefault(i => i.SiteId == (SiteId??0))._Site; } }
        public string Partner           { get {
            return partners.FirstOrDefault(i => i.PartnerId == (PartnerId??0))._Partner; } }

        public string ShortDescription { get { return string.IsNullOrWhiteSpace(Description) ? "" : (Description.Length>20?Description.Substring(0, 20):Description); } }

        public string Mon { get { return time2str(Monday); } set { Monday = time2dec(value); } }
        public string Tue { get { return time2str(Tuesday); } set { Tuesday = time2dec(value); } }
        public string Wed { get { return time2str(Wednesday); } set { Wednesday = time2dec(value); } }
        public string Thu { get { return time2str(Thursday); } set { Thursday = time2dec(value); } }
        public string Fri { get { return time2str(Friday); } set { Friday = time2dec(value); } }
        public string Sat { get { return time2str(Saturday); } set { Saturday = time2dec(value); } }
        public string Sun { get { return time2str(Sunday); } 
            set { Sunday = time2dec(value); } }

        [Column] public int? PairId { get; set; }

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
            return id==0?"":string.Format(del_week, id);
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
            TimeSpan ts = TimeSpan.FromHours(Decimal.ToDouble(hours.Value));
            return string.Format("{0}:{1:00}", (int) ts.TotalHours, ts.Minutes);
        }

        private static decimal? time2dec(string hours)
        {
            if (string.IsNullOrWhiteSpace(hours))
                return null;
            var part = hours.Split(':');
            int hrs = 0, mns = 0;
            try
            {
                hrs = int.Parse(part[0]);
                mns = int.Parse(part[1]);
            }
            catch(Exception e)
            {
                Exception frame = new Exception(hours, e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(frame);
            }
            hrs += mns / 60;
            mns %= 60;
            return (decimal?)(hrs + mns * (10.0 / 6.0));
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
            var description = Week.descriptions[DescriptionId]._Description;
            var intern = InternalNumberId.HasValue ? Week.internalNumbers[InternalNumberId.Value].InternalOrder : CapitalNumber;
            var costctr = CostCenterId.HasValue ? Week.costCenters[CostCenterId.Value]._CostCenter : "";
            var customer = (CustomerId.HasValue && CustomerId.Value > 0) ? Week.customers.First(v => v.CustomerId == CustomerId.Value).CustomerName : "";
            var workarea = WorkAreaId.HasValue ? Week.workAreas[WorkAreaId.Value]._WorkArea : "";
            var partner = PartnerId.HasValue ? Week.partners[PartnerId.Value]._Partner : "";
            var site = SiteId.HasValue ? Week.sites[SiteId.Value]._Site : "";
            var submitted = Submitted.HasValue ?Submitted.Value.ToString("MM/dd hh:mm"):"";

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
                       , IsOvertime?1:0
                       , Monday??0
                       , Tuesday??0
                       , Wednesday??0
                       , Thursday??0
                       , Friday??0
                       , Saturday??0
                       , Sunday??0
                       , Submitted==null?"null":Submitted.ToString()
                       , NewRequest?1:0
                       , SiteId
                       , WorkAreaId
                       , PartnerId
                       , InternalNumberId
                       , CostCenterId
                       , CapitalNumber
                       , CustomerId??0
                       , AccountType==null?"null":AccountType.ToString()
                       );
            }
            return string.Format(upd_week
                       , WeekId
                       , WeekNumber
                       , Year
                       , WorkerId
                       , DescriptionId
                       , Comments
                       , IsOvertime?1:0
                       , Monday??0
                       , Tuesday??0
                       , Wednesday??0
                       , Thursday??0
                       , Friday??0
                       , Saturday??0
                       , Sunday??0
                       , Submitted==null?"null":Submitted.ToString()
                       , NewRequest?1:0
                       , SiteId
                       , WorkAreaId
                       , PartnerId
                       , InternalNumberId
                       , CostCenterId
                       , CapitalNumber
                       , CustomerId??0
                       , AccountType==null?"null":AccountType.ToString()
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
                       ,{22} ";

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
    }
}