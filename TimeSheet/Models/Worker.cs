using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NPoco;

namespace TimeSheet.Models
{
    public class Personnel : UserBase
    {
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
        public static string delegates = all + @" where w.ManagerId = {0}";

        [ResultColumn] public string level { get; set; }
        [ResultColumn] public string site { get; set; }
        [ResultColumn] public string WorkDeptDesc { get; set; }
        [ResultColumn] public string role { get; set; }
    }

    public class WorkerView : UserBase
    {
        public Worker w { get; set; }
        public SelectList levels;
        public SelectList sites;
        public SelectList depts;
        public SelectList roles;
        public SelectList managers;

        public WorkerView()
        { }

        public WorkerView(int id)
        {
            using (tsDB db = new tsDB())
            {
                if (id > 0) 
                    w = db.SingleOrDefault<Worker>("where WorkerId = @0", id);
                else {
                    w = new Worker() {
                        LevelId = 0,
                        FacilityId = 0,
                        WorkDeptId = 0,
                        RoleId = 0
                    };
                }

                levels = AddNone(new SelectList(db.Fetch<Level>(""), "LevelId", "_Level", w.LevelId));
                sites = AddNone(new SelectList(db.Fetch<Site>(""), "SiteId", "_Site", w.FacilityId));
                depts = AddNone(new SelectList(db.Fetch<WorkDept>(""), "WorkDeptId", "WorkDeptDesc", w.WorkDeptId));
                roles = AddNone(new SelectList(db.Fetch<Role>(""), "RoleId", "_Role", w.RoleId));
                managers = AddNone(new SelectList(db.Fetch<Worker>("where IsManager = 1"), "WorkerId", "LastName", w.ManagerId));
            }
        }

        private SelectList AddNone(SelectList list)
        {
            List<SelectListItem> _list = list.ToList();
            _list.Insert(0, new SelectListItem() { Value = "0", Text = "" });
            return new SelectList((IEnumerable<SelectListItem>)_list, "Value", "Text");
        }
    }
}