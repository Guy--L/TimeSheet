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

            var sheet = db.Fetch<Week>(string.Format("select * from Week where workerid = '{0}' and weeknumber = '{1}'", id, week));
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
