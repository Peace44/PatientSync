using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using PatientSync.Server.Models;
using PatientSync.Server.Services;

namespace PatientSync.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var patients = _patientService.GetAllPatients();
            return Ok(patients);
        }

        // GET: api/patient/{id}
        [HttpGet("{id}")]
        public ActionResult<Patient> GetPatientById(int id)
        {
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }
            return Ok(patient);
        }

        // POST: api/patient
        [HttpPost]
        public ActionResult AddPatient([FromBody] Patient patient)
        {
            _patientService.AddPatient(patient);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.ID }, patient);
        }

        // PUT: api/patient/{id}
        [HttpPut("{id}")]
        public ActionResult UpdatePatient(int id, [FromBody] Patient patient)
        {
            if (id != patient.ID)
            {
                return BadRequest("Patient ID mismatch");
            }
            var existing = _patientService.GetPatientById(id);
            if (existing == null)
            {
                return NotFound();
            }
            _patientService.UpdatePatient(patient);
            return NoContent();
        }

        // DELETE: api/patient/{id}
        [HttpDelete("{id}")]
        public ActionResult DeletePatient(int id)
        {
            var patient = _patientService.GetPatientById(id);
            if (patient == null)
            {
                return NotFound();
            }
            _patientService.DeletePatient(id);
            return NoContent();
        }





        // --- Parameter Endpoints for a Specific Patient --- //

        // GET: api/patient/{patientId}/parameters
        [HttpGet("{patientId}/parameters")]
        public ActionResult<IEnumerable<Parameter>> GetParametersForPatient(int patientId)
        {
            var parameters = _patientService.GetParametersForPatient(patientId);
            return Ok(parameters);
        }

        // GET: api/patient/{patientId}/parameters/{parameterId}
        [HttpGet("{patientId}/parameters/{parameterId}")]
        public ActionResult<Parameter> GetParameterById(int patientId, int parameterId)
        {
            var parameter = _patientService.GetParameterById(patientId, parameterId);
            if (parameter == null)
            {
                return NotFound();
            }
            return Ok(parameter);
        }

        // POST: api/patient/{patientId}/parameters
        [HttpPost("{patientId}/parameters")]
        public ActionResult AddParameterToPatient(int patientId, [FromBody] Parameter parameter)
        {
            _patientService.AddParameterToPatient(patientId, parameter);
            return CreatedAtAction(nameof(GetParameterById), new { patientId, parameterId = parameter.ID }, parameter);
        }

        // PUT: api/patient/{patientId}/parameters/{parameterId}
        [HttpPut("{patientId}/parameters/{parameterId}")]
        public ActionResult UpdateParameterForPatient(int patientId, int parameterId, [FromBody] Parameter parameter)
        {
            if (parameterId != parameter.ID)
            {
                return BadRequest("Parameter ID mismatch");
            }
            var existing = _patientService.GetParameterById(patientId, parameterId);
            if (existing == null)
            {
                return NotFound();
            }
            _patientService.UpdateParameterForPatient(patientId, parameter);
            return NoContent();
        }

        // DELETE: api/patient/{patientId}/parameters/{parameterId}
        [HttpDelete("{patientId}/parameters/{parameterId}")]
        public ActionResult DeleteParameterFromPatient(int patientId, int parameterId)
        {
            var parameter = _patientService.GetParameterById(patientId, parameterId);
            if (parameter == null)
            {
                return NotFound();
            }
            _patientService.DeleteParameterFromPatient(patientId, parameterId);
            return NoContent();
        }
    }
}

