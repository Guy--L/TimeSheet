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

    public partial class NumberView : UserBase
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


        public static string SaveInPassing(string internalnumber)
        {
            return string.Format(ins_internalnumber
                , internalnumber
                );
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
            INSERT INTO [dbo].[InternalNumber]
                       ([InternalOrder])
                 VALUES
                       ('{0}')
            select scope_identity()
            ";


    }
}