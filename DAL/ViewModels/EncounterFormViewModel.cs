using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DAL.ViewModels
{
    public class EncounterFormViewModel
    {
        public string? FirstName {  get; set; }
        public string? LastName { get; set; }
        public string? Location {  get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public DateOnly? Date {  get; set; }
        public string? PhoneNo {  get; set; }
        public string? Email {  get; set; }
        public string? IllnessHistory {  get; set; }
        public string? MedicalHistory {  get; set; }
        public string? Medications {  get; set; }
        public string? Allergies {  get; set; }
        public string? Temperature {  get; set; }
        public string? HR {  get; set; }
        public string? RR {  get; set; }
        public string? BPLow {  get; set; }
        public string? BPHigh {  get; set; }
        public string? O2 {  get; set; }
        public string? Pain { get; set; }
        public string? heent { get; set; }
        public string? cv { get; set; }
        public string? chest { get; set; }
        public string? abd { get; set; }
        public string? extr { get; set; }
        public string? skin { get; set; }
        public string? neuro { get; set; }
        public string? other { get; set; }
        public string? diagnosis { get; set; }
        public string? treatmentPlan { get; set; }
        public string? MedicationsDispensed { get; set; }
        public string? procedures { get; set; }
        public string? followUps { get; set; }
        public int requestId { get; set; }
    }
}
