﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PetaPoco;

namespace TimeSheet.Models
{
    public partial class Week
    {
        public string Mon { get { return time2str(Monday); } set { Monday = time2dec(value); } }
        public string Tue { get { return time2str(Tuesday); } set { Tuesday = time2dec(value); } }
        public string Wed { get { return time2str(Wednesday); } set { Wednesday = time2dec(value); } }
        public string Thu { get { return time2str(Thursday); } set { Thursday = time2dec(value); } }
        public string Fri { get { return time2str(Friday); } set { Friday = time2dec(value); } }
        public string Sat { get { return time2str(Saturday); } set { Saturday = time2dec(value); } }
        public string Sun { get { return time2str(Sunday); } set { Sunday = time2dec(value); } }
        [Column] public int PairId { get; set; }

        public static string lst_week = @"
            select b.WeekId PairId, a.* from week a
                left join week b 
                     on a.CapitalNumber = b.CapitalNumber 
	                and a.CostCenterId = b.CostCenterId
	                and a.CustomerId = b.CustomerId
	                and a.DescriptionId = b.DescriptionId
	                and a.InternalNumberId = b.InternalNumberId
	                and a.PartnerId = b.PartnerId
	                and a.SiteId = b.SiteId
	                and a.WorkAreaId = b.WorkAreaId
	                and a.NewRequest = b.NewRequest
	                and a.IsOverTime != b.IsOvertime
                    and a.workerid = b.workerid
                    and a.weeknumber = b.weeknumber
                    and a.year = b.year
                 where a.workerid = '{0}' and a.weeknumber = '{1}' 
        ";

        private static string del_week = @"
            delete a, b from week a
                left join week b
                     on a.CapitalNumber = b.CapitalNumber 
	                and a.CostCenterId = b.CostCenterId
	                and a.CustomerId = b.CustomerId
	                and a.DescriptionId = b.DescriptionId
	                and a.InternalNumberId = b.InternalNumberId
	                and a.PartnerId = b.PartnerId
	                and a.SiteId = b.SiteId
	                and a.WorkAreaId = b.WorkAreaId
	                and a.NewRequest = b.NewRequest
	                and a.IsOverTime != b.IsOvertime
                    and a.workerid = b.workerid
                    and a.weeknumber = b.weeknumber
                    and a.year = b.year
                 where a.weekid = {0}
        ";

        public static string Delete(int id)
        {
            return string.Format(del_week, id);
        }

        private static string time2str(decimal? hours)
        {
            if (!hours.HasValue)
                return "";
            TimeSpan ts = TimeSpan.FromHours(Decimal.ToDouble(hours.Value));
            return string.Format("{0}:{1:00}", (int) ts.TotalHours, ts.Minutes);
        }

        private static decimal? time2dec(string hours)
        {
            if (string.IsNullOrWhiteSpace(hours))
                return null;
            var part = hours.Split(':');
            var hrs = int.Parse(part[0]);
            var mns = int.Parse(part[1]);
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

        public string[] serializeDT()
        {
            var tot = subTotal;
            var description = Sheet.descriptions[DescriptionId];
            var intern = InternalNumberId.HasValue?Sheet.orders[InternalNumberId.Value]:CapitalNumber;
            var costctr = CostCenterId.HasValue?Sheet.accounts[CostCenterId.Value]:"";
            var customer = (CustomerId.HasValue&&CustomerId.Value>0)?Sheet.customers.First(v=>v.CustomerId == CustomerId.Value).CustomerName:"";
            var workarea = WorkAreaId.HasValue?Sheet.workAreas[WorkAreaId.Value]:"";
            var partner = PartnerId.HasValue?Sheet.partners[PartnerId.Value]:"";
            var site = SiteId.HasValue?Sheet.sites[SiteId.Value]:"";
            var submitted = !(Submitted == null);

            return new string[27] {
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
                ,PairId.ToString()                  // 25
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
                && CostCenterId == b.CostCenterId;
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

        public string Save(int id)
        {
            if (WeekId == 0)
            {
                if (subTotal <= 0)
                    return "";

                return string.Format(ins_week
                       , WeekNumber
                       , Year
                       , id
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
                       , CustomerId??0);
            }
            return string.Format(upd_week
                       , WeekId
                       , WeekNumber
                       , Year
                       , id
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
                       , CustomerId??0);
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
                       ,[CustomerId])
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
                       ,{21}) --<CustomerId, int,> ";

        private static string lock_week = @"
            update [week] set [submitted] = {0}
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
             WHERE WeekId = {0} ";
    }
}