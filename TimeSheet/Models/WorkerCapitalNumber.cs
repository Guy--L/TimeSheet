using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public partial class WorkerCapitalNumber
    {
        private static string rem_capitalnumbers = @"
            delete from workercapitalnumber
                where workerid = {0} and workercapitalnumberid in ('{1}')
            ";

        public static string Remove(int workerid, string ids)
        {
            return string.Format(rem_capitalnumbers, workerid, ids.Substring(0, ids.Length - 1));
        }
    }
}