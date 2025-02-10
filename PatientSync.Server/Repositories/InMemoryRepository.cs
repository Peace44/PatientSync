using System;
using System.Collections.Generic;
using System.Linq;
using PatientSync.Server.Models;

namespace PatientSync.Server.Repositories
{
    public class InMemoryRepository : IRepository
    {
        // a. In-memory collections for Users and Patients
        private readonly List<User> _users = new List<User>();
        private readonly List<Patient> _patients = new List<Patient>();

        // b. Lock object for thread safety
        private readonly object _lock = new object();

        // c. Constructor to prepopulate data
        public InMemoryRepository()
        {
            // c.1. Prepopulate with a sample user
            _users.Add(new User { ID = 1, Username = "admin", Password = "password" });

            // c.2. Prepopulate with a sample patient and parameters (using the provided sample JSON)
            _patients.Add(new Patient
            {
                ID = 2,
                FamilyName = "Jack",
                GivenName = "Alfred",
                BirthDate = new DateTime(1949, 5, 25),
                Sex = "M",
                Parameters = new List<Parameter>
                {
                    new Parameter { ID = 0, Name = "356DHAAPH1", Value = "0.2810233897906837", Alarm = false },
                    new Parameter { ID = 1, Name = "J0EREMM0JI", Value = "0.8373179071756629", Alarm = true },
                    new Parameter { ID = 2, Name = "ZPL99W1THC", Value = "0.032467747122267146", Alarm = true }
                }
            });
        }



        // d. User operations
        public IEnumerable<User> GetUsers()
        {
            lock (_lock)
            {
                return _users.ToList();
            }
        }

        public User GetUserById(int id)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.ID == id);
            }
        }

        public User GetUserByUsername(string username)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
        }

        public void AddUser(User user)
        {
            lock (_lock)
            {
                user.ID = _users.Any() ? _users.Max(u => u.ID) + 1 : 1;
                _users.Add(user);
            }
        }

        public void UpdateUser(User user)
        {
            lock (_lock)
            {
                var index = _users.FindIndex(u => u.ID == user.ID);
                if (index != -1)
                {
                    _users[index] = user;
                }
            }
        }

        public void DeleteUser(int id)
        {
            lock (_lock)
            {
                var user = _users.FirstOrDefault(u => u.ID == id);
                if (user != null)
                {
                    _users.Remove(user);
                }
            }
        }



        // e. Patient operations
        public IEnumerable<Patient> GetPatients()
        {
            lock (_lock)
            {
                return _patients.ToList();
            }
        }

        public Patient GetPatientById(int id)
        {
            lock (_lock)
            {
                return _patients.FirstOrDefault(p => p.ID == id);
            }
        }

        public void AddPatient(Patient patient)
        {
            lock (_lock)
            {
                patient.ID = _patients.Any() ? _patients.Max(p => p.ID) + 1 : 1;
                _patients.Add(patient);
            }
        }

        public void UpdatePatient(Patient patient)
        {
            lock (_lock)
            {
                var index = _patients.FindIndex(p => p.ID == patient.ID);
                if (index != -1)
                {
                    _patients[index] = patient;
                }
            }
        }

        public void DeletePatient(int id)
        {
            lock (_lock)
            {
                var patient = _patients.FirstOrDefault(p => p.ID == id);
                if (patient != null)
                {
                    _patients.Remove(patient);
                }
            }
        }



        // f. Parameter operations (accessed via Patient)
        public IEnumerable<Parameter> GetParametersForPatient(int patientId)
        {
            lock (_lock)
            {
                var patient = _patients.FirstOrDefault(p => p.ID == patientId);
                return patient != null ? patient.Parameters.ToList() : Enumerable.Empty<Parameter>();
            }
        }

        public Parameter GetParameterById(int patientId, int parameterId)
        {
            lock (_lock)
            {
                var patient = _patients.FirstOrDefault(p => p.ID == patientId);
                return patient?.Parameters.FirstOrDefault(param => param.ID == parameterId);
            }
        }

        public void AddParameterToPatient(int patientId, Parameter parameter)
        {
            lock (_lock)
            {
                var patient = _patients.FirstOrDefault(p => p.ID == patientId);
                if (patient != null)
                {
                    // Auto-increment the parameter ID within the patient's list
                    parameter.ID = patient.Parameters.Any() ? patient.Parameters.Max(param => param.ID) + 1 : 0;
                    patient.Parameters.Add(parameter);
                }
            }
        }

        public void UpdateParameterForPatient(int patientId, Parameter parameter)
        {
            lock (_lock)
            {
                var patient = _patients.FirstOrDefault(p => p.ID == patientId);
                if (patient != null)
                {
                    var index = patient.Parameters.FindIndex(param => param.ID == parameter.ID);
                    if (index != -1)
                    {
                        patient.Parameters[index] = parameter;
                    }
                }
            }
        }

        public void DeleteParameterFromPatient(int patientId, int parameterId)
        {
            lock (_lock)
            {
                var patient = _patients.FirstOrDefault(p => p.ID == patientId);
                if (patient != null)
                {
                    var parameter = patient.Parameters.FirstOrDefault(param => param.ID == parameterId);
                    if (parameter != null)
                    {
                        patient.Parameters.Remove(parameter);
                    }
                }
            }
        }
    }
}
