using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class Sites : UserBase
    {
        public List<Site> list { get; set; }
    }

    public partial class SiteView : UserBase
    {
        public Site wa { get; set; }
        public SiteView()
        { }

        public SiteView(int id)
        {
            using (tsDB db = new tsDB())
            {
                wa = id == 0 ? (new Site() { SiteId = 0 }) :
                    db.SingleOrDefault<Site>("where SiteId = @0", id);
            }
        }
    }


    public partial class Site
    {
        [ResultColumn]
        public int UseCount { get; set; }
        [ResultColumn]
        public int UserCount { get; set; }

        public static string all = @"
            select i.SiteId
                , i.site
                , (select count(weekid) from week where SiteId = i.SiteId) usecount
                , (select count(distinct workerid) from week where SiteId = i.SiteId) usercount
                from Site i 
        ";
    }

}