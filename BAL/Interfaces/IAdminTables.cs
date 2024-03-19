using BAL.Repository;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IAdminTables 
    {
        public AdminDashboardViewModel GetNewTable();
        public AdminDashboardViewModel GetActiveTable();
        public AdminDashboardViewModel GetPendingTable();
        public AdminDashboardViewModel GetConcludeTable();
        public AdminDashboardViewModel GetToCloseTable();
        public AdminDashboardViewModel GetUnpaidTable();
        public AdminDashboardViewModel AdminDashboardView();
        public AdminDashboardViewModel AdminDashboard(string email);
    }
}
