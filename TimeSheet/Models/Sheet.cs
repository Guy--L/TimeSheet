using System;
using System.Collections.Generic;
using System.Globalization;
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
            return hours.HasValue?TimeSpan.FromHours(Decimal.ToDouble(hours.Value)).ToString("hh:mm"):"";
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
            return (decimal?) (hrs + mns * (10.0 / 6.0));
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

        public string Description { get; set; }

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
    }

    public class Sheet
    {
        public static Dictionary<int, string> descriptions;
        public static Dictionary<int, string> workAreas;
        public static Dictionary<int, string> customers;
        public static Dictionary<int, string> orders;
        public static Dictionary<int, string> accounts;
        public static Dictionary<int, string> sites;
        public static Dictionary<int, string> partners;

        public Dictionary<int, string> Descriptions { get { return descriptions; } }
        public Dictionary<int, string> WorkAreas { get { return workAreas; } }
        public Dictionary<int, string> Customers { get { return customers; } }
        public Dictionary<int, string> Orders { get { return orders; } }
        public Dictionary<int, string> Accounts { get { return accounts; } }
        public Dictionary<int, string> Sites { get { return sites; } }
        public Dictionary<int, string> Partners { get { return partners; } }

        public Worker employee { get; set; }
        public static string user;
        public int weekNumber;

        public string sunday;
        
        public Week normal;
        public Week overtime;
        
        public string User { get { return user; } set { user = value; } }

        /// <summary>
        /// http://stackoverflow.com/questions/5377851/get-date-range-by-week-number-c-sharp
        /// </summary>
        /// <param name="ww"></param>
        /// <returns></returns>
        public DateTime getPeriodEnding()
        {
            DateTime jan1 = new DateTime(DateTime.Now.Year, 1, 1);

            int daysOffset = DayOfWeek.Tuesday - jan1.DayOfWeek;

            DateTime firstMonday = jan1.AddDays(daysOffset);

            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            var weekNum = weekNumber;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstMonday.AddDays(weekNum * 7 - 1);  // Sunday = 0
            return result;
        }
    }
}