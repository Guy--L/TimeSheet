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

    }
}