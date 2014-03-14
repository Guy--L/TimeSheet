using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public class Customers : UserBase
    {
        public List<Customer> list { get; set; }
    }

    public partial class Customer
    {
        public static string Save(int id, string customer)
        {
            return string.Format(ins_customer
                , id
                , customer
                );
        }

        private static string ins_customer = @"
            INSERT INTO [dbo].[Customer]
                       ([WorkerId]
                       ,[CustomerName])
                 VALUES
                       ({0}, '{1}')
            select scope_identity()
            ";

        public static string Remove(string ids)
        {
            return string.Format(rem_customer, ids.Substring(0,ids.Length-1));
        }

        private static string rem_customer = @"
            update customer
                set isactive = 0
                where customerid in ('{0}')
            ";
    }
}