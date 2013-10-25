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

        public dynamic Get(string id, int week)
        {
            tsDB db = new tsDB();
            try
            {
                var sheet = db.Fetch<Week>(string.Format(Week.lst_week, id, week));
                var records = sheet.Select(a => a.serializeDT()).ToArray();
                return new
                {
                    sEcho = 1,
                    iTotalRecords = records.Count(),
                    iTotalDisplayRecords = records.Count(),
                    aaData = records
                };
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
                return null;
            }
        }

        // POST api/hours
        [HttpPost]
        public int[] Post(Sheet hours)
        {
            _db = new tsDB();

            if (!string.IsNullOrWhiteSpace(hours.NewDescription))
            {
                hours.normal.DescriptionId = _db.ExecuteScalar<int>(Models.Description.Save(hours.employee.WorkerId, hours.NewDescription));
                hours.overtime.DescriptionId = hours.normal.DescriptionId;
            }
            if (!string.IsNullOrWhiteSpace(hours.NewCustomer))
            {
                hours.normal.CustomerId = _db.ExecuteScalar<int>(Models.Customer.Save(hours.employee.WorkerId, hours.NewCustomer));
                hours.overtime.CustomerId = hours.normal.CustomerId;
            }
            hours.normal.WeekNumber = hours.weekNumber;
            hours.overtime.WeekNumber = hours.weekNumber;
            hours.normal.NewRequest = hours.NewRequest;         // model binding didn't work for checkbox so we revert to this

            string q = "";
            Tuple<List<int>, List<int>> ids = null;
            try
            {
                q = hours.Save();
                ids = _db.FetchMultiple<int, int>(q);
                return new int[2] { ids.Item1.FirstOrDefault(), ids.Item2.FirstOrDefault() };
            }
            catch (Exception e)
            {
                Exception frame = new Exception(q, e);
                Elmah.ErrorSignal.FromCurrentContext().Raise(frame);
                return null;
            }
        }

        // PUT api/hours/5/32
        [HttpPut]
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
        [HttpDelete]
        public void Delete(int id)
        {
            using (_db = new tsDB())
            {
                dbExec(Week.Delete(id));
            }
        }
    }
}
