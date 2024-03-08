﻿using BAL.Interfaces;
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
        private readonly IPatient_Request patient_Request;
        private readonly IJwtToken _jwtToken;
        public GuestController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IPatient_Request request, IJwtToken token)
        {
            _context = context;
            _environment = environment;
            _config = config;
            patient_Request = request;
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
        public IActionResult Business_Info()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Business_Info(BusinessModel bm)
        {
            Business bus = new()
            {
                Name = bm.BusinessName,
                Phonenumber = bm.BsPhoneNo,
                Createddate = DateTime.Now
            };
            _context.Businesses.Add(bus);
            _context.SaveChanges();

            Request req = new()
            {
                Requesttypeid = 1,
                Firstname = bm.BsFirstName,
                Lastname = bm.BsLastName,
                Phonenumber = bm.BsPhoneNo,
                Email = bm.BsEmail,
                Status = 1,
                Createddate = DateTime.Now,
            };
            _context.Requests.Add(req);
            _context.SaveChanges();
            Requestbusiness ReqBus = new()
            {
                Requestid = req.Requestid,
                Businessid = bus.Id,

            };

            _context.Requestbusinesses.Add(ReqBus);
            _context.SaveChanges();

            Requestclient rc = new()
            {
                Requestid = req.Requestid,
                Firstname = bm.PtFirstName,
                Lastname = bm.BsLastName,
                Phonenumber = bm.BsPhoneNo,
                Street = bm.Street,
                City = bm.city,
                State = bm.state,
                Zipcode = bm.zipcode
            };
            _context.Requestclients.Add(rc);
            _context.SaveChanges();
            return RedirectToAction("submit_request_page", "Guest");
        }
        public IActionResult Concierge_info()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Concierge_info(ConciergeModel cm)
        {
            Concierge c = new()
            {
                Conciergename = cm.ConFirstName + cm.ConLastName,
                Street = cm.ConStreet,
                City = cm.ConCity,
                State = cm.ConState,
                Zipcode = cm.ConZipCode,
                Createddate = DateTime.Now,

            };
            _context.Concierges.Add(c);
            _context.SaveChanges();
            Request req = new()
            {
                Requesttypeid = 4,
                Firstname = cm.ConFirstName,
                Lastname = cm.ConLastName,
                Phonenumber = cm.ConPhoneNo,
                Email = cm.ConEmail,
                Status = 1,
                Createddate = DateTime.Now,

            };
            _context.Requests.Add(req);
            _context.SaveChanges();
            Requestconcierge rc = new()
            {
                Requestid = req.Requestid,
                Conciergeid = c.Conciergeid
            };
            _context.Requestconcierges.Add(rc);
            _context.SaveChanges();
            Requestclient rcl = new()
            {
                Requestid = req.Requestid,
                Firstname = cm.PtFirstName,
                Lastname = cm.PtLastName,
                Phonenumber = cm.PtPhoneNo,
                Email = cm.PtEmail,
                Street = cm.ConStreet,
                City = cm.ConCity,
                State = cm.ConState,
                Zipcode = cm.ConZipCode
            };
            _context.Requestclients.Add(rcl);
            _context.SaveChanges();
            return RedirectToAction("submit_request_page", "Guest");
        }
        public IActionResult Friend_family()
        {
            return View();
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Friend_family(FamilyFriends fmfr)
        {
            Request r = new()
            {
                Requesttypeid = 3,
                Firstname = fmfr.firstName,
                Lastname = fmfr.lastName,
                Phonenumber = fmfr.phone,
                Email = fmfr.email,
                Status = 1,
                Createddate = DateTime.Now
            };
            _context.Requests.Add(r);
            _context.SaveChanges();
            Requestclient rcl = new()
            {
                Requestid = r.Requestid,
                Firstname = fmfr.PatientModel.FirstName,
                Lastname = fmfr.PatientModel.LastName,
                Phonenumber = fmfr.PatientModel.PhoneNo,
                Email = fmfr.PatientModel.Email,
                Location = fmfr.PatientModel.City + fmfr.PatientModel.State,
                City = fmfr.PatientModel.City,
                State = fmfr.PatientModel.State,
                Zipcode = fmfr.PatientModel.ZipCode

            };

            _context.Requestclients.Add(rcl);
            _context.SaveChanges();

            return RedirectToAction("submit_request_page", "Guest");
        }
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
                if (pm.Password != null)
                {
                    //var newvm=new PatientModel();
                    Aspnetuser user = new Aspnetuser();
                    Guid id = Guid.NewGuid();

                    user.Id = id.ToString();
                    user.Email = pm.Email;
                    user.Passwordhash = GenerateSHA256(pm.Password);
                    user.Phonenumber = pm.PhoneNo;
                    user.Username = pm.FirstName;
                    user.Createddate = DateTime.Now;
                    _context.Aspnetusers.Add(user);
                    _context.SaveChanges();

                    //user.Modifieddate = DateTime.Now;

                    User user_obj = new User();
                    user_obj.Aspnetuserid = user.Id;
                    user_obj.Firstname = pm.FirstName;
                    user_obj.Lastname = pm.LastName;
                    user_obj.Email = pm.Email;
                    user_obj.Mobile = pm.PhoneNo;
                    user_obj.Street = pm.Street;
                    user_obj.City = pm.City;
                    user_obj.State = pm.State;
                    user_obj.Zipcode = pm.ZipCode;
                    user_obj.Createddate = DateTime.Now;
                    user_obj.Createdby = id.ToString();
                    //user_obj.Modifiedby = null;
                    _context.Users.Add(user_obj);
                    _context.SaveChanges();



                    Request request = new Request();
                    //change the fname, lname , and contact detials acc to the requestor
                    request.Requesttypeid = 2;
                    request.Userid = user_obj.Userid;
                    request.Firstname = pm.FirstName;
                    request.Lastname = pm.LastName;
                    request.Phonenumber = pm.PhoneNo;
                    request.Email = pm.Email;
                    request.Createddate = DateTime.Now;
                    request.Patientaccountid = id.ToString();
                    request.Status = 1;
                    request.Createduserid = user_obj.Userid;
                    _context.Requests.Add(request);
                    _context.SaveChanges();

                    Requestclient rc = new Requestclient();
                    rc.Requestid = request.Requestid;
                    rc.Firstname = pm.FirstName;
                    rc.Lastname = pm.LastName;
                    rc.Phonenumber = pm.PhoneNo;
                    rc.Location = pm.City + pm.State;
                    rc.Email = pm.Email;
                    rc.Address = pm.RoomSuite + ", " + pm.Street + ", " + pm.City + ", " + pm.State + ", " + pm.ZipCode;
                    rc.Street = pm.Street;
                    rc.City = pm.City;
                    rc.State = pm.State;
                    rc.Zipcode = pm.ZipCode;
                    rc.Notes = pm.Symptoms;

                    _context.Requestclients.Add(rc);
                    _context.SaveChanges();

                    if (pm.File != null)
                    {

                        InsertRequestWiseFile(pm.File);
                        Requestwisefile rwf = new()
                        {
                            Requestid = request.Requestid,
                            Filename = pm.File.FileName,
                            Createddate = DateTime.Now,
                        };
                        _context.Requestwisefiles.Add(rwf);
                        _context.SaveChanges();
                    }

                    return RedirectToAction("create_patient_request", "Guest");
                }
                else
                {

                    User user_obj = _context.Users.FirstOrDefault(u => u.Email == pm.Email);

                    Request request = new Request();
                    //change the fname, lname , and contact detials acc to the requestor
                    request.Requesttypeid = 2;
                    request.Userid = user_obj.Userid;
                    request.Firstname = pm.FirstName;
                    request.Lastname = pm.LastName;
                    request.Phonenumber = pm.PhoneNo;
                    request.Email = pm.Email;
                    request.Createddate = DateTime.Now;
                    request.Patientaccountid = user_obj.Aspnetuserid;
                    request.Status = 1;
                    request.Createduserid = user_obj.Userid;
                    _context.Requests.Add(request);
                    _context.SaveChanges();

                    Requestclient rc = new Requestclient();
                    rc.Requestid = request.Requestid;
                    rc.Firstname = pm.FirstName;
                    rc.Lastname = pm.LastName;
                    rc.Phonenumber = pm.PhoneNo;
                    rc.Location = pm.City + pm.State;
                    rc.Email = pm.Email;
                    rc.Address = pm.RoomSuite + ", " + pm.Street + ", " + pm.City + ", " + pm.State + ", " + pm.ZipCode;
                    rc.Street = pm.Street;
                    rc.City = pm.City;
                    rc.State = pm.State;
                    rc.Zipcode = pm.ZipCode;
                    rc.Notes = pm.Symptoms;

                    _context.Requestclients.Add(rc);
                    _context.SaveChanges();
                    if (pm.File != null)
                    {

                        InsertRequestWiseFile(pm.File);
                        Requestwisefile rwf = new()
                        {
                            Requestid = request.Requestid,
                            Filename = pm.File.FileName,
                            Createddate = DateTime.Now,
                        };
                        _context.Requestwisefiles.Add(rwf);
                        _context.SaveChanges();
                    }

                    return RedirectToAction("create_patient_request", "Guest");
                }

            }
            return View();
        }
        public void InsertRequestWiseFile(IFormFile document)
        {
            string path = _environment.WebRootPath;
            string filePath = "Content/" + document.FileName;
            string fullPath = Path.Combine(path, filePath);

            using FileStream stream = new(fullPath, FileMode.Create);
            document.CopyTo(stream);
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

                return RedirectToAction("PatientDashboard","Home");
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
