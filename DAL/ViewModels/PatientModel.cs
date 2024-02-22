using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DAL.ViewModels
{
    public class PatientModel
    {
        public string Symptoms { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Email { get; set; }
        public string PhoneNo { get; set; }
        public string Street  { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string RoomSuite { get; set; }
        public IFormFile File{ get; set; }
        public string Password { get; set; }
        public string ConfirmPass{ get; set; }
        
    }
}
