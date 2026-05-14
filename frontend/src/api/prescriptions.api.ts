import api from '@/lib/api';

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

