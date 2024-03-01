using DAL.ViewModels;
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
            AdminRequestsViewModel arvm = new AdminRequestsViewModel();
            AdminDashboardViewModel advm = new()
            {
                New = _context.Requests.Count(u => u.Status == 1),
                active = _context.Requests.Count(u => u.Status == 4 || u.Status == 5),
                pending = _context.Requests.Count(u => u.Status == 2),
                conclude = _context.Requests.Count(u => u.Status == 6),
                toclose = _context.Requests.Count(u => u.Status == 7 || u.Status == 3 || u.Status == 8),
                unpaid = _context.Requests.Count(u => u.Status == 9),
                //adminRequests = adminRequests,
                Username = arvm.Name
            };

            return View(advm);
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
                                     Name = rc.Firstname + " " + rc.Lastname,
                                     Requestor = r.Firstname,
                                     PhoneNo = rc.Phonenumber,
                                     Address = rc.Address,
                                     OtherPhoneNo = r.Phonenumber,
                                     requestType = r.Requesttypeid,
                                     status = r.Status,
                                     physicianName = "Dr.XYZ",
                                     servicedate = DateOnly.Parse("22-12-2022"),

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
