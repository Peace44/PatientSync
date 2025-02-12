import axios from "axios";
import { Parameter } from "../models/Parameter";
import { Patient } from "../models/Patient";
import { User } from "../models/User";

// Create an Axios instance with default configuration
const apiClient = axios.create({
    baseURL: "/",
    headers: {
        "Content-Type": "application/json",
    },
    withCredentials: true, // Ensure cookies are included in the requests!!!
});



//
// Patient Operations
//

/**
 * Retrieves the list of patients from the back-end API.
 * @returns A promise that resolves to an array of Patient objects.
 */
export const getPatients = async (): Promise<Patient[]> => {
    try {
        const response = await apiClient.get<Patient[]>("/patient");
        console.log("API Response: ", response.data);
        return Array.isArray(response.data) ? response.data : [];
    } catch (error) {
        console.error("API ERROR: ", error);
        return [];
    }
};

/**
 * Retrieves a patient by their ID.
 * @param id The patient's unique ID.
 * @returns A promise that resolves to a Patient object.
 */
export const getPatientById = async (id: number): Promise<Patient> => {
    const response = await apiClient.get<Patient>(`/patient/${id}`);
    return response.data;
};

/**
 * Adds a new patient.
 * @param patient The patient object to add.
 * @returns A promise that resolves to the newly created Patient object.
 */
export const addPatient = async (patient: Patient): Promise<Patient> => {
    const response = await apiClient.post<Patient>("/patient", patient);
    return response.data;
};

/**
 * Updates an existing patient.
 * @param id The ID of the patient to update.
 * @param patient The updated patient object.
 */
export const updatePatient = async (id: number, patient: Patient): Promise<void> => {
    await apiClient.put(`/patient/${id}`, patient);
};

/**
 * Deletes a patient by ID.
 * @param id The ID of the patient to delete.
 */
export const deletePatient = async (id: number): Promise<void> => {
    await apiClient.delete(`/patient/${id}`);
};





//
// Parameter Operations (for a given patient)
//

/**
 * Retrieves all parameters for a specific patient.
 * @param patientId The patient's unique ID.
 * @returns A promise that resolves to an array of Parameter objects.
 */
export const getParametersForPatient = async (patientId: number): Promise<Parameter[]> => {
    const response = await apiClient.get<Parameter[]>(`/patient/${patientId}/parameters`);
    return response.data;
};

/**
 * Retrieves a specific parameter for a patient.
 * @param patientId The patient's unique ID.
 * @param parameterId The parameter's unique ID.
 * @returns A promise that resolves to a Parameter object.
 */
export const getParameterById = async (patientId: number, parameterId: number): Promise<Parameter> => {
    const response = await apiClient.get<Parameter>(`/patient/${patientId}/parameters/${parameterId}`);
    return response.data;
};

/**
 * Adds a new parameter to a patient.
 * @param patientId The patient's unique ID.
 * @param parameter The parameter object to add.
 * @returns A promise that resolves to the newly created Parameter object.
 */
export const addParameterToPatient = async (patientId: number, parameter: Parameter): Promise<Parameter> => {
    const response = await apiClient.post<Parameter>(`/patient/${patientId}/parameters`, parameter);
    return response.data;
};

/**
 * Updates an existing parameter for a patient.
 * @param patientId The patient's unique ID.
 * @param parameterId The parameter's unique ID.
 * @param parameter The updated parameter object.
 */
export const updateParameterForPatient = async (patientId: number, parameterId: number, parameter: Parameter): Promise<void> => {
    await apiClient.put(`/patient/${patientId}/parameters/${parameterId}`, parameter);
};

/**
 * Deletes a parameter from a patient.
 * @param patientId The patient's unique ID.
 * @param parameterId The parameter's unique ID.
 */
export const deleteParameterFromPatient = async (patientId: number, parameterId: number): Promise<void> => {
    await apiClient.delete(`/patient/${patientId}/parameters/${parameterId}`);
};





//
// User Operations
//

/**
 * Retrieves all users.
 * @returns A promise that resolves to an array of User objects.
 */
export const getUsers = async (): Promise<User[]> => {
    const response = await apiClient.get<User[]>("/user");
    return response.data;
};

/**
 * Retrieves a user by their ID.
 * @param id The user's unique ID.
 * @returns A promise that resolves to a User object.
 */
export const getUserById = async (id: number): Promise<User> => {
    const response = await apiClient.get<User>(`/user/${id}`);
    return response.data;
};

/**
 * Retrieves a user by their username.
 * @param username The user's username.
 * @returns A promise that resolves to a User object.
 */
export const getUserByUsername = async (username: string): Promise<User> => {
    const response = await apiClient.get<User>(`/user/byusername/${username}`);
    return response.data;
};

/**
 * Adds a new user.
 * @param user The user object to add.
 * @returns A promise that resolves to the newly created User object.
 */
export const addUser = async (user: User): Promise<User> => {
    const response = await apiClient.post<User>("/user", user);
    return response.data;
};

/**
 * Updates an existing user.
 * @param id The user's unique ID.
 * @param user The updated user object.
 */
export const updateUser = async (id: number, user: User): Promise<void> => {
    await apiClient.put(`/user/${id}`, user);
};

/**
 * Deletes a user by ID.
 * @param id The user's unique ID.
 */
export const deleteUser = async (id: number): Promise<void> => {
    await apiClient.delete(`/user/${id}`);
};