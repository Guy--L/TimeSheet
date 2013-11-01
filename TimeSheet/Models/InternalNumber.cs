using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public partial class InternalNumber
    {
        public static string Save(string internalnumber)
        {
            return string.Format(ins_internalnumber
                , internalnumber
                );
        }

        private static string ins_internalnumber = @"
            INSERT INTO [dbo].[InternalNumber]
                       ([InternalOrder])
                 VALUES
                       ('{0}')
            select scope_identity()
            ";

    }
}