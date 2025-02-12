using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
using PatientSync.Server.Hubs;
using PatientSync.Server.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PatientSync.Server.Services
{
    /// <summary>
    /// A background service that periodically updates the alarm status for patient parameters.
    /// Every 10 seconds, it randomly selects one parameter for each patient and toggles its alarm status.
    /// It also notifies connected clients of changes via SignalR.
    /// </summary>
    public class AlarmUpdateService : BackgroundService
    {
        private readonly IRepository _repository; // The repo used to access & update patient data.
        private readonly IHubContext<PatientHub> _hubContext;
        private readonly Random _random; // A Random instance used to generate random numbers.
        private readonly ILogger<AlarmUpdateService> _logger; // Logger instance for logging.

        /// <summary>
        /// Initializes a new instance of the <see cref="AlarmUpdateService"/> class.
        /// </summary>
        /// <param name="repository">An implementation of IRepository to access patient data.</param>
        /// <param name="hubContext">The SignalR hub context for sending real-time updates.</param>
        /// <param name="logger">The logger instance for logging.</param>
        public AlarmUpdateService(IRepository repository, IHubContext<PatientHub> hubContext, ILogger<AlarmUpdateService> logger)
        {
            _repository = repository;
            _hubContext = hubContext;
            _random = new Random();
            _logger = logger;
        }

        /// <summary>
        /// Executes the background task that updates patient alarms and broadcasts updates.
        /// </summary>
        /// <param name="stoppingToken">A token to monitor for cancellation requests.</param>
        /// <returns>A Task representing the background operation.</returns>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("AlarmUpdateService started at: {time}", DateTimeOffset.Now);

            // Loop continuously until a cancellation is requested.
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Updating alarms at: {time}", DateTimeOffset.Now);
                UpdateAlarms(); // Update the alarm statuses for patients.
                await _hubContext.Clients.All.SendAsync("ReceivePatientUpdate", cancellationToken: stoppingToken); // Broadcast an update notification to all connected clients (this notifies them to refresh their patient data)
                _logger.LogInformation("Alarms updated and notification sent at: {time}", DateTimeOffset.Now);
                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken); // Wait for 10 seconds before updating again.
            }

            _logger.LogInformation("AlarmUpdateService stopped at: {time}", DateTimeOffset.Now);
        }

        /// <summary>
        /// Iterates through each patient and randomly updates the alarm status of one of its parameters.
        /// </summary>
        private void UpdateAlarms()
        {
            var patients = _repository.GetPatients().ToList(); // Retrieve a snapshot of the current patients list.
            _logger.LogInformation("Retrieved {count} patients from repository", patients.Count);

            foreach (var patient in patients)
            {
                // Only update if the patient has at least 1 parameter
                if (patient.Parameters.Any())
                {
                    int randomIndex = _random.Next(0, patient.Parameters.Count); // Choose a random parameter index within the patient's Parameters list
                    bool newAlarmStatus = _random.Next(0, 2) == 0; // Decide randomly whether the alarm should be true or false
                    patient.Parameters[randomIndex].Alarm = newAlarmStatus; // Update the alarm status of the selected parameter
                    _repository.UpdatePatient(patient); // Update the patient in the repository to reflect the change
                    _logger.LogInformation("Updated alarm status for patient {patientId}, parameter {parameterId} to {newAlarmStatus}", patient.ID, patient.Parameters[randomIndex].ID, newAlarmStatus);
                }
            }
        }
    }
}
