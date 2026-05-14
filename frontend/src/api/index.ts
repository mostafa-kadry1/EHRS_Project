// Dedicated API modules — always prefer these over patient.api.ts
// for overlapping symbols (PatientProfileData, MedicalHistoryData, PrescriptionItem).
export * from './auth.api';
export * from './doctor.api';
export * from './appointments.api';
export * from './dashboard.api';
export * from './profile.api';
export * from './medical.api';
export * from './prescriptions.api';

// patient.api.ts re-declares some of the above types; export only the
// functions that are unique to it to avoid duplicate-export TS errors.
export {
  getPatientAppointments,
  cancelPatientAppointment,
  getPatientDashboard,
} from './patient.api';