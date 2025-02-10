using System.Collections.Generic;
using PatientSync.Server.Models;

namespace PatientSync.Server.Repositories
{
    public interface IRepository
    {
        // User operations
        IEnumerable<User> GetUsers();
        User GetUserById(int id);
        User GetUserByUsername(string username);
        void AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(int id);



        // Patient operations
        IEnumerable<Patient> GetPatients();
        Patient GetPatientById(int id);
        void AddPatient(Patient patient);
        void UpdatePatient(Patient patient);
        void DeletePatient(int id);



        // Parameter operations (accessed via Patient)
        IEnumerable<Parameter> GetParametersForPatient(int patientId);
        Parameter GetParameterById(int patientId, int parameterId);
        void AddParameterToPatient(int patientId, Parameter parameter);
        void UpdateParameterForPatient(int patientId, Parameter parameter);
        void DeleteParameterFromPatient(int patientId, int parameterId);
    }
}
