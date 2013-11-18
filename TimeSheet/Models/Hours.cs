using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeSheet.Models
{
    public class AccType
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Hrs
    {
        static Hrs()
        {
            partners = new SelectList(Week.partners, "PartnerId", "_Partner", 0);
            sites = new SelectList(Week.sites, "SiteId", "_Site");
            workAreas = new SelectList(Week.workAreas, "WorkAreaId", "_WorkArea");

            headers = Sheet.headers;
        }

        public Hrs()
        {
        }

        public Hrs(Week w)
        {
            if (w.internalNumbers == null)
                w.GetLists(w.WorkerId);

            internalNumbers = new SelectList(w.internalNumbers, "InternalNumberId", "InternalOrder");
            times = new SelectList(w.customers.Where(c => c.WorkerId == 0), "CustomerId", "CustomerName");
            customers = new SelectList(w.customers.Where(c => c.WorkerId != 0), "CustomerId", "CustomerName");
            descriptions = new SelectList(w.descriptions, "DescriptionId", "_Description", 0);
            costCenters = new SelectList(w.costCenters, "CostCenterId", "_CostCenter");

            WorkerId = w.WorkerId;
            WeekNumber = w.WeekNumber;
            Year = w.Year;
            CostCenterId = 0;
            PartnerId = 0;
            SiteId = 0;
            WorkAreaId = 0;
            CustomerId = 0;
            ChargeAccount = ChargeTo.Cost_Center;
        }

        private static List<DateTime> headers;
        private static SelectList partners;
        private static SelectList sites;
        private static SelectList times;
        private static SelectList workAreas;

        private SelectList customers;
        private SelectList costCenters;
        private SelectList internalNumbers;
        private SelectList descriptions;

        public SelectList Partners { get { return partners; } }
        public SelectList Sites { get { return sites; } }
        public SelectList Times { get { return times; } }
        public SelectList Customers { get { return customers; } }
        public SelectList WorkAreas { get { return workAreas; } }
        public SelectList CostCenters { get { return costCenters; } }
        public SelectList InternalNumbers { get { return internalNumbers; } }
        public SelectList Descriptions { get { return descriptions; } }

        [Range(1, int.MaxValue, ErrorMessage = "Select a Type of activity")]
        public int TimeTypeId { get; set; }

        [Required(ErrorMessage = "Select or Add a Customer")]
        public string CustomerAdd { get; set; }

        [Required(ErrorMessage = "Select or Add a Cost Center")]
        public string CostCenterAdd { get; set; }

        [Required(ErrorMessage = "Select or Add a Description")]
        public string DescriptionAdd { get; set; }

        [Required(ErrorMessage = "Select or Add an Internal Order")]
        public string InternalNumberAdd { get; set; }

        public List<DateTime> Columns { get { return headers; } set { headers = value; } }

        public int WeekId { get; set; }
        public int oWeekId { get; set; }
        public int WeekNumber { get; set; } 		
		public int Year { get; set; } 		
		public int WorkerId { get; set; }

        [Required(ErrorMessage = "Select or Add a Description")]
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

        [Range(1, int.MaxValue, ErrorMessage = "Select a Site")]
        public int? SiteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a WorkArea")]
		public int? WorkAreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a Partner")]
        public int? PartnerId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select an Internal Order Number")]
        public int? InternalNumberId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a CostCenter")]
        public int? CostCenterId { get; set; }

        [Required(ErrorMessage="Provide a Capital Account")]
		public string CapitalNumber { get; set; } 		

        [Required(ErrorMessage="Select or Add a Customer")]
		public int? CustomerId { get; set; }
        
        [Required(ErrorMessage="Select a Charge Account")]
        public int? AccountType { get; set; }

        [Range(1, 2440, ErrorMessage = "Enter Hours")]
        public int TotalMinutes { get; set; }

        public ChargeTo? ChargeAccount 
        { 
            get { 
                return AccountType==null?(Nullable<ChargeTo>) null:(ChargeTo)AccountType; 
            }
            set
            {
                AccountType = (int?)value;
            }
        }

        //public static Dictionary<int, string> chargeTo = new Dictionary<int, string>
        //{
        //    {0, "Cost Center" },
        //    {1, "Internal Number"},
        //    {2, "Capital Number"}
        //};

        //public Dictionary<int, string> ChargeTo { get { return chargeTo; } }

        public string Save()
        {
            CustomerId = (CustomerId == null || CustomerId == 0) ? TimeTypeId : CustomerId;
            WorkAreaId = WorkAreaId ?? 0;
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
            AccountType = b.AccountType;
            Year = b.Year;
            NewRequest = b.NewRequest;
        }

        public void AddIfNew()
        {
            using (tsDB _db = new tsDB())
            {
                if ((DescriptionId == null || DescriptionId == 0) && !string.IsNullOrWhiteSpace(DescriptionAdd))
                    DescriptionId = _db.ExecuteScalar<int>(Models.Description.Save(WorkerId, DescriptionAdd));
                else
                {
                    _db.Execute(Models.Description.Activate(DescriptionId));
                }

                if ((CustomerId == null || CustomerId == 0) && !string.IsNullOrWhiteSpace(CustomerAdd))
                    CustomerId = _db.ExecuteScalar<int>(Models.Customer.Save(WorkerId, CustomerAdd));

                if ((InternalNumberId == null || InternalNumberId == 0) && !string.IsNullOrWhiteSpace(InternalNumberAdd))
                    InternalNumberId = _db.ExecuteScalar<int>(Models.InternalNumber.Save(InternalNumberAdd));

                if ((CostCenterId == null || CostCenterId == 0) && !string.IsNullOrWhiteSpace(CostCenterAdd))
                    CostCenterId = _db.ExecuteScalar<int>(Models.CostCenter.Save(CostCenterAdd));
            }
        }
    }
}