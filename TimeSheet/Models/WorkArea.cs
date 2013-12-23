using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class WorkAreas : UserBase
    {
        public List<WorkArea> list { get; set; }
    }

    public partial class AreaView : UserBase
    {
        public WorkArea wa { get; set; }
        public AreaView()
        { }

        public AreaView(int id)
        {
            using (tsDB db = new tsDB())
            {
                wa = id == 0 ? (new WorkArea() { WorkAreaId = 0 }) :
                    db.SingleOrDefault<WorkArea>("where WorkAreaId = @0", id);
            }
        }
    }


    public partial class WorkArea
    {
        [ResultColumn] public int UseCount { get; set; }
        [ResultColumn] public int UserCount { get; set; }

        public static string all = @"
            select i.workareaid
                , i.workarea
                , (select count(weekid) from week where workareaid = i.workareaid) usecount
                , (select count(distinct workerid) from week where workareaid = i.workareaid) usercount
                from workarea i 
        ";
    }

}