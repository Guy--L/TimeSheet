using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeSheet.Models
{
    public class Hrs
    {
        static Hrs()
        {
            partners = new SelectList(Week.partners, "PartnerId", "_Partner");
            sites = new SelectList(Week.sites, "SiteId", "_Site");
            internalNumbers = new SelectList(Week.internalNumbers, "InternalNumberId", "InternalOrder");
            workAreas = new SelectList(Week.workAreas, "WorkAreaId", "_WorkArea");
            times = new SelectList(Week.customers.Where(c => c.WorkerId == 0), "CustomerId", "CustomerName");
            customers = new SelectList(Week.customers.Where(c => c.WorkerId != 0), "CustomerId", "CustomerName");
            descriptions = new SelectList(Week.descriptions, "DescriptionId", "_Description");
            costCenters = new SelectList(Week.costCenters, "CostCenterId", "_CostCenter");
            headers = Sheet.headers;
        }

        private static List<DateTime> headers;
        private static SelectList partners;
        private static SelectList sites;
        private static SelectList times;
        private static SelectList customers;
        private static SelectList workAreas;
        private static SelectList costCenters;
        private static SelectList internalNumbers;
        private static SelectList descriptions;

        public SelectList Partners { get { return partners; } }
        public SelectList Sites { get { return sites; } }
        public SelectList Times { get { return times; } }
        public SelectList Customers { get { return customers; } }
        public SelectList WorkAreas { get { return workAreas; } }
        public SelectList CostCenters { get { return costCenters; } }
        public SelectList InternalNumbers { get { return internalNumbers; } }
        public SelectList Descriptions { get { return descriptions; } }

        public int TimeTypeId { get; set; }
        public string CustomerAdd { get; set; }
        public string DescriptionAdd { get; set; }
        public string InternalNumberAdd { get; set; }

        public List<DateTime> Columns { get { return headers; } set { headers = value; } }

        public int WeekId { get; set; }
        public int oWeekId { get; set; }
        public int WeekNumber { get; set; } 		
		public int Year { get; set; } 		
		public int WorkerId { get; set; } 		
		public int DescriptionId { get; set; } 		
		public string Comments { get; set; } 		
		public bool IsOvertime { get; set; } 		
		public string nMon { get; set; } 		
		public string nTue { get; set; } 		
		public string nWed { get; set; } 		
		public string nThu { get; set; } 		
		public string nFri { get; set; } 		
		public string nSat { get; set; } 		
		public string nSun { get; set; }

        public string oMon { get; set; }
        public string oTue { get; set; }
        public string oWed { get; set; }
        public string oThu { get; set; }
        public string oFri { get; set; }
        public string oSat { get; set; }
        public string oSun { get; set; } 		

		public DateTime? Submitted { get; set; } 		
		public bool NewRequest { get; set; } 		
		public int? SiteId { get; set; } 		
		public int? WorkAreaId { get; set; } 		
		public int? PartnerId { get; set; } 		
		public int? InternalNumberId { get; set; } 		
		public int? CostCenterId { get; set; } 		
		public string CapitalNumber { get; set; } 		
		public int? CustomerId { get; set; }

        public string Save()
        {
            Week normal = new Week(this, true);
            Week overtime = new Week(this, false);
            return normal.SaveWeek() + overtime.SaveWeek();
        }

        public void CopyHeader(Week b)
        {
            if (b == null)
                return;

            DescriptionId = b.DescriptionId;
            CustomerId = b.CustomerId;
            SiteId = b.SiteId;
            PartnerId = b.PartnerId;
            InternalNumberId = b.InternalNumberId;
            WorkAreaId = b.WorkAreaId;
            CostCenterId = b.CostCenterId;
            CapitalNumber = b.CapitalNumber;
            WeekNumber = b.WeekNumber;
            Year = b.Year;
        }
    }
}