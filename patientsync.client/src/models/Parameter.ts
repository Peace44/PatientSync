/**
 * Represents a parameter associated with a patient.
 */
export interface Parameter {
    id: number;
    name: string;
    value: string;
    alarm: boolean;
}