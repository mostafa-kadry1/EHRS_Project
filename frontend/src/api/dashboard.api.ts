import api from '@/lib/api';

export interface DoctorTodayAppointment {
  patient: string;
  status: 'Waiting' | 'Completed' | 'Cancelled' | 'In Progress';
}

export async function getDoctorTodayDashboard() {
  const res = await api.get('/Dashboard/today');
  return res.data as { appointments: DoctorTodayAppointment[] };
}

