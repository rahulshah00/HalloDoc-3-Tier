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
        public int New {  get; set; }
        public int conclude {  get; set; }
        public int unpaid {  get; set; }
        public int pending {  get; set; }
        public int active {  get; set; }
        public int toclose { get; set; }
        
    }
}