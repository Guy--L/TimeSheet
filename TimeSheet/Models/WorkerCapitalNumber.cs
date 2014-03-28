using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public partial class WorkerCapitalNumber
    {
        private static string rem_capitalnumbers = @"
            delete from workercapitalnumber
                where workerid = @workerid and capitalnumber in (@caps)
            ";

        public NPoco.Sql Remove(int workerid, string ids)
        {
            var caps = ids.Split(',').Where(s => s != "");
            var sql = new Sql();
            return sql.Append(rem_capitalnumbers, new { workerid, caps });
        }
    }
}