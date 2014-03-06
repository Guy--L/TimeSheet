using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public class Missing : UserBase
    {
        internal static int YearWeek(DateTime d)
        {
            var wk = new ISO_8601(d);
            int week = wk.week;
            return d.Year * 100 + week;
        }

        private static string notsubmitted = @"
            select distinct
                 w.ionname
                , w.[FirstName]
                , w.[LastName]
                , coalesce(m.[IonName],'') as ManagerIon 
                from worker w
                left join 
					(select [year], workerid, weeknumber, submitted from week 
						where [year] * 100 + weeknumber = @0) k on w.workerid = k.workerid
                left join worker m on w.managerid = m.workerid
                where w.ondisability = 0 and w.isactive = 1 and k.submitted is null
                order by w.[LastName], w.[FirstName]
                ";

        public DateTime requestdate { get; set; }
        public List<Worker> workers { get; set; }

        public void Check()
        {
            using (tsDB db = new tsDB())
            {
                int yrwk = YearWeek(requestdate);
                workers = db.Fetch<Worker>(notsubmitted, yrwk);
            }
        }
    }
}