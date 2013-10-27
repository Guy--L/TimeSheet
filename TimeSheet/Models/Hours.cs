using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TimeSheet.Models
{
    public class Hours
    {
        static Hours()
        {
            partners = new SelectList(Week.partners, "PartnerId", "_Partner");
            sites = new SelectList(Week.sites, "SiteId", "SiteName");
            internalNumbers = new SelectList(Week.internalNumbers, "InternalNumberId", "InternalOrder");
            workAreas = new SelectList(Week.workAreas, "WorkAreaId", "_WorkArea");
            times = new SelectList(Week.customers.Where(c => c.WorkerId == 0), "CustomerId", "CustomerName");
            customers = new SelectList(Week.customers.Where(c => c.WorkerId != 0), "CustomerId", "CustomerName");
            descriptions = new SelectList(Week.descriptions, "DescriptionId", "_Description");
            costCenters = new SelectList(Week.costCenters, "CostCenterId", "_CostCenter");
        }

        private static SelectList partners;
        private static SelectList sites;
        private static SelectList times;
        private static SelectList customers;
        private static SelectList workAreas;
        private static SelectList costCenters;
        private static SelectList internalNumbers;
        private static SelectList descriptions;

        public SelectList Partners          { get { return partners; }}
        public SelectList Sites             { get { return sites; }}
        public SelectList Times             { get { return times; } }
        public SelectList Customers         { get { return customers; } }
        public SelectList WorkAreas         { get { return workAreas; } }
        public SelectList CostCenters       { get { return costCenters; } }
        public SelectList InternalNumbers   { get { return internalNumbers; } }
        public SelectList Descriptions      { get { return descriptions; } }

        public int TimeTypeId { get; set; }
        public string NewCustomer { get; set; }
        public string NewDescription { get; set; }
        public string NewInternalNumber { get; set; }
        public bool NewRequest { get; set; }

        public Week normal { get; set; }
        public Week overtime { get; set; }

        public List<DateTime> Headers { get; set; }
    }
}