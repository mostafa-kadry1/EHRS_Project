import api from '@/lib/api';

export interface DoctorProfileData {
  fullName: string;
  specialization: string;
  medicalLicense: string;
  email: string;
  affiliatedHospital: string;
  about: string;
  profilePicture?: string;
}

export async function getDoctorProfile() {
  const res = await api.get('/DoctorProfile');
  return res.data as DoctorProfileData;
}

export async function updateDoctorProfile(payload: FormData) {
  const res = await api.put('/DoctorProfile', payload, {
    headers: { 'Content-Type': 'multipart/form-data' },
  });
  return res.data;
}

