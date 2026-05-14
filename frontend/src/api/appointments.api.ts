import api from '@/lib/api';

export interface DashboardAppointment {
  patient: string;
  status: 'Waiting' | 'Scheduled' | 'Completed' | 'Cancelled' | 'In Progress';
}

export async function getDoctorAppointments(tab: 'upcoming' | 'past', search: string) {
  const res = await api.get(`/Appointments/${tab}`, {
    params: { search, pageSize: 50 },
  });

  // backend returns PagedResult: { items: [...] }
  return res.data.items as DashboardAppointment[];
}

