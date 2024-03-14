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

        public GuestController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IRequestRepo request, IJwtToken token)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _patient_Request = request;
            _jwtToken = token;
        }
        public static string GenerateSHA256(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            using (var hashEngine = SHA256.Create())
            {
                var hashedBytes = hashEngine.ComputeHash(bytes, 0, bytes.Length);
                var sb = new StringBuilder();
                foreach (var b in hashedBytes)
                {
                    var hex = b.ToString("x2");
                    sb.Append(hex);
                }
                return sb.ToString();
            }
        }
        public IActionResult Agree(int Requestid)
        {
            Request req = _context.Requests.FirstOrDefault(x => x.Requestid == Requestid);

            req.Status = 4;
            req.Modifieddate = DateTime.Now;

            _context.Update(req);
            _context.SaveChanges();

            Requeststatuslog requeststatuslog = new Requeststatuslog();

            requeststatuslog.Requestid = Requestid;
            requeststatuslog.Notes = "Agreement Accepted";
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Status = 4;

            _context.Add(requeststatuslog);
            _context.SaveChanges();

            return RedirectToAction("login_page", "Guest");
        }
        public IActionResult CancelAgreement(int Requestid, string Notes)
        {
            Request req = _context.Requests.FirstOrDefault(x => x.Requestid == Requestid);

            req.Status = 7;
            req.Modifieddate = DateTime.Now;

            _context.Update(req);
            _context.SaveChanges();

            Requeststatuslog requeststatuslog = new Requeststatuslog();

            requeststatuslog.Requestid = Requestid;
            requeststatuslog.Notes = Notes;
            requeststatuslog.Createddate = DateTime.Now;
            requeststatuslog.Status = 7;

            _context.Add(requeststatuslog);
            _context.SaveChanges();

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
            if(ModelState.IsValid)
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

            var password = GenerateSHA256(demouser.Passwordhash);
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
        [HttpGet]
        public ActionResult ResetPassword(string token)
        {
            // 4. In the MVC controller, create an action method to handle the password reset request
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                // 5. Verify the JWT token and allow the user to reset the password if the token is valid
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateAudience = false,
                    ValidIssuer = _config["Jwt:Issuer"],
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var email = jwtToken.Claims.First(x => x.Type == "email").Value;

                ResetPasswordViewModel rpvm = new ResetPasswordViewModel()
                {
                    email = email
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
                aspnetuser.Passwordhash = GenerateSHA256(rpvm.password);
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
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult forgot_password_page(ForgotPasswordViewModel fvm)
        {
            if (ModelState.IsValid)
            {

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["Jwt:Key"]);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim("email", fvm.email) }),
                    Expires = DateTime.UtcNow.AddHours(24),
                    Issuer = _config["Jwt:Issuer"],
                    //Audience = _audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var jwtToken = tokenHandler.WriteToken(token);

                var resetLink = Url.Action("ResetPassword", "Home", new { token = jwtToken }, Request.Scheme);

                //----------------------------------

                var smtpClient = new SmtpClient("smtp.office365.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("tatva.dotnet.rahulshah@outlook.com", "@08RahulTatvA"),
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false
                };
                //smtpClient.Send("tatva.dotnet.rahulshah@outlook.com", "rahul0810shah@gmail.com", "This is a trial email for smtpClient.", "this is token ->" + resetLink);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress("tatva.dotnet.rahulshah@outlook.com"),
                    Subject = "Subject",
                    Body = "<h1>Hello , Good morning!!</h1><a href=\"" + resetLink + "\" >Reset your password</a>",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(fvm.email);
                smtpClient.Send(mailMessage);
                return RedirectToAction("login_page", "Guest");
            }
            return View();
        }
    }

}
