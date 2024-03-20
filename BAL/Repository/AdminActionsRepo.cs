using BAL.Interfaces;
using DAL.DataContext;
using DAL.DataModels;
using DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BAL.Repository
{
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
    public class AdminActionsRepo : IAdminActions
    {
        private readonly ApplicationDbContext _context;

        public AdminActionsRepo(ApplicationDbContext context)
        {
            _context = context;
        }
        public ViewCaseViewModel ViewCaseAction(int requestid)
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
            return vc;
        }
        public void AssignCaseAction(int RequestId, string AssignPhysician, string AssignDescription)
        {
            var user = _context.Requests.FirstOrDefault(h => h.Requestid == RequestId);
            if (user != null)
            {
                user.Status = 2;
                user.Modifieddate = DateTime.Now;
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
        }
        public void CancelCaseAction(int requestid, string Reason, string Description)
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
            }
        }
        public void BlockCaseAction(int requestid, string blocknotes)
        {
            var user = _context.Requests.FirstOrDefault(u => u.Requestid == requestid);
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
        }
        public void TransferCase(int RequestId, string TransferPhysician, string TransferDescription, int adminid)
        {
            var req = _context.Requests.FirstOrDefault(h => h.Requestid == RequestId);
            if (req != null)
            {

                req.Status = 2;
                req.Modifieddate = DateTime.Now;
                req.Physicianid = int.Parse(TransferPhysician);
                
                _context.Update(req);
                _context.SaveChanges();

                Requeststatuslog requeststatuslog = new Requeststatuslog();

                requeststatuslog.Requestid = RequestId;
                requeststatuslog.Notes = TransferDescription;
                requeststatuslog.Createddate = DateTime.Now;
                requeststatuslog.Status = 2;
                requeststatuslog.Transtophysicianid = int.Parse(TransferPhysician);
                requeststatuslog.Adminid = adminid;
                
                _context.Add(requeststatuslog);
                _context.SaveChanges();
            }
        }
        public bool ClearCaseModal(int requestid)
        {
            //Admin admin = _context.Admins.GetFirstOrDefault(a => a.Email == AdminEmail);
            try
            {
                Request req = _context.Requests.FirstOrDefault(req => req.Requestid == requestid);
                req.Modifieddate = DateTime.Now;

                Requeststatuslog reqStatusLog = new Requeststatuslog()
                {
                    Requestid = requestid,
                    Status = (short)RequestStatus.Clear,
                    //Adminid = adminid,
                    Notes = "Admin cleared this request",
                    Createddate = DateTime.Now,
                };

                req.Status = (short)RequestStatus.Clear;
                req.Modifieddate = DateTime.Now;


                _context.Requests.Update(req);
                _context.SaveChanges();

                _context.Requeststatuslogs.Add(reqStatusLog);
                _context.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        void IAdminActions.SendOrderAction(int requestid, SendOrderViewModel sendOrder)
        {
            Orderdetail Order = new()
            {
                Requestid = requestid,
                Faxnumber = sendOrder.FaxNo,
                Email = sendOrder.BusEmail,
                Businesscontact = sendOrder.BusContact,
                Prescription = sendOrder.prescription,
                Noofrefill = sendOrder.RefillCount,
                Createddate = DateTime.Now,
                Vendorid = 1
            };
            _context.Add(Order);
            _context.SaveChanges();
        }

        public CloseCaseViewModel CloseCaseGet(int requestid)
        {
            var files = _context.Requestwisefiles.Where(x => x.Requestid == requestid).ToList();
            var user = _context.Requestclients.FirstOrDefault(x => x.Requestid == requestid);
            //string dob = user.Intyear + "-" + user.Strmonth.ToString() + "-" + user.Intdate;
            CloseCaseViewModel model = new()
            {
                firstname = user.Firstname,
                lastname = user.Lastname,
                //dateofbirth = DateTime.Parse(dob),
                phoneno = user.Phonenumber,
                email = user.Email,
                RequestwisefileList = files,
                requestid = requestid,
            };
            return model;
        }

        public void CloseCasePost(CloseCaseViewModel model, int requestid)
        {
            var user = _context.Requestclients.FirstOrDefault(r => r.Requestid == requestid);

            user.Firstname = model.firstname;
            user.Lastname = model.lastname;
            user.Phonenumber = model.phoneno;
            user.Email = model.email;

            _context.Update(user);
            _context.SaveChanges();
        }
    }
}
