import * as signalR from "@microsoft/signalr";

let connection: signalR.HubConnection | null = null; // Define a variable to hold the SignalR connection instance

/**
 * Starts the SignalR connection and registers event handlers.
 * @param onUpdate - Callback for general patient updates.
 * @param onOpenDetail - Callback for opening patient detail dialogs.
 */
export const startSignalRConnection = async (
    onUpdate: () => void,
    onOpenDetail: (patientId: number) => void) => {
        console.log("Starting SignalR connection...");
        connection = new signalR.HubConnectionBuilder()
            .withUrl("/patientHub", {withCredentials: true})
            .withAutomaticReconnect()
            .build();

        // When an update is received, call the provided callback (Register the event handler for patient updates)
        connection.on("ReceivePatientUpdate", () => {
            console.log("Received patient update notification from server!");
            onUpdate();
        });

        // Register the handler for opening patient detail dialogs.
        connection.on("OpenPatientDetail", (patientId: number) => {
            console.log("Received request to open patient detail for patientId:", patientId);
            onOpenDetail(patientId);
        });

        try {
            await connection.start();
            console.log("SignalR connection established!");
        } catch (err) {
            console.error("SignalR CONNECTION ERROR:", err);
        }
    };

/**
 * Subscribes to patient updates and detail dialog events.
 * @param onUpdate - Callback for general updates.
 * @param onOpenDetail - Callback for opening patient detail dialogs.
 */
export const subscribeToPatientUpdates = (
    onUpdate: () => void,
    onOpenDetail: (patientId: number) => void) => {
    console.log("Subscribing to patient updates...");
    if (!connection) {
        console.log("No existing connection found. Starting a new SignalR connection...");
        startSignalRConnection(onUpdate, onOpenDetail);
    } else {
        console.log("Using existing SignalR connection to subscribe to events...");
        connection.on("ReceivePatientUpdate", () => {
            console.log("Received patient update notification from server!");
            onUpdate();
        });
        connection.on("OpenPatientDetail", (patientId) => {
            console.log("Received open patient detail notification from server!");
            onOpenDetail(patientId);
        });
    }
};
