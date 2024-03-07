﻿using DAL.ViewModels;
using DAL.DataModels;
using Microsoft.AspNetCore.Mvc;
using DAL.DataContext;
namespace HalloDoc_Project.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _config;
        public AdminController(ApplicationDbContext context, IWebHostEnvironment environment, IConfiguration config)
        {
            _context = context;
            _environment = environment;
            _config = config;
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ViewCase(int requestid)
        {
            if(ModelState.IsValid)
            {
                Requestclient rc = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);
                ViewCaseViewModel vc = new()
                {
                    requestID = rc.Requestid,
                    patientemail = rc.Email,
                    patientfirstname = rc.Firstname,
                    patientlastname = rc.Lastname,
                    patientnotes = rc.Notes,
                    patientphone = rc.Phonenumber,
                    address = rc.Address,
                    rooms = "N/A"
                };
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
        public IActionResult AdminDBView()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid
                                 }).ToList();

            AdminRequestsViewModel arvm = new AdminRequestsViewModel();
            AdminDashboardViewModel advm = new()
            {
                adminRequests = adminRequests,
                Username = arvm.Name
            };

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
            List<Physician> physician=_context.Physicians.ToList();
            List<Region> regions=_context.Regions.ToList();
            List<Casetag> casetags=_context.Casetags.ToList();

            AdminRequestsViewModel arvm = new AdminRequestsViewModel();
            AdminDashboardViewModel advm = new()
            {
                physician=physician, 
                regions=regions,
                casetags=casetags,
                New = _context.Requests.Count(u => u.Status == 1),
                active = _context.Requests.Count(u => u.Status == 4 || u.Status == 5),
                pending = _context.Requests.Count(u => u.Status == 2),
                conclude = _context.Requests.Count(u => u.Status == 6),
                toclose = _context.Requests.Count(u => u.Status == 7 || u.Status == 3 || u.Status == 8),
                unpaid = _context.Requests.Count(u => u.Status == 9),
                Username = arvm.Name
            };
            return View(advm);
        }
        [HttpPost]
        public IActionResult AssignCase(int RequestId, string AssignPhysician, string AssignDescription)
        {
            var user = _context.Requests.FirstOrDefault(h => h.Requestid == RequestId);
            if(user!=null)
            {
                user.Status = 2;
                user.Modifieddate=DateTime.Now;
                user.Physicianid = int.Parse(AssignPhysician);

                _context.Update(user);
                _context.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = RequestId;
                requeststatuslog.Notes = AssignDescription;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 2;

                _context.Add(requeststatuslog);
                _context.SaveChanges();
            }
            return Ok();
        }
        [HttpPost]
        public ActionResult CancelCase(int requestid, string Reason, string Description)
        {
            var user = _context.Requests.FirstOrDefault(h => h.Requestid == requestid);
            if (user != null)
            {
                user.Status = 3;
                user.Casetag = Reason;

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = requestid;
                requeststatuslog.Notes = Description;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 3;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                _context.Update(user);
                _context.SaveChanges();

                return RedirectToAction("Admin_Dash");
            }
            return Ok();
        }
        [HttpPost]
        public IActionResult BlockCase(int requestid, string blocknotes)
        {
            var user= _context.Requests.FirstOrDefault(u=>u.Requestid== requestid);
            if (user != null)
            {
                user.Status = 11;

                _context.Update(user);
                _context.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = requestid;
                requeststatuslog.Notes = blocknotes ?? "--";
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 11;

                _context.Add(requeststatuslog);
                _context.SaveChanges();

                Blockrequest blockRequest = new Blockrequest();

                blockRequest.Requestid = requestid.ToString();
                blockRequest.Createddate = DateTime.Now;
                blockRequest.Email = user.Email;
                blockRequest.Phonenumber = user.Phonenumber;
                blockRequest.Reason = blocknotes ?? "--";
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult NewTable()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {

                                     requestid= r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requesteddate = r.Createddate,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status
                                 }).Where(x => x.status == 1).ToList();

            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };

            return PartialView("NewTable", model);
        }
        [HttpPost]
        public IActionResult ActiveTable()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022")
                                 }
                                ).Where(x => x.status == 4 || x.status == 5).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };

            return PartialView("ActiveTable", model);
        }

        [HttpPost]
        public IActionResult PendingTable()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     requestid=r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022")
                                 }
                                ).Where(x => x.status == 2).ToList();

            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                
                adminRequests = adminRequests,
            };
            return PartialView("PendingTable", model);
        }
        [HttpPost]
        public IActionResult ConcludeTable()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022")
                                 }
                                ).Where(x => x.status == 6).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                
                adminRequests = adminRequests,
            };

            return PartialView("ConcludeTable", model);
        }
        [HttpPost]
        public IActionResult ToCloseTable()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     requestid = r.Requestid,
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022")
                                 }
                                ).Where(x => x.status == 3 || x.status == 7 || x.status == 8).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };

            return PartialView("ToCloseTable", model);
        }
        public IActionResult DeleteFile(int fileid,int requestid)
        {

            var fileRequest=_context.Requestwisefiles.FirstOrDefault(x => x.Requestwisefileid == fileid);
            fileRequest.Isdeleted = true;

            _context.Update(fileRequest);
            _context.SaveChanges();

            return RedirectToAction("ViewUploads",new { requestid= requestid});
        }
        public void InsertRequestWiseFile(IFormFile document,String uniqueID)
        {
            string path = _environment.WebRootPath;
            string filePath = "Content/"+uniqueID+"$"+document.FileName;
            string fullPath = Path.Combine(path, filePath);

            using FileStream stream = new(fullPath, FileMode.Create);
            document.CopyTo(stream);
        }
        public IActionResult ViewUploads(int requestid)
        {
            
            var user = _context.Requests.FirstOrDefault(r=>r.Requestid==requestid);
            var requestFile = _context.Requestwisefiles.Where(r => r.Requestid == requestid).ToList();
            var requests=_context.Requests.FirstOrDefault(r=> r.Requestid==requestid);

            ViewUploadsViewModel uploads = new()
            {
                ConfirmationNo = requests.Confirmationnumber,
                Patientname=user.Firstname+" "+user.Lastname,
                RequestID=requestid,
                Requestwisefiles=requestFile
            };
            return View(uploads);
        }
        [HttpPost]
        public IActionResult ViewUploads(ViewUploadsViewModel uploads)
        {

            if (uploads.File != null)
            {
                var uniqueid = Guid.NewGuid().ToString();
                InsertRequestWiseFile(uploads.File,uniqueid);
                
                var filestring = Path.GetFileNameWithoutExtension(uploads.File.FileName);
                var extensionstring=Path.GetExtension(uploads.File.FileName);
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
            SendOrderViewModel model = new SendOrderViewModel()
            {
                requestid = requestid
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult SendOrders(int requestid,SendOrderViewModel sendOrder)
        {            
            Orderdetail Order = new()
            {
                Requestid = requestid,
                Faxnumber = sendOrder.FaxNo,
                Email = sendOrder.BusEmail,
                Businesscontact = sendOrder.BusContact,
                Prescription = sendOrder.prescirption,
                Noofrefill = sendOrder.RefillCount,
                Createddate = DateTime.Now,
                Vendorid=1
            };
            return View(sendOrder);
        }
        [HttpPost]
        public IActionResult UnpaidTable()
        {
            var adminRequests = (from r in _context.Requests
                                 join rc in _context.Requestclients on r.Requestid equals rc.Requestid
                                 select new AdminRequestsViewModel
                                 {
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022")
                                 }
                                ).Where(x => x.status == 9).ToList();
            AdminDashboardViewModel model = new AdminDashboardViewModel()
            {
                adminRequests = adminRequests,
            };
            return PartialView("UnpaidTable", model);
        }
    }
}
