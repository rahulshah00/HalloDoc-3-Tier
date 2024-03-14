using DAL.ViewModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IRequestRepo
    { 
        public void PRequest(PatientModel pm,string path);
        public void BRequest(BusinessModel bm);
        public void FRequest(FamilyFriendModel ffm);
        public void CRequest(ConciergeModel cm);
    }
}