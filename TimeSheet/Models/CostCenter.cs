using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class CostCenters : UserBase
    {
        public List<CostCenter> list { get; set; }
    }

    public partial class CenterView : UserBase
    {
        public CostCenter cc { get; set; }
        public CenterView()
        { }

        public CenterView(int id)
        {
            using (tsDB db = new tsDB())
            {
                cc = id == 0 ? (new CostCenter() { CostCenterId = 0 }) :
                    db.SingleOrDefault<CostCenter>("where CostCenterId = @0", id);
            }
        }
    }


    public partial class CostCenter
    {
        [ResultColumn] public int UseCount { get; set; }
        [ResultColumn] public int UserCount { get; set; }

        public static string Save(string cc)
        {
            return string.Format(ins_costcenter
                , cc
                );
        }

        public static string all = @"
            select i.costcenterid
                , i.costcenter
                , i.legalentity
                , (select count(weekid) from week where accounttype = {0} and costcenterid = i.costcenterid) usecount
                , (select count(distinct workerid) from week where accounttype = {0} and costcenterid = i.costcenterid) usercount
                from costcenter i 
            order by (CASE WHEN (i.legalentity IS NULL or i.legalentity = '') THEN 1 ELSE 0 END) DESC, i.costcenter
        ";

        private static string ins_costcenter = @"
            INSERT INTO [dbo].[CostCenter]
                       ([CostCenter],[LegalEntity])
                 VALUES
                       ('{0}',0)
            select scope_identity()
            ";

    }
}