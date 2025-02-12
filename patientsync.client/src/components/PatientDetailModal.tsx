import React, { useEffect } from "react";
import { Patient } from "../models/Patient";
import "./PatientDetailModal.css";

interface PatientDetailModalProps {
    patient: Patient;
    onClose: () => void;
}

const PatientDetailModal: React.FC<PatientDetailModalProps> = ({ patient, onClose }) => {
    useEffect(() => {
        console.log("PatientDetailModal mounted");
        return () => {
            console.log("PatientDetailModal unmounted");
        };
    }, []);

    useEffect(() => {
        if (patient) {
            console.log("Patient data:", patient);
        } else {
            console.log("No patient data available");
        }
    }, [patient]);

    if (!patient) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-content">
                <div className="modal-header">
                    <h3>Patient Details</h3>
                </div>
                <div className="modal-body">
                    <p><strong>Family Name:</strong> {patient.familyName}</p>
                    <p><strong>Given Name:</strong> {patient.givenName}</p>
                    <p><strong>Sex:</strong> {patient.sex}</p>
                    <p><strong>Birth Date:</strong> {new Date(patient.birthDate).toLocaleDateString()}</p>
                    <h4>Parameters</h4>
                    <table className="modal-table">
                        <thead>
                            <tr>
                                <th>Name</th>
                                <th>Value</th>
                                <th>Alarm</th>
                            </tr>
                        </thead>
                        <tbody>
                            {patient.parameters && patient.parameters.length > 0 ? (
                                patient.parameters.map((param, index) => (
                                    <tr key={index} className={param.alarm ? "alarm-true" : "alarm-false"}>
                                        <td>{param.name}</td>
                                        <td>{param.value}</td>
                                        <td>{param.alarm ? "Yes 🚨" : "No ✅"}</td>
                                    </tr>
                                ))
                            ) : (
                                <tr>
                                    <td colSpan={3} style={{ textAlign: "center" }}>No parameters available</td>
                                </tr>
                            )}
                        </tbody>
                    </table>
                </div>
                <button className="modal-close-button" onClick={onClose}>
                    Close
                </button>
            </div>
        </div>
    );
};

export default PatientDetailModal;