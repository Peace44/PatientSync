import { Parameter } from "./Parameter";

/**
 * Represents a patient with personal details and a collection of parameters.
 */
export interface Patient {
    id: number;
    familyName: string;
    givenName: string;
    birthDate: string; // The patient's birth date in ISO string format.
    sex: string;
    parameters: Parameter[]; // A list of parameters associated with the patient.
}
