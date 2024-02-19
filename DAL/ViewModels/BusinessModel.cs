using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class BusinessModel
    {
        public string BsFirstName {  get; set; }  
        public string BsLastName {  get; set; }
        public string BsPhoneNo {  get; set; }
        
        public string BsEmail {  get; set; }
        public string BusinessName {  get; set; }
        public int BsCaseNo {  get; set; }
        public string Symptoms {  get; set; }
        public string PtFirstName {  get; set; }
        public string PtLastName { get; set;}
        public DateOnly PtDOB {  get; set; } 
        public string PtEmail { get; set; }
        public string Street {  get; set; }
        public string city {  get; set; }
        public string zipcode {  get; set; }
        public string state {  get; set; }
        public string room {  get; set; }
        public string PtPhoneNo { get; set; }   
    }
}
