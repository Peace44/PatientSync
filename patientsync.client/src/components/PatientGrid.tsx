import React, { useState } from "react";
import { Patient } from "../models/Patient";
import "./PatientGrid.css"; 

interface PatientGridProps {
    patients: Patient[];
    onPatientClick: (patient: Patient) => void;
}

// Function to format dates as "dd-MMM-yyyy"
const formatDate = (dateString: string) => {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-GB", {
        day: "2-digit",
        month: "short",
        year: "numeric",
    }).replace(/ /g, "-");
};

const PatientGrid: React.FC<PatientGridProps> = ({ patients, onPatientClick }) => {
    const [sortColumn, setSortColumn] = useState<keyof Patient | null>(null);
    const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

    // Function to handle sorting logic
    const handleSort = (column: keyof Patient) => {
        if (sortColumn === column) setSortOrder(sortOrder === "asc" ? "desc" : "asc"); // If the same column is clicked again, toggle sort order
        else { // set a new column and default to ascending order
            setSortColumn(column);
            setSortOrder("asc");
        }
    };

    // Sort patients based on the selected column & order
    const sortedPatients = [...patients].sort((a, b) => {
        if (!sortColumn) return 0; // If no sorting column is selected, return original order

        let valueA: any = a[sortColumn];
        let valueB: any = b[sortColumn];

        // Handle Birth Date Sorting
        if (sortColumn === "birthDate") {
            valueA = new Date(a.birthDate).getTime();
            valueB = new Date(b.birthDate).getTime();
        }
        // Handle Status Sorting (Convert Alarm to Numeric Value)
        else if (sortColumn === "status") {
            const hasAlarmA = a.parameters.some((param) => param.alarm);
            const hasAlarmB = b.parameters.some((param) => param.alarm);
            valueA = hasAlarmA ? 1 : 0; // "Alarm" → 1, "OK" → 0
            valueB = hasAlarmB ? 1 : 0;
        }
        // Handle # Parameters Sorting
        else if (sortColumn === "parametersCount") {
            valueA = a.parameters.length; // Get the number of parameters
            valueB = b.parameters.length;
        }
        // Handle Normal String or Number Sorting
        else {
            valueA = a[sortColumn];
            valueB = b[sortColumn];
        }

        // Handle string comparisons
        if (typeof valueA === "string" && typeof valueB === "string") {
            return sortOrder === "asc"
                ? valueA.localeCompare(valueB)
                : valueB.localeCompare(valueA);
        }

        // Handle number comparisons
        if (typeof valueA === "number" && typeof valueB === "number") {
            return sortOrder === "asc" ? valueA - valueB : valueB - valueA;
        }

        return 0;
    });

    //// For simplicity, sort by FamilyName (extendable to other criteria)
    //const sortedPatients = [...patients].sort((a, b) =>
    //    a.familyName.localeCompare(b.familyName)
    //);

    return (
        <table className="table">
            <thead>
                <tr>
                    <th className="table-header" onClick={() => handleSort("familyName")}>
                        Family Name {sortColumn === "familyName" ? (sortOrder === "asc" ? "▲" : "▼") : ""}
                    </th>
                    <th className="table-header" onClick={() => handleSort("givenName")}>
                        Given Name {sortColumn === "givenName" ? (sortOrder === "asc" ? "▲" : "▼") : ""}
                    </th>
                    <th className="table-header" onClick={() => handleSort("sex")}>
                        Sex {sortColumn === "sex" ? (sortOrder === "asc" ? "▲" : "▼") : ""}
                    </th>
                    <th className="table-header" onClick={() => handleSort("birthDate")}>
                        Birth Date {sortColumn === "birthDate" ? (sortOrder === "asc" ? "▲" : "▼") : ""}
                    </th>
                    <th className="table-header" onClick={() => handleSort("parametersCount")}>
                        # Parameters {sortColumn === "parametersCount" ? (sortOrder === "asc" ? "▲" : "▼") : ""}
                    </th>
                    <th className="table-header" onClick={() => handleSort("status")}>
                        Status {sortColumn === "status" ? (sortOrder === "asc" ? "▲" : "▼") : ""}
                    </th>
                </tr>
            </thead>
            <tbody>
                {sortedPatients.map((patient) => {
                    // Determine if any parameter has an alarm set to true
                    const hasAlarm = patient.parameters.some((param) => param.alarm);

                    return (
                        <tr
                            key={patient.id}
                            onClick={() => onPatientClick(patient)}
                            className="table-row"
                        >
                            <td className="table-cell">{patient.familyName}</td>
                            <td className="table-cell">{patient.givenName}</td>
                            <td className="table-cell">{patient.sex}</td>
                            <td className="table-cell">{formatDate(patient.birthDate)}</td>
                            <td className="table-cell">{patient.parameters.length}</td>
                            <td className={`table-cell ${hasAlarm ? "alarm-label" : "ok-label"}`}>
                                {hasAlarm ? "🚨 ALARM 🚨" : "✅ OKAY ✅"}
                            </td>
                        </tr>
                    );
                })}
            </tbody>
        </table>
    );
};

export default PatientGrid;