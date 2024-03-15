using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Interfaces
{
    public interface IResetPasswordService
    {
        public string GenerateJWTTokenForPassword(ForgotPasswordViewModel fvm);
        public string ValidateToken(string token);
    }
}
