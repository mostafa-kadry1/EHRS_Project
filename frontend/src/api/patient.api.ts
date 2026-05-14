import api from '@/lib/api';

export interface Appointment {
  appointmentId?: number;
  id?: number;
  appointmentDateTime?: string;
  doctorId?: number;
  doctorName?: string;
  doctorProfilePicture?: string;
  reasonForVisit?: string;
  status?: string;
}

export async function getPatientAppointments() {
  const res = await api.get('/PatientAppointments');
  return res.data;
}

export async function cancelPatientAppointment(id: number) {
  const res = await api.post(`/PatientAppointments/${id}/cancel`);
  return res.data;
}

export async function getPatientDashboard() {
  const res = await api.get('/PatientDashboard');
  return res.data;
}

export async function getPatientProfile() {
  const res = await api.get('/PatientProfile');
  return res.data;
}

export async function updatePatientProfile(payload: FormData) {
  const res = await api.put('/PatientProfile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return res.data;
}

export interface MedicalHistoryData {
  chronicDiseases: string[];
  allergies: string[];
  surgeries: any[];
}

// NOTE: medical/profile/prescriptions are duplicated in dedicated api files
// (src/api/medical.api.ts, src/api/profile.api.ts, src/api/prescriptions.api.ts).
// To avoid export conflicts, these overlapping members are kept here only
// for now; components should prefer the dedicated files.


export async function getPatientMedicalHistory() {
  const res = await api.get('/PatientMedicalHistory');
  return res.data as MedicalHistoryData;
}

export async function updatePatientMedicalHistory(payload: Partial<MedicalHistoryData>) {
  const res = await api.put('/PatientMedicalHistory', payload);
  return res.data;
}

export interface PrescriptionItem {
  recordId: number;
  doctorName: string;
  specialization: string;
  recordDateTime: string;
  prescriptionPath: string | null;
  chiefComplaint: string;
  diagnosis: string;
  treatment: string;
}

export async function getPatientPrescriptions(tab: 'active' | 'past') {
  const res = await api.get('/PatientPrescriptions', {
    params: { tab, pageSize: 50 },
  });
  return res.data as { items: PrescriptionItem[] };
}

export async function downloadPatientPrescription(recordId: number) {
  const res = await api.get(`/PatientPrescriptions/${recordId}/download`, {
    responseType: 'blob',
  });
  return res.data as Blob;
}


// ── Booking API ──────────────────────────────────────────────────────────────

export interface BookingDoctorDto {
  doctorId: number;
  fullName: string;
  specialization: string;
  area: string;
  profilePicture?: string;
}

export async function getBookingAreas(): Promise<string[]> {
  const res = await api.get('/PatientBooking/areas');
  return res.data;
}

export async function getBookingSpecialties(area: string): Promise<string[]> {
  const res = await api.get('/PatientBooking/specialties', { params: { area } });
  return res.data;
}

export async function getBookingDoctors(area: string, specialty: string): Promise<BookingDoctorDto[]> {
  const res = await api.get('/PatientBooking/doctors', { params: { area, specialty } });
  return res.data;
}

export interface CreateBookingPayload {
  doctorId: number;
  appointmentDate: string; // "YYYY-MM-DD"
  area: string;
  specialty: string;
  reasonForVisit?: string;
}

export async function createBooking(payload: CreateBookingPayload) {
  const res = await api.post('/PatientBooking', payload);
  return res.data;
}
