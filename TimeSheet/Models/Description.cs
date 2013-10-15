using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public partial class Description
    {
        public static string Save(int id, string description)
        {
            return string.Format(ins_description
                , id
                , description
                , ""
                , description.GetHashCode()
                , 1
                , DateTime.Now.ToShortDateString()
                );
        }

        private static string ins_description = @"
            INSERT INTO [TimeSheetDB].[dbo].[Description]
                       ([WorkerId]
                       ,[Description]
                       ,[AccountNumber]
                       ,[HashCode]
                       ,[IsActive]
                       ,[DateLastUsed])
                 VALUES
                       ({0} -- <WorkerId, int,>
                       ,'{1}' -- <Description, nvarchar(max),>
                       ,'{2}' -- <AccountNumber, nchar(10),>
                       ,{3}   -- <HashCode, int,>
                       ,{4}   -- <IsActive, bit,>
                       ,'{5}' -- <DateLastUsed, datetime,>)
            select @@scope_identity()
            ";
    }
}