using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IAdminActions
    {
        public ViewCaseViewModel ViewCaseAction(int requestid);
        public void AssignCaseAction(int RequestId, string AssignPhysician, string AssignDescription);
        public void CancelCaseAction(int requestid, string Reason, string Description);
        public void BlockCaseAction(int requestid, string blocknotes);
        public void TransferCase(int RequestId, string TransferPhysician, string TransferDescription,int adminid);
        public bool ClearCaseModal(int requestid);
        public void SendOrderAction(int requestid, SendOrderViewModel sendOrder);
        public CloseCaseViewModel CloseCaseGet(int requestid);
        public void CloseCasePost(CloseCaseViewModel model,int requestid);
    }
}
