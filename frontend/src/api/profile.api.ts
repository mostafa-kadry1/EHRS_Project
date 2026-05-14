import api from '@/lib/api';

export interface PatientProfileData {
  patientId: string;
  fullName: string;
  gender: string;
  birthDate: string;
  email: string;
  contactNumber: string;
  address: string;
  bloodType: string;
  heightCm: string;
  weightKg: string;
  ssn: string;
  age: string;
  profilePicture?: string;
}

export async function getPatientProfile() {
  const res = await api.get('/PatientProfile');
  return res.data as PatientProfileData;
}

export async function updatePatientProfile(payload: FormData) {
  const res = await api.put('/PatientProfile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return res.data as PatientProfileData;
}

