using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Net.Mail;
using System.Threading.Tasks;
using Postal;
using Reminder.Models;
using System.Diagnostics;
using System.Threading;

namespace Reminder
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

    class Program
    {
        internal static int YearWeek(DateTime d)
        {
            var wk = new ISO_8601(d);
            int week = wk.week;
            return d.Year * 100 + week;
        }

        private static string notsubmitted = @"
            select distinct ionname from worker w
                left join week k on w.workerid = k.workerid
                where w.ondisability = 0 and w.isactive = 1 and
                    (k.weekid is null or 
                    (k.year * 100 + k.weeknumber = @0 and k.submitted is null))                                   
        ";

        static void Main(string[] args)
        {
            int yearwk = 0;
            DayOfWeek dow = DateTime.Today.DayOfWeek;
            bool sunday = (dow == DayOfWeek.Sunday);
            DateTime d = sunday ? DateTime.Today : DateTime.Today.AddDays(-(double)dow);
            yearwk = YearWeek(d);

            List<string> behind;
            using (tsDB db = new tsDB())
            {
                behind = db.Fetch<string>(notsubmitted, yearwk);
            }
            var viewsPath = Path.GetFullPath(ConfigurationManager.AppSettings["templates"]);
            var engines = new ViewEngineCollection();
            engines.Add(new FileSystemRazorViewEngine(viewsPath));
            var service = new EmailService(engines);

            dynamic email = new Email(sunday ? "FirstReminder" : "SecondReminder");
            var to = string.Join("@pg.com, ", behind.Where(s => !string.IsNullOrWhiteSpace(s)).ToArray()) + "@pg.com";
            email.To = to;
            var from = ConfigurationManager.AppSettings["sentfrom"];
            email.From = from;
            email.Ending = d.ToShortDateString();

            if (!sunday)
            {
                email.Cc = ConfigurationManager.AppSettings["copiesto"];
            }
            service.Send(email);
        }
    }
}
