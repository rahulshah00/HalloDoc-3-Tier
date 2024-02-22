using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class AdminDashboardViewModel
    {
        public List<AdminRequestsViewModel> adminRequests{ get; set; }   
        public string Username {  get; set; }
    }
}
