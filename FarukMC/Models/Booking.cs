using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using FarukMC.Areas.Identity.Data;
using FarukMC.Data;
using Microsoft.AspNetCore.Identity;

namespace FarukMC.Models
{
    public class Booking : BaseEntity
    {
        [Key]
        public int ID { get; set; }        
        [DisplayFormat(ApplyFormatInEditMode =true, DataFormatString = "{0:dd/MM/yyyy-HH:mm}")]
        [DisplayName("Surgery Starting Time")]        
        public DateTime SurgeryStartingTime { get; set; }
        [DisplayFormat(ApplyFormatInEditMode =true,DataFormatString = "{0:dd/MM/yyyy-HH:mm}")]
        [DisplayName("Surgery Ending Time")]
        public DateTime SurgeryEndingTime { get; set; }
        [DisplayName("Room No.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a room")]
        public int SurgeryRoomId { get; set; }
        [DisplayName("Surgery Room")]
        public SurgeryRoom SurgeryRoom { get; set; }
        [Required]
        [DisplayName("Patient Full Name")]
        public string PatientFullName { get; set; }
        [DisplayName("Patient Date Of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime PatientDateOfBirth { get; set; }
        [DisplayName("Patient Gender")]
        public Gender PatientGender { get; set; }
        [DisplayName("Patient MRN Number")]
        public long PatientMRNnumber { get; set; }
        [Required]
        [DisplayName("Surgery Name")]
        public string SurgeryName { get; set; }
        [DisplayName("Surgery Site")]
        public string SurgerySite { get; set; }
        [DisplayName("Surgery Department")]
        public int SurgicalDepartmentId { get; set; }
        public SurgicalDepartment SurgicalDepartment { get; set; }
        [DisplayName("Surgeon Name")]
        [Required]
        public string SurgeonID { get; set; }
        [ForeignKey("SurgeonID")]
        public ApplicationUser Surgeon { get; set; }
        [DisplayName("Blood Requested")]
        public bool BloodRequested { get; set; }
        public string BloodRequestedText{get;set;}
        [DisplayName("Post Operative Care Requested")]        
        public bool RequestedPostOperativeCare { get; set; }
        [DisplayName("Surgery Position")]        
        public bool SurgeryPosition { get; set; }
        public string SurgeryPositionText { get; set; }
        [DisplayName("Frozen Section")]
        public bool FrozenSection { get; set; }
        [DisplayName("Special Things Like Sutures")]
        public bool SpecialThingsLikeSutures { get; set; }        
        public string SpecialThingsLikeSuturesText { get; set; }
        public bool Consumables { get; set; }
        public string ConsumablesText { get; set; }
        [DisplayName("Anesthesia Technique")]
        public int AnesthesiaTechniqueId { get; set; }
        public AnesthesiaTechnique AnesthesiaTechnique { get; set; }
        [DisplayName("Special Devices")]
        public bool SpecialDevices { get; set; }
        public string SpecialDevicesText { get; set; }
        public bool Turniquet { get; set; }
        public bool CArm { get; set; }
        public bool Harmonic { get; set; }
        public bool Ligasure { get; set; }
        
        public bool Microscope { get; set; }
        public string Others { get; set; }
        [DisplayName("Anesthetic Name")]
        public int? AnestheticsId { get; set; }
        public Anesthetics Anesthetics { get; set; }
        [DisplayName("Appointment Status")]
        public AppointmentStatus AppointmentStatus { get; set; }       
        [DisplayName("Patient Status")]
        public PatientStatus PatientStatus { get; set; }   
        [DisplayName("Booked Items")]
        public List<ItemsBooked> ItemsBooked { get; set; }

        public List<SMS> SMS { get; set; }
    }

    public enum Gender : byte
    {
        Male,
        Female,
        Unknown,
        NA
    }
    public enum AppointmentStatus:byte
    { 
        Pending,
        Approved,
        Denied
    }

    public enum PatientStatus:byte
    {
        Pending,
        Arrived,
        Cancelled,
        Postponed
    }

   
}
