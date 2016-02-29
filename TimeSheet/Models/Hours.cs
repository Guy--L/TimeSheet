using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TimeSheet.Controllers;

namespace TimeSheet.Models
{
    public class Hrs
    {
        static Hrs()
        {
            Week.GetLists();
            partners = new SelectList(Week.partners, "PartnerId", "_Partner", 0);
            sites = new SelectList(Week.sites, "SiteId", "_Site");
            workAreas = new SelectList(Week.workAreas, "WorkAreaId", "_WorkArea");
        }

        public Hrs()
        {
        }

        public Hrs(Week w)
        {
            if (w == null)
                return;

            if (w.internalNumbers == null)
                w.GetLists(w.WorkerId);

            internalNumbers = new SelectList(w.internalNumbers
                                .Where(i=>w.workNumbers.Exists(n=>n.InternalNumberId == i.InternalNumberId)), "InternalNumberId", "InternalOrder", 0);

            times = new SelectList(w.customers.Where(c => c.WorkerId == 0), "CustomerId", "CustomerName");
            customers = new SelectList(w.customers.Where(c => c.WorkerId != 0 && (c.IsActive || c.CustomerId == 0)), "CustomerId", "CustomerName");
            descriptions = new SelectList(w.descriptions.Where(d => d.IsSelectable || d.DescriptionId == 0), "DescriptionId", "_Description", 0);
            
            costCenters = new SelectList(w.costCenters
                            .Where(i => w.workCenters.Exists(n => n.CostCenterId == i.CostCenterId)), "CostCenterId", "_CostCenter", 0);

            capitalNumbers = new SelectList(w.workCapitals
                            .Where(i => w.workCapitals.Exists(n => n.CapitalNumber == i.CapitalNumber)), "CapitalNumber", "CapitalNumber", 0);

            WorkerId = w.WorkerId;
            WeekNumber = w.WeekNumber;
            Year = w.Year;
            CostCenterId = 0;
            PartnerId = 0;
            SiteId = 0;
            WorkAreaId = 0;
            CustomerId = 0;
            CapitalNumberKey = "";
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
        private SelectList capitalNumbers;
        private SelectList descriptions;

        public SelectList Partners { get { return partners; } }
        public SelectList Sites { get { return sites; } }
        public SelectList Times { get { return times; } }
        public SelectList Customers { get { return customers; } }
        public SelectList WorkAreas { get { return workAreas; } }
        public SelectList CostCenters { get { return costCenters; } }
        public SelectList InternalNumbers { get { return internalNumbers; } }
        public SelectList CapitalNumbers { get { return capitalNumbers;  } }
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

        [Required(ErrorMessage = "Select or Add a Capital Number")]
        public string CapitalNumberAdd { get; set; }
        
        public List<DateTime> Columns { get { return headers; } set { headers = value; } }

        public int WeekId { get; set; }
        public int oWeekId { get; set; }
        public int WeekNumber { get; set; } 		
		public int Year { get; set; } 		
		public int WorkerId { get; set; }

        public string csCustomer { get; set; }
        public string csDescription { get; set; }
        public string csCostCenter { get; set; }
        public string csInternalNumber { get; set; }
        public string csCapitalNumber { get; set; }

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
		
        [Required(ErrorMessage = "<br />Select New or Existing Request")]
		public bool? NewRequest { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a Site")]
        public int? SiteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a WorkArea")]
		public int? WorkAreaId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Select a Partner")]
        public int? PartnerId { get; set; }

        [Required(ErrorMessage = "Select an Internal Order Number"),Range(1, int.MaxValue, ErrorMessage = "Select an Internal Order Number")]
        public int? InternalNumberId { get; set; }

        [Required(ErrorMessage = "Select a Cost Center"), Range(1, int.MaxValue, ErrorMessage = "Select a CostCenter")]
        public int? CostCenterId { get; set; }

        [Required(ErrorMessage="Provide a Capital Account")]
		public string CapitalNumberKey { get; set; } 		

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

        public NPoco.Sql Save()
        {
            CustomerId = (CustomerId == null || CustomerId == 0) ? TimeTypeId : CustomerId;
            WorkAreaId = WorkAreaId ?? 0;
            Week normal = new Week(this, true);
            Week overtime = new Week(this, false);
            var n = normal.SaveWeek();
            var o = overtime.SaveWeek();
            return n==null?o:n.Append( o );
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
            TimeTypeId = b.CustomerId==null?0:b.CustomerId.Value;
            
            CapitalNumberKey = b.CapitalNumber;
            WeekNumber = b.WeekNumber;
            AccountType = b.AccountType;
            Year = b.Year;
            NewRequest = b.NewRequest;
            Submitted = b.Submitted;

            if (AccountType == (int)ChargeTo.Capital_Number)
            {
                List<SelectListItem> items = capitalNumbers.ToList();
                SelectListItem exists = items.FirstOrDefault(i => i.Value == CapitalNumberKey);
                if (exists == null)
                {
                    var stub = new SelectListItem() { Value = CapitalNumberKey, Text = CapitalNumberKey };
                    items.Add(stub);
                    capitalNumbers = new SelectList(items, "Value", "Text", 0);
                }
            }
        }

        /// <summary>
        /// Find the first week (a cultural dependency) then first date of week
        /// </summary>
        /// <param name="year"></param>
        /// <param name="weekOfYear"></param>
        /// <returns></returns>
        public static DateTime FirstDateOfWeek(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var wk = new ISO_8601(firstThursday);
            int firstWeek = wk.week;
            
            var weekNum = weekOfYear;
            if (firstWeek <= 1)
            {
                weekNum -= 1;
            }
            var result = firstThursday.AddDays(weekNum * 7);
            return result.AddDays(-3);
        }
        
        public void UpdateCachedLists()
        {
            using (tsDB _db = new tsDB())
            {
                try {
                    if (!string.IsNullOrEmpty(csDescription)) _db.Execute(Models.Description.UnSelectable(csDescription));
                    if (!string.IsNullOrEmpty(csCustomer)) { var c = new Customer(); _db.Execute(c.Remove(csCustomer)); }
                    if (!string.IsNullOrEmpty(csInternalNumber)) { var i = new InternalNumber(); _db.Execute(i.Remove(WorkerId, csInternalNumber)); }
                    if (!string.IsNullOrEmpty(csCostCenter)) { var c = new CostCenter(); _db.Execute(c.Remove(WorkerId, csCostCenter)); }
                    if (!string.IsNullOrEmpty(csCapitalNumber)) { var w = new WorkerCapitalNumber(); _db.Execute(w.Remove(WorkerId, csCapitalNumber)); }
                }
                catch (Exception e)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception("Last SQL: "+_db.LastSQL+"\n", e));
                }

                if ((DescriptionId == 0) && !string.IsNullOrWhiteSpace(DescriptionAdd))
                {
                    var d = new Description();
                    DescriptionId = _db.ExecuteScalar<int>(d.Save(WorkerId, DescriptionAdd));
                }
                else
                {
                    _db.Execute(Models.Description.Activate(DescriptionId));
                }

                if ((CustomerId == null || CustomerId == 0) && !string.IsNullOrWhiteSpace(CustomerAdd))
                {
                    var c = new Customer();
                    CustomerId = _db.ExecuteScalar<int>(c.Save(WorkerId, CustomerAdd));
                }

                if ((InternalNumberId == null || InternalNumberId == 0) && !string.IsNullOrWhiteSpace(InternalNumberAdd))
                    InternalNumberId = _db.ExecuteScalar<int>(Models.InternalNumber.SaveInPassing(InternalNumberAdd, WorkerId));
                else if (InternalNumberId < 0)
                {
                    InternalNumberId = -InternalNumberId;
                    WorkerInternalNumber win = new WorkerInternalNumber() {
                        InternalNumberId = InternalNumberId.Value,
                        WorkerId = WorkerId
                    };
                    _db.Save<WorkerInternalNumber>(win);
                }
                    
                if ((CostCenterId == null || CostCenterId == 0) && !string.IsNullOrWhiteSpace(CostCenterAdd))
                    CostCenterId = _db.ExecuteScalar<int>(Models.CostCenter.SaveInPassing(CostCenterAdd, WorkerId));
                else if (CostCenterId < 0)
                {
                    CostCenterId = -CostCenterId;
                    WorkerCostCenter wcc = new WorkerCostCenter()
                    {
                        CostCenterId = CostCenterId.Value,
                        WorkerId = WorkerId
                    };
                    _db.Save<WorkerCostCenter>(wcc);
                }

                if (AccountType==(int)ChargeTo.Capital_Number && 
                    CapitalNumberAdd != CapitalNumberKey && 
                    !string.IsNullOrWhiteSpace(CapitalNumberAdd))
                {
                    WorkerCapitalNumber wcn = new WorkerCapitalNumber()
                    {
                        CapitalNumber = CapitalNumberAdd,
                        WorkerId = WorkerId
                    };
                    _db.Save<WorkerCapitalNumber>(wcn);
                }
                
                CapitalNumberKey = CapitalNumberAdd;
            }
        }
    }
}