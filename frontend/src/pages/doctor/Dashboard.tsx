import { StatusBadge } from "@/components/shared/StatusBadge";
import { useProfile } from "@/contexts/ProfileContext";
import { useLanguage } from "@/contexts/LanguageContext";
import { useQuery } from "@tanstack/react-query";
import { getDoctorTodayDashboard } from '@/api/dashboard.api';
import { useAuth } from "@/contexts/AuthContext";

interface Appointment {
  patient: string;
  status: "Waiting" | "Completed" | "Cancelled" | "In Progress";
}

export default function Dashboard() {
  const { profileImage } = useProfile();
  const { t } = useLanguage();
  const { user } = useAuth();

  const { data: appointments = [], isLoading } = useQuery({
    queryKey: ['today-appointments'],
    queryFn: async () => {
      const response = await getDoctorTodayDashboard();
      return response.appointments as Appointment[];
    },
  });

  return (
    <>
      <div className="flex items-center gap-3 mb-8">
        <img
          src={profileImage}
          alt={user?.fullName || t.drName}
          className="w-10 h-10 rounded-full object-cover"
        />
        <h1 className="text-xl font-semibold text-foreground">
          {user?.fullName || t.drName}
        </h1>
      </div>

      <div className="bg-card rounded-xl border border-border overflow-hidden w-full max-w-2xl">
        <div className="px-4 md:px-6 py-3 md:py-4 border-b border-border">
          <h2 className="text-sm md:text-base font-semibold text-foreground">
            {t.todaysAppointments}
          </h2>
        </div>

        {isLoading ? (
          <div className="p-12 flex justify-center">
            <div className="w-8 h-8 border-4 border-primary/30 border-t-primary rounded-full animate-spin" />
          </div>
        ) : appointments.length === 0 ? (
          <div className="p-8 text-center text-muted-foreground">
            {"No appointments for today"}
          </div>
        ) : (
          <>
            <div className="hidden md:block">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-border bg-muted/40">
                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.patient}
                    </th>

                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.status}
                    </th>
                  </tr>
                </thead>

                <tbody className="divide-y divide-border">
                  {appointments.map((appt, i) => (
                    <tr
                      key={i}
                      className="hover:bg-muted/30 transition-colors"
                    >
                      <td className="px-6 py-3.5 text-sm text-foreground">
                        {appt.patient}
                      </td>

                      <td className="px-6 py-3.5">
                        <StatusBadge status={appt.status} />
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="md:hidden divide-y divide-border">
              {appointments.map((appt, i) => (
                <div
                  key={i}
                  className="flex items-center justify-between px-4 py-3"
                >
                  <p className="text-sm font-medium text-foreground">
                    {appt.patient}
                  </p>

                  <StatusBadge status={appt.status} />
                </div>
              ))}
            </div>
          </>
        )}
      </div>
    </>
  );
}