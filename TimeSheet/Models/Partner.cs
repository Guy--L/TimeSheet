using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class Partners : UserBase
    {
        public List<Partner> list { get; set; }
    }

    public partial class PartnerView : UserBase
    {
        public Partner wa { get; set; }
        public PartnerView()
        { }

        public PartnerView(int id)
        {
            using (tsDB db = new tsDB())
            {
                wa = id == 0 ? (new Partner() { PartnerId = 0 }) :
                    db.SingleOrDefault<Partner>("where PartnerId = @0", id);
            }
        }
    }


    public partial class Partner
    {
        [ResultColumn]
        public int UseCount { get; set; }
        [ResultColumn]
        public int UserCount { get; set; }

        public static string all = @"
            select i.PartnerId
                , i.Partner
                , (select count(weekid) from week where PartnerId = i.PartnerId) usecount
                , (select count(distinct workerid) from week where PartnerId = i.PartnerId) usercount
                from Partner i 
        ";
    }
}