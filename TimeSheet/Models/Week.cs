using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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

        private static string time2str(decimal? hours)
        {
            return hours.HasValue ? TimeSpan.FromHours(Decimal.ToDouble(hours.Value)).ToString("hh':'mm") : "";
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
                return Monday ?? 0
                    + Tuesday ?? 0
                    + Wednesday ?? 0
                    + Thursday ?? 0
                    + Friday ?? 0
                    + Saturday ?? 0
                    + Sunday ?? 0;
            }
        }

        public string[] serializeDT()
        {
            return new string[19] {
                 WeekId.ToString()
                ,WeekNumber.ToString()
                ,IsOvertime.ToString()
                ,Sheet.descriptions[DescriptionId]
                ,InternalNumberId.HasValue?Sheet.orders[InternalNumberId.Value]:CapitalNumber 
                ,CostCenterId.HasValue?Sheet.accounts[CostCenterId.Value]:""
                ,time2str(Monday)
                ,time2str(Tuesday)
                ,time2str(Wednesday)
                ,time2str(Thursday)
                ,time2str(Friday)
                ,time2str(Saturday)
                ,time2str(Sunday)
                ,time2str(subTotal)
                ,CustomerId.HasValue?Sheet.customers[CustomerId.Value]:""
                ,WorkAreaId.HasValue?Sheet.workAreas[WorkAreaId.Value]:""
                ,NewRequest.ToString()
                ,PartnerId.HasValue?Sheet.partners[PartnerId.Value]:""
                ,SiteId.HasValue?Sheet.sites[SiteId.Value]:""
            };
        }

        public string Save()
        {
            if (WeekId == 0 && subTotal > 0)
                return string.Format(ins_week
                       , WeekNumber
                       , Year
                       , DescriptionId
                       , Comments
                       , IsOvertime
                       , Monday
                       , Tuesday
                       , Wednesday
                       , Thursday
                       , Friday
                       , Saturday
                       , Sunday
                       , Submitted
                       , NewRequest
                       , SiteId
                       , WorkAreaId
                       , PartnerId
                       , InternalNumberId
                       , CostCenterId
                       , CapitalNumber
                       , CustomerId);

            return string.Format(upd_week
                       , WeekId
                       , WeekNumber
                       , Year
                       , DescriptionId
                       , Comments
                       , IsOvertime
                       , Monday
                       , Tuesday
                       , Wednesday
                       , Thursday
                       , Friday
                       , Saturday
                       , Sunday
                       , Submitted
                       , NewRequest
                       , SiteId
                       , WorkAreaId
                       , PartnerId
                       , InternalNumberId
                       , CostCenterId
                       , CapitalNumber
                       , CustomerId);
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
                       ,{4} --<Comments, nvarchar(max),>
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
                       ,{20} --<CapitalNumber, nvarchar(50),>
                       ,{21} --<CustomerId, int,>)
               select @@scope_identity() ";

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
             WHERE WeekId = {0}";
    }
}