using DAL.ViewModels;
using DAL.DataModels;
using Microsoft.AspNetCore.Mvc;
using DAL.DataContext;
using System.Net.Mail;
using System.Net;
using System.Reflection.Metadata;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Cryptography.X509Certificates;
using BAL.Interfaces;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
namespace HalloDoc_Project.Controllers
{
    [CustomAuthorize("Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;
        private readonly IAdminActions _adminActions;
        private readonly IAdminTables _adminTables;
        private readonly IFileOperations _fileOperations;
        private readonly IEncounterForm _encounterForm;
        private readonly IAdmin _admin;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config, IEmailService emailService, IAdminTables adminTables, IAdminActions adminActions, IFileOperations fileOperations, IEncounterForm encounterForm, IAdmin admin)
        {
            _context = context;
            _environment = environment;
            _config = config;
            _emailService = emailService;
            _adminActions = adminActions;
            _adminTables = adminTables;
            _fileOperations = fileOperations;
            _encounterForm = encounterForm;
            _admin = admin;
        }
        public IActionResult Index()
        {
            return View();
        }

        public enum RequestStatus
        {
            Unassigned = 1,
            Accepted = 2,
            Cancelled = 3,
            MDEnRoute = 4,
            MDOnSite = 5,
            Conclude = 6,
            CancelledByPatient = 7,
            Closed = 8,
            Unpaid = 9,
            Clear = 10,
            Block = 11,
        }

        public enum DashboardStatus
        {
            New = 1,
            Pending = 2,
            Active = 3,
            Conclude = 4,
            ToClose = 5,
            Unpaid = 6,
        }

        public enum RequestType
        {
            Business = 1,
            Patient = 2,
            Family = 3,
            Concierge = 4
        }

        public enum AllowRole
        {
            Admin = 1,
            Patient = 2,
            Physician = 3
        }
        public IActionResult Access()
        {
            return View();
        }
        //Delete, DeleteAll, ViewUploads, SendOrders(Get) methods are not converted to three tier.
        public IActionResult ViewCase(int requestid)
        {
            if (ModelState.IsValid)
            {
                ViewCaseViewModel vc = _adminActions.ViewCaseAction(requestid);
                return View(vc);
            }
            return View();
        }
        public ActionResult AssignCase()
        {
            return Ok();
        }
        public ActionResult BlockCase(String blockreason)
        {

            return Ok();
        }
        public IActionResult ViewNotes(int requestid)
        {
            ViewCaseViewModel vn = new ViewCaseViewModel();
            return View();
        }
        public ActionResult TransferNotes()
        {
            return Ok();
        }
        public IActionResult AdminDBView()
        {
            AdminDashboardViewModel advm = _adminTables.AdminDashboardView();
            return View(advm);
        }
        public static string GetDOB(Requestclient reqcli)
        {
            string dob = reqcli.Intyear + "-" + reqcli.Strmonth + "-" + reqcli.Intdate;
            if (reqcli.Intyear == null || reqcli.Strmonth == null || reqcli.Intdate == null)
            {
                return " ";
            }

            string dobdate = DateTime.Parse(dob).ToString("MMM dd, yyyy");

            return dobdate;
        }
        public IActionResult Partners()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult ProviderLocation()
        {
            return View();
        }
        public IActionResult Providers()
        {
            return View();
        }
        public IActionResult Records()
        {
            return View();
        }
        public IActionResult AdminDashboard()
        {
            var email = HttpContext.Session.GetString("Email");
            AdminDashboardViewModel advm = _adminTables.AdminDashboard(email);
            return View(advm);
        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, string AssignPhysician, string AssignDescription)
        {
            _adminActions.AssignCaseAction(RequestId, AssignPhysician, AssignDescription);
            return Ok();
        }
        [HttpPost]
        public ActionResult CancelCase(int requestid, string Reason, string Description)
        {
            _adminActions.CancelCaseAction(requestid, Reason, Description);
            return Ok();
        }
        [HttpPost]
        public IActionResult BlockCase(int requestid, string blocknotes)
        {
            _adminActions.BlockCaseAction(requestid, blocknotes);
            return Ok();
        }
        [HttpPost]
        public IActionResult TransferCase(int RequestId, string TransferPhysician, string TransferDescription)
        {
            _adminActions.TransferCase(RequestId, TransferPhysician, TransferDescription);
            return Ok();
        }
        [HttpPost]
        public bool ClearCaseModal(int requestid)
        {
            string AdminEmail = HttpContext.Session.GetString("Email");
            //Admin admin = _context.Admins.GetFirstOrDefault(a => a.Email == AdminEmail);
            return _adminActions.ClearCaseModal(requestid);
        }
        [HttpPost]
        public IActionResult NewTable()
        {
            AdminDashboardViewModel model = _adminTables.GetNewTable();
            return PartialView("NewTable", model);
        }
        [HttpPost]
        public IActionResult ActiveTable()
        {
            AdminDashboardViewModel model = _adminTables.GetActiveTable();
            return PartialView("ActiveTable", model);
        }
        [HttpPost]
        public IActionResult PendingTable()
        {
            AdminDashboardViewModel model = _adminTables.GetPendingTable();
            return PartialView("PendingTable", model);
        }
        [HttpPost]
        public IActionResult ConcludeTable()
        {
            AdminDashboardViewModel model = _adminTables.GetConcludeTable();
            return PartialView("ConcludeTable", model);
        }
        [HttpPost]
        public IActionResult ToCloseTable()
        {
            AdminDashboardViewModel model = _adminTables.GetToCloseTable();
            return PartialView("ToCloseTable", model);
        }
        [HttpPost]
        public IActionResult UnpaidTable()
        {
            AdminDashboardViewModel model = _adminTables.GetUnpaidTable();
            return PartialView("UnpaidTable", model);
        }
        public IActionResult EncounterForm(int requestId, EncounterFormViewModel EncModel)
        {
            EncModel = _encounterForm.EncounterFormGet(requestId);
            return View(EncModel);
        }
        [HttpPost]
        public IActionResult EncounterForm(EncounterFormViewModel model)
        {
            _encounterForm.EncounterFormPost(model.requestId, model);
            return EncounterForm(model.requestId, model);
        }
        public IActionResult AdminProfile()
        {
            var email = HttpContext.Session.GetString("Email");
            AdminProfileViewModel model = new AdminProfileViewModel();
            if (email != null)
            {
                model = _admin.AdminProfileGet(email);
            }
            return View("AdminProfile", model);
        }
        [HttpPost]
        public IActionResult AdminInfoPost(AdminProfileViewModel apvm)
        {
            _admin.AdminInfoPost(apvm);
            return AdminProfile();
        }
        [HttpPost]
        public IActionResult BillingInfoPost(AdminProfileViewModel apvm)
        {
            _admin.BillingInfoPost(apvm);
            return AdminProfile();
        }
        [HttpPost]
        public IActionResult PasswordPost(AdminProfileViewModel apvm)
        {
            var email = HttpContext.Session.GetString("Email");
            _admin.PasswordPost(apvm,email);
            return AdminProfile();
        }
        public IActionResult DeleteFile(int fileid, int requestid)
        {
            var fileRequest = _context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
            fileRequest.Isdeleted = true;

            _context.Update(fileRequest);
            _context.SaveChanges();

            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }
        public IActionResult DeleteAllFiles(int requestid)
        {
            var request = _context.Requestwisefiles.Where(r => r.Requestid == requestid && r.Isdeleted != true).ToList();
            for (int i = 0; i < request.Count; i++)
            {
                request[i].Isdeleted = true;
                _context.Update(request[i]);
            }
            _context.SaveChanges();
            return RedirectToAction("ViewUploads", new { requestid = requestid });
        }
        public IActionResult ViewUploads(int requestid)
        {

            var user = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);
            var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
            var requests = _context.Requests.FirstOrDefault(r => r.Requestid == requestid);

            ViewUploadsViewModel uploads = new()
            {
                ConfirmationNo = requests.Confirmationnumber,
                Patientname = user.Firstname + " " + user.Lastname,
                RequestID = requestid,
                Requestwisefiles = requestFile
            };
            return View(uploads);
        }
        public IActionResult SetPath(int requestid)
        {
            var path = _environment.WebRootPath;
            return SendMail(requestid, path);
        }
        public IActionResult SendMail(int requestid, string path)
        {
            _emailService.SendEmailWithAttachments(requestid, path);
            return RedirectToAction("ViewUploads", "Admin", new { requestid = requestid });
        }
        [HttpPost]
        public IActionResult SendAgreement(int RequestId, string PhoneNo, string email)
        {
            if (ModelState.IsValid)
            {
                var AgreementLink = Url.Action("ReviewAgreement", "Guest", new { ReqId = RequestId }, Request.Scheme);
                //----------------------------------
                _emailService.SendAgreementLink(RequestId, AgreementLink, email);
                return RedirectToAction("AdminDashboard", "Guest");
            }
            return View();
        }
        public IActionResult BusinessData(int BusinessId)
        {
            var result = _context.Healthprofessionals.FirstOrDefault(x => x.Vendorid == BusinessId);
            return Json(result);
        }
        [HttpPost]
        public IActionResult ViewUploads(ViewUploadsViewModel uploads)
        {
            if (uploads.File != null)
            {
                var uniqueid = Guid.NewGuid().ToString();
                var path = _environment.WebRootPath;
                _fileOperations.insertfilesunique(uploads.File, uniqueid, path);

                var filestring = Path.GetFileNameWithoutExtension(uploads.File.FileName);
                var extensionstring = Path.GetExtension(uploads.File.FileName);
                Requestwisefile requestwisefile = new()
                {
                    Filename = uniqueid + "$" + uploads.File.FileName,
                    Requestid = uploads.RequestID,
                    Createddate = DateTime.Now
                };
                _context.Update(requestwisefile);
                _context.SaveChanges();
            }
            return RedirectToAction("ViewUploads", new { requestid = uploads.RequestID });
        }
        public IActionResult SendOrders(int requestid)
        {
            List<Healthprofessional> healthprofessionals = _context.Healthprofessionals.ToList();
            List<Healthprofessionaltype> healthprofessionaltypes = _context.Healthprofessionaltypes.ToList();
            SendOrderViewModel model = new SendOrderViewModel()
            {
                requestid = requestid,
                healthprofessionals = healthprofessionals,
                healthprofessionaltype = healthprofessionaltypes
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult SendOrders(int requestid, SendOrderViewModel sendOrder)
        {
            _adminActions.SendOrderAction(requestid, sendOrder);
            return SendOrders(requestid);
        }
        public List<Healthprofessional> filterVenByPro(string ProfessionId)
        {
            var result = _context.Healthprofessionals.Where(u => u.Profession == int.Parse(ProfessionId)).ToList();
            return result;
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("login_page", "Guest");
        }

    }
}