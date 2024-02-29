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
        [Required]
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [Required]
        public string DateOfBirth { get; set; }
        [Required]
        [RegularExpression("^([\\w\\.\\-]+)@([\\w\\-]+)((\\.(\\w){2,3})+)$", ErrorMessage = "Enter Valid Email")]
        public string Email { get; set; }
        [Required] 
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",ErrorMessage = "Entered phone format is not valid.")]
        public string PhoneNo { get; set; }
        [Required(ErrorMessage = "Street cannot be empty")]
        public string Street  { get; set; }
        [Required(ErrorMessage = "City cannot be empty")]
        public string City { get; set; }
        [Required(ErrorMessage = "State cannot be empty")]
        public string State { get; set; }
        [Required(ErrorMessage = "Zipcode cannot be empty")]
        public string ZipCode { get; set; }
        
        public string RoomSuite { get; set; }

        public IFormFile File{ get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "Password and Confirm Password should be same.")]
        public string ConfirmPass{ get; set; }
        
    }
}
