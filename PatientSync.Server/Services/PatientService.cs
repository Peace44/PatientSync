using System.Collections.Generic;
using PatientSync.Server.Models;
using PatientSync.Server.Repositories;

namespace PatientSync.Server.Services
{
    public class PatientService : IPatientService
    {
        private readonly IRepository _repository;

        public PatientService(IRepository repository)
        {
            _repository = repository;
        }



        // Patient operations
        public IEnumerable<Patient> GetAllPatients() => _repository.GetPatients();
        public Patient GetPatientById(int id) => _repository.GetPatientById(id);
        public void AddPatient(Patient patient) => _repository.AddPatient(patient);
        public void UpdatePatient(Patient patient) => _repository.UpdatePatient(patient);
        public void DeletePatient(int id) => _repository.DeletePatient(id);
        


        // Parameter operations
        public IEnumerable<Parameter> GetParametersForPatient(int patientId) => _repository.GetParametersForPatient(patientId);
        public Parameter GetParameterById(int patientId, int parameterId) => _repository.GetParameterById(patientId, parameterId);
        public void AddParameterToPatient(int patientId, Parameter parameter) => _repository.AddParameterToPatient(patientId, parameter);
        public void UpdateParameterForPatient(int patientId, Parameter parameter) => _repository.UpdateParameterForPatient(patientId, parameter);
        public void DeleteParameterFromPatient(int patientId, int parameterId) => _repository.DeleteParameterFromPatient(patientId, parameterId);
    }
}
