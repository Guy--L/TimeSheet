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
using PetaPoco;

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
            week = 1 + ts.Days / 7;                         // Count of Thursdays 
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
            select distinct w.ionname, m.ionname manager from worker w
                left join week k on w.workerid = k.workerid
                left join worker m on w.managerid = m.workerid
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

            List<Worker> behind;
            using (tsDB db = new tsDB())
            {
                behind = db.Fetch<Worker>(notsubmitted, yearwk);
            }

            if (behind == null || !behind.Any())
                return;

            var viewsPath = Path.GetFullPath(ConfigurationManager.AppSettings["templates"]);
            var engines = new ViewEngineCollection();
            engines.Add(new FileSystemRazorViewEngine(viewsPath));
            var service = new EmailService(engines);

            dynamic email = new Email(sunday ? "FirstReminder" : "SecondReminder");
            email.From = ConfigurationManager.AppSettings["sentfrom"];
            email.Ending = d.ToShortDateString();

            if (sunday)
            {
                var ions = behind.Where(s=>!string.IsNullOrWhiteSpace(s.IonName)).Select(i => i.IonName).ToArray();
                email.To = string.Join("@pg.com, ", ions) + "@pg.com";

                service.Send(email);
            }
            else
            {
                var managers = behind.Select(m => m.manager).Distinct();

                foreach (var mgr in managers)
                {
                    var ions = behind.Where(s => s.manager == mgr && !string.IsNullOrWhiteSpace(s.IonName)).Select(i => i.IonName).ToArray();
                    email.To = string.Join("@pg.com, ", ions) + "@pg.com";

                    email.Cc = ConfigurationManager.AppSettings["copiesto"] + (string.IsNullOrWhiteSpace(mgr) ? "" : (", " + mgr + "@pg.com"));
                    service.Send(email);
                }
            }
        }
    }
}
