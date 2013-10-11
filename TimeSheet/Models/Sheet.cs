using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public partial class Week
    {
        private static string time(decimal? hours)
        {
            return TimeSpan.FromHours(Decimal.ToDouble(hours ?? 0)).ToString("hh:mm");
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

        public string[] serialize()
        {
            return new string[19] {
                 WeekId.ToString()
                ,WeekNumber.ToString()
                ,IsOvertime.ToString()
                ,Sheet.descriptions[DescriptionId]
                ,InternalNumberId.HasValue?Sheet.orders[InternalNumberId.Value]:CapitalNumber 
                ,CostCenterId.HasValue?Sheet.accounts[CostCenterId.Value]:""
                ,time(Monday)
                ,time(Tuesday)
                ,time(Wednesday)
                ,time(Thursday)
                ,time(Friday)
                ,time(Saturday)
                ,time(Sunday)
                ,time(subTotal)
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
        public Week edit;
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