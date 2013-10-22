using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
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
        public int weekNumber { get; set; }
        public string NewDescription { get; set; }
        public string NewCustomer { get; set; }

        public string sunday;

        public Week normal { get; set; }
        public Week overtime { get; set; }
        
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

        public string Save()
        {
            normal.Match(overtime);
            return normal.Save(employee.WorkerId) + overtime.Save(employee.WorkerId);
        }
    }
}