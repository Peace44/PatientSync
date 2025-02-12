using System;
using System.Collections.Generic;
using System.Linq;
using PatientSync.Server.Models;
using Serilog;

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
            Log.Information("Initializing InMemoryRepository with sample data.");

            // c.1. Prepopulate with sample users
            AddUser(new User(new AuthModel { Username = "admin", Password = "password" }));
            AddUser(new User(new AuthModel { Username = "peace", Password = "peace" }));

            // c.2. Prepopulate with sample patients and parameters (using the provided sample JSON)
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

            
            AddPatient(new Patient
            {
                ID = 3,
                FamilyName = "Anderson",
                GivenName = "Emma",
                BirthDate = new DateTime(1975, 12, 1),
                Sex = "F",
                Parameters = new List<Parameter>
                    {
                        new Parameter { ID = 0, Name = "ZPL99W1THC", Value = "0.4877147362745184", Alarm = false },
                        new Parameter { ID = 1, Name = "356DHAAPH1", Value = "0.3575789268978416", Alarm = false },
                        new Parameter { ID = 2, Name = "LKJ76HGF", Value = "0.3782423300440987", Alarm = false },
                        new Parameter { ID = 3, Name = "YTR56PLM", Value = "0.6198349201124583", Alarm = true }
                    }
            });

            _patients.Add(new Patient
            {
                ID = 4,
                FamilyName = "Miller",
                GivenName = "Daniela",
                BirthDate = new DateTime(1925, 11, 11),
                Sex = "F",
                Parameters = new List<Parameter>
                    {
                        new Parameter { ID = 2, Name = "LKJ76HGF", Value = "0.5071539883483737", Alarm = false }
                    }
            });

            _patients.Add(new Patient
            {
                ID = 5,
                FamilyName = "Smith",
                GivenName = "Alice",
                BirthDate = new DateTime(1978, 8, 27),
                Sex = "F",
                Parameters = new List<Parameter>
                    {
                        new Parameter { ID = 1, Name = "XCV12BNM", Value = "0.2640726908209953", Alarm = true },
                        new Parameter { ID = 2, Name = "LKJ76HGF", Value = "0.3619824529060439", Alarm = true }
                    }
            });

            _patients.Add(new Patient
            {
                ID = 6,
                FamilyName = "Thomas",
                GivenName = "Robert",
                BirthDate = new DateTime(2003, 6, 20),
                Sex = "M",
                Parameters = new List<Parameter>
                    {
                        new Parameter { ID = 0, Name = "XCV12BNM", Value = "0.9902347261278025", Alarm = false },
                        new Parameter { ID = 1, Name = "LKJ76HGF", Value = "0.2246987125053669", Alarm = false }
                    }
            });

            Log.Information("InMemoryRepository initialized with {UserCount} users and {PatientCount} patients.", _users.Count, _patients.Count);
        }

        // d. User operations
        public IEnumerable<User> GetUsers()
        {
            lock (_lock)
            {
                Log.Information("Fetching all users.");
                return _users.ToList();
            }
        }

        public User GetUserById(int id)
        {
            lock (_lock)
            {
                Log.Information("Fetching user with ID {UserId}.", id);
                return _users.FirstOrDefault(u => u.ID == id);
            }
        }

        public User GetUserByUsername(string username)
        {
            lock (_lock)
            {
                Log.Information("Fetching user with username {Username}.", username);
                return _users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
            }
        }

        public void AddUser(User user)
        {
            lock (_lock)
            {
                if (user.ID == null) user.ID = _users.Any() ? _users.Max(u => u.ID) + 1 : 1;
                _users.Add(user);
                Log.Information("Added new user with ID {UserId} and username {Username}.", user.ID, user.Username);
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
                    Log.Information("Updated user with ID {UserId}.", user.ID);
                }
                else Log.Warning("User with ID {UserId} not found for update.", user.ID);
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
                    Log.Information("Deleted user with ID {UserId}.", id);
                }
                else Log.Warning("User with ID {UserId} not found for deletion.", id);
            }
        }

        // e. Patient operations
        public IEnumerable<Patient> GetPatients()
        {
            lock (_lock)
            {
                Log.Information("Fetching all patients.");
                return _patients.ToList();
            }
        }

        public Patient GetPatientById(int id)
        {
            lock (_lock)
            {
                Log.Information("Fetching patient with ID {PatientId}.", id);
                return _patients.FirstOrDefault(p => p.ID == id);
            }
        }

        public void AddPatient(Patient patient)
        {
            lock (_lock)
            {
                if (patient.ID == null) patient.ID = _patients.Any() ? _patients.Max(p => p.ID) + 1 : 1;
                _patients.Add(patient);
                Log.Information("Added new patient with ID {PatientId} and name {PatientName}.", patient.ID, $"{patient.GivenName} {patient.FamilyName}");
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
                    Log.Information("Updated patient with ID {PatientId}.", patient.ID);
                }
                else Log.Warning("Patient with ID {PatientId} not found for update.", patient.ID);
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
                    Log.Information("Deleted patient with ID {PatientId}.", id);
                }
                else Log.Warning("Patient with ID {PatientId} not found for deletion.", id);
            }
        }

        // f. Parameter operations (accessed via Patient)
        public IEnumerable<Parameter> GetParametersForPatient(int patientId)
        {
            lock (_lock)
            {
                Log.Information("Fetching parameters for patient with ID {PatientId}.", patientId);
                var patient = _patients.FirstOrDefault(p => p.ID == patientId);
                return patient != null ? patient.Parameters.ToList() : Enumerable.Empty<Parameter>();
            }
        }

        public Parameter GetParameterById(int patientId, int parameterId)
        {
            lock (_lock)
            {
                Log.Information("Fetching parameter with ID {ParameterId} for patient with ID {PatientId}.", parameterId, patientId);
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
                    if (parameter.ID == null) parameter.ID = patient.Parameters.Any() ? patient.Parameters.Max(param => param.ID) + 1 : 0;
                    patient.Parameters.Add(parameter);
                    Log.Information("Added new parameter with ID {ParameterId} to patient with ID {PatientId}.", parameter.ID, patientId);
                }
                else Log.Warning("Patient with ID {PatientId} not found for adding parameter.", patientId);
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
                        Log.Information("Updated parameter with ID {ParameterId} for patient with ID {PatientId}.", parameter.ID, patientId);
                    }
                    else Log.Warning("Parameter with ID {ParameterId} not found for update in patient with ID {PatientId}.", parameter.ID, patientId);
                }
                else Log.Warning("Patient with ID {PatientId} not found for updating parameter.", patientId);
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
                        Log.Information("Deleted parameter with ID {ParameterId} from patient with ID {PatientId}.", parameterId, patientId);
                    }
                    else Log.Warning("Parameter with ID {ParameterId} not found for deletion in patient with ID {PatientId}.", parameterId, patientId);
                }
                else Log.Warning("Patient with ID {PatientId} not found for deleting parameter.", patientId);
            }
        }
    }
}