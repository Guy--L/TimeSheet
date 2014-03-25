using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NPoco;

namespace TimeSheet.Models
{
    public partial class Description
    {
        public NPoco.Sql Save(int id, string description)
        {
            var sql = new Sql();
            return sql.Append(ins_description
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

        private static string select_description = @"
            update description 
                set isselectable = {0},
                datelastused = '{2}'
                where descriptionid in ({1})
            ";

        public static string UnSelectable(string ids)
        {
            return string.Format(select_description, 0, ids.Substring(0, ids.Length - 1), DateTime.Now.ToString("d"));
        }

        public static string ReSelectable(string ids)
        {
            return string.Format(select_description, 1, ids.Substring(0, ids.Length - 1), DateTime.Now.ToString("d"));
        }

        public static string InActivate(int id)
        {
            return string.Format(active_description, id, DateTime.Now.ToShortDateString(), 0);
        }

        public static string Activate(int id)
        {
            return string.Format(active_description, id, DateTime.Now.ToShortDateString(), 1);
        }

        private static string active_description = @"
            update description 
                set isactive = {2},
                datelastused = '{1}'
                where descriptionid = {0}
            ";

    }
}