using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminRequestsViewModel
    {
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        public string Requestor { get; set; }
        public DateTime Requesteddate { get; set; }
        public string PhoneNo { get; set; }
        public string Address { get; set; }
        public string Notes { get; set; }
        public string OtherPhoneNo{ get; set; }
        
    }
}
