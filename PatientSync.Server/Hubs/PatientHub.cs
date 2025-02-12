using Microsoft.AspNetCore.SignalR;
using Serilog;
using System.Threading.Tasks;

namespace PatientSync.Server.Hubs
{
    /// <summary>
    /// A SignalR hub that broadcasts patient update notifications to all connected clients.
    /// </summary>
    public class PatientHub : Hub
    {
        public PatientHub()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        // Called by a client to notify all other connections for the same user to open the patient detail dialog.
        // <param name="patientId">The ID of the patient whose details should be displayed.</param>
        // <returns>A Task representing the asynchronous operation.</returns>
        public async Task SyncOpenPatientDetail(int patientID)
        {
            Log.Information("SyncOpenPatientDetail called with patientID: {PatientID}", patientID);

            try
            {
                if (Context.UserIdentifier != null)
                {
                    await Clients.User(Context.UserIdentifier).SendAsync("OpenPatientDetail", patientID); // Broadcast the "OpenPatientDetail" event to all connections for this user, excluding the caller if desired
                    Log.Information("OpenPatientDetail event sent to user: {UserIdentifier}", Context.UserIdentifier);
                }
                else Log.Warning("UserIdentifier is null. Cannot send OpenPatientDetail event.");
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error occurred while sending OpenPatientDetail event for patientID: {PatientID}", patientID);
                throw;
            }
        }

        // This hub can be extended in the future with methods to
        // send targeted messages,
        // handle client methods invocations,
        // etc...
        // For now, it serves as a broadcast hub.
    }
}
