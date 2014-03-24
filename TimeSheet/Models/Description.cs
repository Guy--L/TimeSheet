using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public partial class Description
    {
        public static NPoco.Sql Save(int id, string description)
        {
            return Sql.Builder.Append(ins_description
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
                       (@0
                       ,@1
                       ,@2
                       ,@3
                       ,@4
                       ,@5)
            select scope_identity()
            ";

        private static string rem_description = @"
            update description 
                set isactive = 0,
                datelastused = '{1}'
                where descriptionid in ('{0}')
            ";

        public static string Remove(string ids)
        {
            return string.Format(rem_description, ids.Substring(0, ids.Length - 1));
        }

        public static string InActivate(int id)
        {
            return string.Format(inactivate, id, DateTime.Now.ToShortDateString());
        }

        private static string inactivate = @"
            update description 
                set isactive = 0,
                datelastused = '{1}'
                where descriptionid =  {0}
            ";

        public static string Activate(int id)
        {
            return string.Format(activate, id, DateTime.Now.ToShortDateString());
        }

        private static string activate = @"
            update description 
                set isactive = 1,
                datelastused = '{1}'
                where descriptionid =  {0}
            ";

    }
}