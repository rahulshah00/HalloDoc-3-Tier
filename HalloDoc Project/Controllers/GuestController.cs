using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BAL.Repository;

namespace HalloDoc_Project.Controllers
{
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IRequestRepo _patient_Request;
        private readonly IJwtToken _jwtToken;
        private readonly IResetPasswordService _resetPasswordService;
        private readonly IEmailService _emailService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAgreement _agreement;
        public GuestController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IRequestRepo request, IJwtToken token, IResetPasswordService resetPasswordService, IEmailService emailService, IPasswordHasher passwordHasher, IAgreement agreement)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _patient_Request = request;
            _jwtToken = token;
            _resetPasswordService = resetPasswordService;
            _emailService = emailService;
            _passwordHasher = passwordHasher;
            _agreement = agreement;
        }
        //Only ResetPassword is not taken into the three tier architecture otherwise everything from GuestController is in three tier architecture.
        public IActionResult Agree(int Requestid)
        {
            _agreement.AgreementAccepted(Requestid);
            return RedirectToAction("login_page", "Guest");
        }
        public IActionResult CancelAgreement(int Requestid, string Notes)
        {
            _agreement.AgreementRejected(Requestid, Notes);
            return RedirectToAction("login_page", "Guest");
        }
        public IActionResult ReviewAgreement(int ReqId)
        {
            var user = _context.Requestclients.FirstOrDefault(x => x.Requestid == ReqId);
            if (user != null)
            {
                ReviewAgreementViewModel reviewmodel = new()
                {
                    reqID = ReqId,
                    PatientName = user.Firstname + " " + user.Lastname
                };
                return View(reviewmodel);
            }
            return RedirectToAction("submit_request_page");
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult AccessDenied()
        {
            return View();
        }
        public IActionResult submit_request_page()
        {
            return View();
        }
        public IActionResult patient_submit_request_screen()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Business_Info()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Business_Info(BusinessModel bm)
        {
            if (ModelState.IsValid)
            {
                _patient_Request.BRequest(bm);
            }
            return View();
        }
        [HttpGet]
        public IActionResult Concierge_info()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Concierge_info(ConciergeModel cm)
        {
            if (ModelState.IsValid)
            {
                _patient_Request.CRequest(cm);
            }
            return View();
        }
        public IActionResult Friend_family()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Friend_family(FamilyFriendModel fmfr)
        {
            //path = _environment.WebRootPath;
            if (ModelState.IsValid)
            {
                _patient_Request.FRequest(fmfr);
            }
            return View(fmfr);
        }
        [HttpGet]
        public IActionResult create_patient_request()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult create_patient_request(PatientModel pm)
        {
            string path = _environment.WebRootPath;
            if (ModelState.IsValid)
            {
                _patient_Request.PRequest(pm, path);
                return RedirectToAction("create_patient_request", "Guest");
            }
            return View();
        }
        public IActionResult login_page()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult login_page(Aspnetuser demouser)
        {
            var password = _passwordHasher.GenerateSHA256(demouser.Passwordhash);
            Aspnetuser v = _context.Aspnetusers.FirstOrDefault(dt => dt.Username == demouser.Username && dt.Passwordhash == password);
            if (v != null)
            {
                HttpContext.Session.SetString("Email", v.Email);
                var token = _jwtToken.generateJwtToken(v.Email, "Patient");
                Response.Cookies.Append("jwt", token);

                TempData["success"] = "Logged In Successfully";
                return RedirectToAction("PatientDashboard", "Home");
            }
            return View();
        }
        public IActionResult forgot_password_page()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult forgot_password_page(ForgotPasswordViewModel fvm)
        {
            if (ModelState.IsValid)
            {
                var jwtToken = _resetPasswordService.GenerateJWTTokenForPassword(fvm);
                var resetLink = Url.Action("ResetPassword", "Guest", new { token = jwtToken }, Request.Scheme);
                _emailService.SendEmailForPasswordReset(fvm, resetLink);
                return RedirectToAction("login_page", "Guest");
            }
            return View();
        }
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            // 4. In the MVC controller, create an action method to handle the password reset request
            try
            {
                string emailid = _resetPasswordService.ValidateToken(token);
                ResetPasswordViewModel rpvm = new ResetPasswordViewModel()
                {
                    email = emailid
                };
                return View(rpvm);
            }
            catch (Exception ex)
            {
                return Content("Invalid token");
            }
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordViewModel rpvm)
        {
            Aspnetuser aspnetuser = _context.Aspnetusers.FirstOrDefault(u => u.Email == rpvm.email);
            if (rpvm.password == rpvm.confirmpassword)
            {
                aspnetuser.Passwordhash = _passwordHasher.GenerateSHA256(rpvm.password);
                aspnetuser.Modifieddate = DateTime.Now;
                _context.Aspnetusers.Update(aspnetuser);
                _context.SaveChanges();
                return View("login_page");
            }
            else
            {
                return View("Error");
            }
        }

    }

}
