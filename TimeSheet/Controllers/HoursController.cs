using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TimeSheet.Models;

namespace TimeSheet.Controllers
{
    public class HoursController : ApiController
    {
        private tsDB _db;

        private void dbExec(string q)
        {
            try
            {
                _db.Execute(q);
            }
            catch (Exception e)
            {
                Exception frame = new Exception(q, e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(frame);
            }
        }

        // GET api/hours
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/hours/5
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public void Post(PostHours id)
        {
            tsDB _db = new tsDB();

            if (!string.IsNullOrWhiteSpace(id.DescriptionAdd))
            {
                id.normal.DescriptionId = _db.ExecuteScalar<int>(Models.Description.Save(id.normal.WorkerId, id.DescriptionAdd));
            }
            if (!string.IsNullOrWhiteSpace(id.CustomerAdd))
            {
                id.normal.CustomerId = _db.ExecuteScalar<int>(Models.Customer.Save(id.normal.WorkerId, id.CustomerAdd));
            }

            id.overtime.WeekNumber = id.normal.WeekNumber;
            id.normal.NewRequest = id.OrderAdd;         // model binding didn't work for checkbox so we revert to this

            dbExec(id.Save());
        }

        // PUT api/hours/5/32
        public void Put(int id, int week)
        {
            using (_db = new tsDB())
            {
                dbExec(Week.Submit(id, week));
            }
        }

        /// <summary>
        /// Remove normal and overtime records for this weekid
        /// </summary>
        /// <param name="id">weekid</param>
        public void Delete(int id)
        {
            using (_db = new tsDB())
            {
                dbExec(Week.Delete(id));
            }
        }
    }
}
