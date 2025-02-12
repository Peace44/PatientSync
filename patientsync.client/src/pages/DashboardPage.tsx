import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import PatientGrid from "../components/PatientGrid";
import PatientDetailModal from "../components/PatientDetailModal";
import { getPatients } from "../services/apiService";
import { subscribeToPatientUpdates } from "../services/signalRService";
import { Patient } from "../models/Patient";
import "./DashboardPage.css";



const DashboardPage: React.FC = () => {
    const navigate = useNavigate();

    // Define state variables
    const [patients, setPatients] = useState<Patient[]>([]);
    const [selectedPatient, setSelectedPatient] = useState<Patient | null>(null);
    const [loggedInUser, setLoggedInUser] = useState<string | null>(null);

    // Define filter states
    const [filterFamilyName, setFilterFamilyName] = useState("");
    const [filterGivenName, setFilterGivenName] = useState("");
    const [filterSex, setFilterSex] = useState("");
    const [filterBirthDateStart, setFilterBirthDateStart] = useState("");
    const [filterBirthDateEnd, setFilterBirthDateEnd] = useState("");
    const [filterStatus, setFilterStatus] = useState("");

    // Fetch logged-in user info
    const fetchUserInfo = async () => {
        try {
            const response = await axios.get("/authentication/me", { withCredentials: true });
            setLoggedInUser(response.data.username);
        } catch (error) {
            console.error("FAILED TO FETCH USER INFO:", error);
            setLoggedInUser(null);
        }
    };


    // Define the function to load patient data
    const loadPatients = async () => {
        try {
            const data = await getPatients();
            console.log("Fetched patients from API:\n", data);

            if (!Array.isArray(data)) {
                console.error("UNEXPECTED RESPONSE FORMAT: EXPECTED AN ARRAY!", data);
                setPatients([]); // Fallback to an empty array
            }
            else {
                setPatients(data);
            }
        } catch (error) {
            console.error("FAILED TO LOAD PATIENTS:", error);
            setPatients([]); // Ensure state remains an array even if the API fails
        }
    };

    // Callback for general updates
    const handleUpdate = () => {
        console.log("SignalR update received. Reloading patients...");
        loadPatients();
    };

    // Callback for opening patient detail dialogs (synchronizing across tabs)
    const handleOpenDetail = (patientId: number) => {
        console.log("Synchronizing detail dialog for patientId:", patientId);
        // Find the patient by ID and open the detail dialog.
        const patientToOpen = patients.find((p) => p.id === patientId);
        if (patientToOpen) {
            setSelectedPatient(patientToOpen);
        }
    };

    // useEffect to load patients on component mount and subscribe to updates
    useEffect(() => {
        fetchUserInfo();
        loadPatients();
        subscribeToPatientUpdates(handleUpdate, handleOpenDetail);
    }, []);


    // Advanced filtering logic (case-insensitive)
    const filteredPatients = Array.isArray(patients)
        ? patients.filter((patient) => {
            const matchesFamilyName = patient.familyName
                ?.toLowerCase()
                .includes(filterFamilyName.toLowerCase());
            const matchesGivenName = patient.givenName
                ?.toLowerCase()
                .includes(filterGivenName.toLowerCase());
            const matchesSex =
                filterSex === "" || patient.sex.toLowerCase() === filterSex.toLowerCase();

            // Convert birth date to a comparable format (YYYY-MM-DD)
            const birthDate = new Date(patient.birthDate).toISOString().split("T")[0];

            // Check if birth date is within the selected range
            const matchesBirthDateRange =
                (filterBirthDateStart === "" || birthDate >= filterBirthDateStart) &&
                (filterBirthDateEnd === "" || birthDate <= filterBirthDateEnd);

            const hasAlarm = patient.parameters.some((param) => param.alarm);
            const matchesStatus =
                filterStatus === "" ||
                (filterStatus.toLowerCase() === "alarm" && hasAlarm) ||
                (filterStatus.toLowerCase() === "ok" && !hasAlarm);

            return matchesFamilyName && matchesGivenName && matchesSex && matchesBirthDateRange && matchesStatus;
        })
        :
        [];

    // Event handlers
    const handlePatientClick = (patient: Patient) => {
        setSelectedPatient(patient);
    };

    const handleModalClose = () => {
        setSelectedPatient(null);
    };

    const handleLogout = async () => {
        try {
            await axios.post("/authentication/logout", {}, { withCredentials: true })
            navigate("/authentication/login");
        } catch (error) {
            console.error("LOGOUT ERROR: ", error);
        }
    };

    return (
        <div className="dashboard-container">

            <header className="dashboard-header">
                <h2>Patient Dashboard</h2>
                <div className="user-info">
                    {loggedInUser ? `Welcome, ${loggedInUser}` : "NO LOGGED-IN USER!"}
                </div>
                <button onClick={handleLogout} className="logout-button">Logout</button>
            </header>

            {/* Filter inputs */}
            <div className="filter-inputs">
                <input
                    type="text"
                    placeholder="Filter by Family Name..."
                    value={filterFamilyName}
                    onChange={(e) => setFilterFamilyName(e.target.value)}
                    style={{ padding: "8px", width: "200px" }}
                />
                <input
                    type="text"
                    placeholder="Filter by Given Name..."
                    value={filterGivenName}
                    onChange={(e) => setFilterGivenName(e.target.value)}
                    style={{ padding: "8px", width: "200px" }}
                />
                <select
                    value={filterSex}
                    onChange={(e) => setFilterSex(e.target.value)}
                    style={{ padding: "8px", width: "150px" }}
                >
                    <option value="">Filter by Sex...</option>
                    <option value="M">Male</option>
                    <option value="F">Female</option>
                </select>
                {/* Birth Date Range Filtering */}
                <div style={{ display: "flex", flexDirection: "column", alignItems: "center" }}>
                    <label style={{ fontSize: "14px", fontWeight: "bold" }}>[FROM</label>
                    <input
                        type="date"
                        value={filterBirthDateStart}
                        onChange={(e) => setFilterBirthDateStart(e.target.value)}
                        style={{ padding: "8px", width: "180px" }}
                    />
                </div>
                <div style={{ display: "flex", flexDirection: "column", alignItems: "center" }}>
                    <label style={{ fontSize: "14px", fontWeight: "bold" }}>TO]</label>
                    <input
                        type="date"
                        value={filterBirthDateEnd}
                        onChange={(e) => setFilterBirthDateEnd(e.target.value)}
                        style={{ padding: "8px", width: "180px" }}
                    />
                </div>
                <select
                    value={filterStatus}
                    onChange={(e) => setFilterStatus(e.target.value)}
                    style={{ padding: "8px", width: "150px" }}
                >
                    <option value="">Filter by Status...</option>
                    <option value="OK">OK</option>
                    <option value="Alarm">Alarm</option>
                </select>
            </div>
                
            {/* Instructions */}
            <text>To search for specific patients, use the above filters...</text>
            <br></br>
            <text>Click on a patient to view their details...</text>
            <br></br>
            <text>To sort a column in ascending or descending order, click on its header...</text>
  
            {/* Render the patient grid */}
            {filteredPatients.length > 0 ? (
                <PatientGrid patients={filteredPatients} onPatientClick={handlePatientClick} />
            ) : (
                <p>NO PATIENTS FOUND!</p>
            )}
            {/* Conditionally render the detail modal */}
            {selectedPatient && (
                <PatientDetailModal patient={selectedPatient} onClose={handleModalClose} />
            )}
        </div>
    );
};

export default DashboardPage;