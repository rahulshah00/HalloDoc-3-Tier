﻿using DAL.DataModels;
using HalloDoc_Project.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DAL.DataContext;
using DAL.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System.Web;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication;
using System.Net.Cache;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System.Net.Mail;
using System.Net;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BAL.Interfaces;
namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Patient")]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IRequestRepo _patient_Request;

        public HomeController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IRequestRepo request)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _patient_Request = request;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult patient_submit_request_screen()
        {
            return View();
        }

        public IActionResult SelectedDownload()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [HttpPost]
        public JsonResult CheckEmail(string email)
        {
            bool emailExists = _context.Users.Any(u => u.Email == email);
            return Json(new { exists = emailExists });

        }


        public IActionResult PatientProfile()
        {

            var email = HttpContext.Session.GetString("Email");
            User v = _context.Users.FirstOrDefault(dt => dt.Email == email);
            PatientProfileViewModel ppm = new()
            {
                email = v.Email,
                FirstName = v.Firstname,
                LastName = v.Lastname,
                PhoneNo = v.Mobile,
                street = v.Street,
                state = v.State,
                city = v.City,
                zipcode = v.Zipcode,
                userid = v.Userid
            };
            return View(ppm);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult editprofile(PatientProfileViewModel ppm)
        {
            //string phoneNumber = "+" + /*pm.CountryCode*/ + '-' + pm.Phone;
            HttpContext.Session.GetString("Email");
            User dbUser = _context.Users.FirstOrDefault(u => u.Email == ppm.email);
            dbUser.Firstname = ppm.FirstName;
            dbUser.Lastname = ppm.LastName;
            dbUser.Intdate = ppm.Date.Day;
            dbUser.Strmonth = ppm.Date.Month.ToString();
            dbUser.Intyear = ppm.Date.Year;
            dbUser.Mobile = ppm.PhoneNo;
            dbUser.Street = ppm.street;
            dbUser.City = ppm.city;
            dbUser.State = ppm.state;
            dbUser.Zipcode = ppm.zipcode;

            _context.Update(dbUser);
            _context.SaveChanges();
            TempData["loginUserId"] = dbUser.Userid;
            return RedirectToAction("PatientProfile");
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

                return RedirectToAction("PatientDashboard");
            }
            return View();
        }

        public IActionResult patient_site()
        {
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

        public IActionResult ViewDocuments(int requestid)
        {
            string? email = HttpContext.Session.GetString("Email");
            User user = _context.Users.FirstOrDefault(u => u.Email == email);
            Request request = _context.Requests.FirstOrDefault(v => v.Requestid == requestid);
            List<Requestwisefile> files = _context.Requestwisefiles.Where(reqFiles => reqFiles.Requestid == requestid).ToList();

            ViewDocumentsViewModel vm = new ViewDocumentsViewModel();

            vm.ConfirmationNo = request.Confirmationnumber;
            vm.RequestID = requestid;
            vm.Username = user.Firstname + " " + user.Lastname;
            vm.Requestwisefiles = files;

            return View(vm);
        }
        [HttpPost]
        public IActionResult ViewDocuments(ViewDocumentsViewModel vm)
        {
            if (vm.File != null)
            {
                InsertRequestWiseFile(vm.File);
                Requestwisefile rwf = new()
                {
                    Requestid = vm.RequestID,
                    Filename = vm.File.FileName,
                    Createddate = DateTime.Now,
                };
                _context.Requestwisefiles.Add(rwf);
                _context.SaveChanges();
            }
            return ViewDocuments(vm.RequestID);
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

        public IActionResult PatientDashboard()
        {
            var email = HttpContext.Session.GetString("Email");
            User user = _context.Users.FirstOrDefault(u => u.Email == email);
            PatientDashboardViewModel pd = new PatientDashboardViewModel();
            pd.Username = user.Firstname + " " + user.Lastname == null ? "" : user.Lastname;
            pd.Requests = _context.Requests.Where(req => req.Userid == user.Userid).ToList();
            List<int> documentCount = new List<int>();
            for (int i = 0; i < pd.Requests.Count; i++)
            {
                int count = _context.Requestwisefiles.Count(rf => rf.Requestid == pd.Requests[i].Requestid);
                documentCount.Add(count);
            }
            pd.DocumentCount = documentCount;

            return View(pd);
        }

        public IActionResult Friend_family()
        {
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Friend_family(FamilyFriendModel fmfr)
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

        public async Task<IActionResult> DownloadAllFiles(int requestId)
        {
            try
            {
                // Fetch all document details for the given request:
                var documentDetails = _context.Requestwisefiles.Where(m => m.Requestid == requestId).ToList();

                if (documentDetails == null || documentDetails.Count == 0)
                {
                    return NotFound("No documents found for download");
                }

                // Create a unique zip file name
                var zipFileName = $"Documents_{DateTime.Now:yyyyMMddHHmmss}.zip";
                var zipFilePath = Path.Combine(_environment.WebRootPath, "DownloadableZips", zipFileName);

                // Create the directory if it doesn't exist
                var zipDirectory = Path.GetDirectoryName(zipFilePath);
                if (!Directory.Exists(zipDirectory))
                {
                    Directory.CreateDirectory(zipDirectory);
                }

                // Create a new zip archive
                using (var zipArchive = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
                {
                    // Add each document to the zip archive
                    foreach (var document in documentDetails)
                    {
                        var documentPath = Path.Combine(_environment.WebRootPath, "Content", document.Filename);
                        zipArchive.CreateEntryFromFile(documentPath, document.Filename);
                    }
                }

                // Return the zip file for download
                var zipFileBytes = await System.IO.File.ReadAllBytesAsync(zipFilePath);
                return File(zipFileBytes, "application/zip", zipFileName);
            }
            catch
            {
                return BadRequest("Error downloading files");
            }
        }

        public IActionResult logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login_page", "Guest");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}