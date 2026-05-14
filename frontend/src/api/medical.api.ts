import api from '@/lib/api';

export interface MedicalHistoryData {
  chronicDiseases: string[];
  allergies: string[];
  surgeries: any[];
}

export async function getPatientMedicalHistory() {
  const res = await api.get('/PatientMedicalHistory');
  return res.data as MedicalHistoryData;
}

export async function updatePatientMedicalHistory(payload: Partial<MedicalHistoryData>) {
  const res = await api.put('/PatientMedicalHistory', payload);
  return res.data as MedicalHistoryData;
}

