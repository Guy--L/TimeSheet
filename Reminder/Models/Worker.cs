using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetaPoco;

namespace Reminder.Models
{
    public partial class Worker
    {
        [Column] public string manager { get; set; }
    }
}
