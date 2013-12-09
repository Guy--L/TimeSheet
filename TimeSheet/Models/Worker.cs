using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class Personnel
    {
        public bool IsAdmin { get; set; }
        public string User;

        public IEnumerable<Worker> list { get; set; }
    }

    public partial class Worker
    {
        public static string all = @"
            select w.*, l.level, s.site, d.WorkDeptDesc, r.Role
            from worker w
            left join [level] l on w.LevelId = l.LevelId
            left join site s on w.FacilityId = s.SiteId
            left join workdept d on w.WorkDeptId = d.WorkDeptId
            left join role r on w.RoleId = r.RoleId
        ";
        [ResultColumn] public string level { get; set; }
        [ResultColumn] public string site { get; set; }
        [ResultColumn] public string WorkDeptDesc { get; set; }
        [ResultColumn] public string role { get; set; }
    }
}