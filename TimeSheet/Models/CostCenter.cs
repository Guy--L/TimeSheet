using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public class CostCenters : UserBase
    {
        public List<CostCenter> list { get; set; }
    }

    public partial class CostCenter
    {
        public int UseCount { get; set; }
        public int UserCount { get; set; }

        public static string Save(string cc)
        {
            return string.Format(ins_costcenter
                , cc
                );
        }

        private static string ins_costcenter = @"
            INSERT INTO [dbo].[CostCenter]
                       ([CostCenter],[LegalEntity])
                 VALUES
                       ('{0}',0)
            select scope_identity()
            ";

    }
}