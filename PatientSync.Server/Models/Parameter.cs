using System.ComponentModel.DataAnnotations;

namespace PatientSync.Server.Models
{
    /// <summary>
    ///  Represents a parameter associated with a patient.
    /// </summary>
    public class Parameter
    {
        public int? ID { get; set; } // Unique identifier for the parameter.



        [Required(ErrorMessage = "PARAMETER NAME IS REQUIRED!")]
        [StringLength(100, ErrorMessage = "PARAMETER NAME CANNOT BE LONGER THAN 100 CHARACTERS!")]
        public string Name { get; set; } // The name or code of the parameter.



        [Required(ErrorMessage = "PARAMETER VALUE IS REQUIRED!")]
        public string Value { get; set; } // The value of the parameter.



        [Required(ErrorMessage = "PARAMETER ALARM IS REQUIRED!")]
        public bool Alarm { get; set; } // Indicates whether this parameter is in alarm state.
    }
}
