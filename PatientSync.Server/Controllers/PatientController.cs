using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PatientSync.Server.Models;
using PatientSync.Server.Services;
using Serilog;

namespace PatientSync.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        // --- Patient Endpoints --- //

        // GET: api/patient
        [HttpGet]
        public ActionResult<IEnumerable<Patient>> GetAllPatients()
        {
            Log.Information("Getting all patients");
            var patients = _patientService.GetAllPatients();
            Log.Information("Retrieved {PatientCount} patients", patients.Count());
            return Ok(patients);
        }

        // GET: api/patient/{id}
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatientById(int id)
        {
            Log.Information("Getting patient with ID {PatientId}", id);
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                Log.Warning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }
            Log.Information("Retrieved patient with ID {PatientId}", id);
            return Ok(patient);
        }

        // POST: api/patient
        [HttpPost]
        public ActionResult AddPatient([FromBody] Patient patient)
        {
            Log.Information("Adding new patient");
            _patientService.AddPatient(patient);
            Log.Information("Added patient with ID {PatientId}", patient.ID);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.ID }, patient);
        }

        // PUT: api/patient/{id}
        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.ID)
            {
                Log.Warning("Patient ID mismatch: {PatientId} != {BodyPatientId}", id, patient.ID);
                return BadRequest("Patient ID mismatch");
            }
            var existing = _patientService.GetPatientById(id);
            if (existing == null)
            {
                Log.Warning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }
            Log.Information("Updating patient with ID {PatientId}", id);
            _patientService.UpdatePatient(patient);
            Log.Information("Updated patient with ID {PatientId}", id);
            return NoContent();
        }

        // DELETE: api/patient/{id}
        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            Log.Information("Deleting patient with ID {PatientId}", id);
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                Log.Warning("Patient with ID {PatientId} not found", id);
                return NotFound();
            }
            _patientService.DeletePatient(id);
            Log.Information("Deleted patient with ID {PatientId}", id);
            return NoContent();
        }

        // --- Parameter Endpoints for a Specific Patient --- //

        // GET: api/patient/{patientId}/parameters
        [HttpGet("{patientId}/parameters")]
        public ActionResult<IEnumerable<Parameter>> GetParametersForPatient(int patientId)
        {
            Log.Information("Getting parameters for patient with ID {PatientId}", patientId);
            var parameters = _patientService.GetParametersForPatient(patientId);
            Log.Information("Retrieved {ParameterCount} parameters for patient with ID {PatientId}", parameters.Count(), patientId);
            return Ok(parameters);
        }

        // GET: api/patient/{patientId}/parameters/{parameterId}
        [HttpGet("{patientId}/parameters/{parameterId}")]
        public ActionResult<Parameter> GetParameterById(int patientId, int parameterId)
        {
            Log.Information("Getting parameter with ID {ParameterId} for patient with ID {PatientId}", parameterId, patientId);
            var parameter = _patientService.GetParameterById(patientId, parameterId);
            if (parameter == null)
            {
                Log.Warning("Parameter with ID {ParameterId} for patient with ID {PatientId} not found", parameterId, patientId);
                return NotFound();
            }
            Log.Information("Retrieved parameter with ID {ParameterId} for patient with ID {PatientId}", parameterId, patientId);
            return Ok(parameter);
        }

        // POST: api/patient/{patientId}/parameters
        [HttpPost("{patientId}/parameters")]
        public ActionResult AddParameterToPatient(int patientId, [FromBody] Parameter parameter)
        {
            Log.Information("Adding parameter to patient with ID {PatientId}", patientId);
            _patientService.AddParameterToPatient(patientId, parameter);
            Log.Information("Added parameter with ID {ParameterId} to patient with ID {PatientId}", parameter.ID, patientId);
            return CreatedAtAction(nameof(GetParameterById), new { patientId, parameterId = parameter.ID }, parameter);
        }

        // PUT: api/patient/{patientId}/parameters/{parameterId}
        [HttpPut("{patientId}/parameters/{parameterId}")]
        public ActionResult UpdateParameterForPatient(int patientId, int parameterId, [FromBody] Parameter parameter)
        {
            if (parameterId != parameter.ID)
            {
                Log.Warning("Parameter ID mismatch: {ParameterId} != {BodyParameterId}", parameterId, parameter.ID);
                return BadRequest("Parameter ID mismatch");
            }
            var existing = _patientService.GetParameterById(patientId, parameterId);
            if (existing == null)
            {
                Log.Warning("Parameter with ID {ParameterId} for patient with ID {PatientId} not found", parameterId, patientId);
                return NotFound();
            }
            Log.Information("Updating parameter with ID {ParameterId} for patient with ID {PatientId}", parameterId, patientId);
            _patientService.UpdateParameterForPatient(patientId, parameter);
            Log.Information("Updated parameter with ID {ParameterId} for patient with ID {PatientId}", parameterId, patientId);
            return NoContent();
        }

        // DELETE: api/patient/{patientId}/parameters/{parameterId}
        [HttpDelete("{patientId}/parameters/{parameterId}")]
        public ActionResult DeleteParameterFromPatient(int patientId, int parameterId)
        {
            Log.Information("Deleting parameter with ID {ParameterId} from patient with ID {PatientId}", parameterId, patientId);
            var parameter = _patientService.GetParameterById(patientId, parameterId);
            if (parameter == null)
            {
                Log.Warning("Parameter with ID {ParameterId} for patient with ID {PatientId} not found", parameterId, patientId);
                return NotFound();
            }
            _patientService.DeleteParameterFromPatient(patientId, parameterId);
            Log.Information("Deleted parameter with ID {ParameterId} from patient with ID {PatientId}", parameterId, patientId);
            return NoContent();
        }
    }
}

