using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public class ISO_8601
    {
        public int year;
        public int week;
        public int dayOfWeek;

        public ISO_8601(DateTime dt)
        {
            dayOfWeek = 1 + ((int)dt.DayOfWeek + 1 + 5) % 7; // Mon=1 to Sun=7
            DateTime NearestThu = dt.AddDays(4 - dayOfWeek);
            year = NearestThu.Year;
            DateTime Jan1 = new DateTime(year, 1, 1);
            TimeSpan ts = NearestThu.Subtract(Jan1);
            week = 1 + ts.Days / 7; // Count of Thursdays 
        }
    }
}