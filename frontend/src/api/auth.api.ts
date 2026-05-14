import api from '@/lib/api';

export type Role = 'Patient' | 'Doctor';

export async function loginPatient(data: any) {
  const res = await api.post('/PatientAuth/login', data);
  return res.data;
}

export async function loginDoctor(data: any) {
  const res = await api.post('/DoctorAuth/login', data);
  return res.data;
}

export async function registerPatient(data: any) {
  const res = await api.post('/PatientAuth/register', data);
  return res.data;
}

export async function registerDoctor(data: any) {
  const res = await api.post('/DoctorAuth/register', data);
  return res.data;
}

