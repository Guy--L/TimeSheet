using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public class Customers : UserBase
    {
        public List<Customer> list { get; set; }
    }

    public partial class Customer
    {
        public NPoco.Sql Save(int id, string customer)
        {
            var sql = new Sql();
            return sql.Append(ins_customer
                , id
                , customer
                );
        }

        private static string ins_customer = @"
            INSERT INTO [dbo].[Customer]
                       ([WorkerId]
                       ,[CustomerName])
                 VALUES
                       (@0, @1)
            select scope_identity()
            ";

        public NPoco.Sql Remove(string ids)
        {
            var custs = ids.Split(',').Where(s => s != "");
            return new Sql(rem_customers, new { custs });
        }

        private static string rem_customers = @"
            update customer
                set isactive = 0
                where customerid in (@custs)
            ";
    }
}