using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public partial class timesheet_buffer
    {
        public string[] serialize()
        {
            return new string[13] {
                 ID
                ,TimesheetType
                ,Description
                ,AccountingNumber
                ,GLnumber
                ,Monday_hours
                ,Tuesday_hours
                ,Wednesday_hours
                ,Thursday_hours
                ,Friday_hours
                ,Saturday_hours
                ,Sunday_hours
                ,WeekTotal 
            };
        }
    }

    public class Sheet
    {
        public tblEmployee employee { get; set; }
        public static string user;
        public int weekNumber;
        public string sunday;
        public List<timesheet_buffer> hours;
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