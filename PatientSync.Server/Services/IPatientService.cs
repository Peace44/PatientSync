using System.Collections.Generic;
using PatientSync.Server.Models;

namespace PatientSync.Server.Services
{
    public interface IPatientService
    {
        // Patient operations
        IEnumerable<Patient> GetAllPatients();
        Patient GetPatientById(int id);
        void AddPatient(Patient patient);
        void UpdatePatient(Patient patient);
        void DeletePatient(int id);



        // Parameter operations for a given patient
        IEnumerable<Parameter> GetParametersForPatient(int patientId);
        Parameter GetParameterById(int patientId, int parameterId);
        void AddParameterToPatient(int patientId, Parameter parameter);
        void UpdateParameterForPatient(int patientId, Parameter parameter);
        void DeleteParameterFromPatient(int patientId, int parameterId);
    }
}
