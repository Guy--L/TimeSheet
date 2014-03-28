using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class InternalNumbers : UserBase
    {
        public List<InternalNumber> list { get; set; }
    }

    public class NumberView : UserBase
    {
        public InternalNumber ino { get; set; }
        public NumberView()
        { }

        public NumberView(int id)
        {
            using (tsDB db = new tsDB())
            {
                ino = id == 0 ? (new InternalNumber() { InternalNumberId = 0 }) :
                    db.SingleOrDefault<InternalNumber>("where InternalNumberId = @0", id);
            }
        }
    }

    public partial class InternalNumber
    {
        [ResultColumn] public int UseCount { get; set; }
        [ResultColumn] public int UserCount { get; set; }

        public static string SaveInPassing(string internalnumber, int workerid)
        {
            return string.Format(ins_internalnumber
                , internalnumber
                , workerid
                );
        }

        public NPoco.Sql Remove(int workerid, string ids)
        {
            var internals = ids.Split(',').Where(s => s != "");
            return new NPoco.Sql(rem_internalnumbers, new { worker = workerid, internals });
        }

        public static string all = @"
            select i.internalnumberid
                , i.internalorder
                , i.legalentity
                , (select count(weekid) from week where accounttype = {0} and internalnumberid = i.internalnumberid) usecount
                , (select count(distinct workerid) from week where accounttype = {0} and internalnumberid = i.internalnumberid) usercount
                from internalnumber i 
            order by (CASE WHEN (i.legalentity IS NULL or i.legalentity = '') THEN 1 ELSE 0 END) DESC, i.internalorder
        ";

        private static string ins_internalnumber = @"
            declare @@newid int
            INSERT INTO InternalNumber ([InternalOrder]) VALUES ('{0}')
            select @@newid = scope_identity()
            insert into workerinternalnumber (workerid, internalnumberid) values ({1}, @@newid)
            select @@newid
            ";

        private static string rem_internalnumbers = @"
            delete from workerinternalnumber
                where workerid = @worker and internalnumberid in (@internals)
            ";
    }
}