using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace PatientSync.Server.Models
{
    /// <summary>
    /// Represents a patient with personal details and a list of parameters
    /// </summary>
    public class Patient
    {
        public int? ID { get; set; } // Unique identifier for the patient.



        [Required(ErrorMessage = "FAMILY NAME IS REQUIRED!")]
        [StringLength(100, ErrorMessage = "FAMILY NAME CANNOT BE LONGER THAN 100 CHARACTERS!")]
        public string FamilyName { get; set; } // The patient's family (last) name.



        [Required(ErrorMessage = "GIVEN NAME IS REQUIRED!")]
        [StringLength(100, ErrorMessage = "GIVEN NAME CANNOT BE LONGER THAN 100 CHARACTERS!")]
        public string GivenName { get; set; } // The patient's given (first) name.



        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } // The patient's birth date.



        [Required(ErrorMessage = "SEX IS REQUIRED!")]
        [StringLength(10, ErrorMessage = "SEX CANNOT BE LONGER THAN 10 CHARACTERS!")]
        public string Sex { get; set; } // The patient's sex or gender.



        public List<Parameter> Parameters { get; set; } = new List<Parameter>(); // Collection of parameters associated with the patient.
    }
}
