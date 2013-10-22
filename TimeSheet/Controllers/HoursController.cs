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

        // POST api/hours
        public void Post(Sheet hours)
        {
            tsDB db = new tsDB();
            if (!string.IsNullOrWhiteSpace(hours.NewDescription))
            {
                hours.normal.DescriptionId = db.ExecuteScalar<int>(Models.Description.Save(hours.employee.WorkerId, hours.NewDescription));
                hours.overtime.DescriptionId = hours.normal.DescriptionId;
            }
            if (!string.IsNullOrWhiteSpace(hours.NewCustomer))
            {
                hours.normal.CustomerId = db.ExecuteScalar<int>(Models.Customer.Save(hours.employee.WorkerId, hours.NewCustomer));
                hours.overtime.CustomerId = hours.normal.CustomerId;
            }
            hours.normal.WeekNumber = hours.weekNumber;
            hours.overtime.WeekNumber = hours.weekNumber;
            try
            {
                db.Execute(hours.Save());
            }
            catch(Exception e)
            {
                Elmah.ErrorSignal.FromCurrentContext().Raise(e);
            }
        }

        // PUT api/hours/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/hours/5
        public void Delete(int id)
        {
        }
    }
}
