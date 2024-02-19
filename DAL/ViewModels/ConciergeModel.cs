using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.ViewModels
{
    public class ConciergeModel
    {
        public string ConFirstName{ get; set; }
        public string ConLastName { get; set;}
        public string ConPhoneNo {  get; set; }
        public string ConEmail { get; set; }
        public string ConPropertyName {  get; set; }
        public string ConStreet {  get; set; }
        public string ConCity {  get; set; }
        public string ConState {  get; set; }
        public string ConZipCode { get; set; }
        public string PtSymptoms {  get; set; }
        public string PtFirstName {  get; set; }
        public string PtLastName { get; set;}
        public string PtDOB {  get; set; }
        public string PtEmail {  get; set; }
        public string PtPhoneNo { get;set; }
        public string PtRoomSuite {  get; set; }
    }
}
