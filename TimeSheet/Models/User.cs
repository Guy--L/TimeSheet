using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TimeSheet.Models
{
    public class UserBase
    {
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }
        public bool Impersonating { get; set; }
        public string User;
        public string Admin;
    }
}