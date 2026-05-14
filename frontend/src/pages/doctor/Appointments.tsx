import { useState } from "react";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { Search } from "lucide-react";
import { useLanguage } from "@/contexts/LanguageContext";
import { useQuery } from "@tanstack/react-query";
import { getDoctorAppointments } from '@/api/appointments.api';

interface Appointment {
  id: string;
  patient: string;
  date: string;
  type: string;
  status: "Waiting" | "Scheduled" | "Completed" | "Cancelled" | "In Progress";
}

export default function Appointments() {
  const { t } = useLanguage();
  const [tab, setTab] = useState<"upcoming" | "past">("upcoming");
  const [search, setSearch] = useState("");

  const { data, isLoading } = useQuery({
    queryKey: ['appointments', tab, search],
    queryFn: async () => {
      // The backend returns a PagedResult: { items: [...] }
      const items = await getDoctorAppointments(tab, search);
      return items as Appointment[];
    },
  });

  const appointments = data || [];

  return (
    <>
      <div className="mb-6">
        <h1 className="text-xl md:text-2xl font-bold text-foreground">
          {t.appointments}
        </h1>

        <p className="text-sm text-muted-foreground mt-1">
          {t.manageAppointments}
        </p>
      </div>

      <div className="flex gap-1 bg-muted p-1 rounded-lg w-fit mb-6">
        {(["upcoming", "past"] as const).map((tabKey) => (
          <button
            key={tabKey}
            onClick={() => {
              setTab(tabKey);
              setSearch("");
            }}
            className={`px-3 md:px-5 py-2 rounded-md text-xs md:text-sm font-medium capitalize transition-all ${
              tab === tabKey
                ? "bg-primary text-primary-foreground shadow-sm"
                : "text-muted-foreground hover:text-foreground"
            }`}
          >
            {tabKey === "upcoming" ? t.upcoming : t.past}
          </button>
        ))}
      </div>

      <div className="bg-card rounded-xl border border-border overflow-hidden">
        <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between px-4 md:px-6 py-3 md:py-4 border-b border-border gap-3">
          <div className="flex items-center gap-3">
            <h2 className="text-sm md:text-base font-semibold text-foreground whitespace-nowrap">
              {tab === "upcoming"
                ? t.upcomingAppointments
                : t.pastAppointments}
            </h2>
          </div>

          <div className="relative flex-1 sm:flex-none">
            <Search className="absolute left-2.5 top-1/2 -translate-y-1/2 w-4 h-4 text-muted-foreground rtl:left-auto rtl:right-2.5" />

            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder={t.searchPatient}
              className="pl-8 pr-3 py-1.5 text-sm rounded-md border border-border bg-background text-foreground placeholder:text-muted-foreground focus:outline-none focus:ring-2 focus:ring-ring w-full sm:w-40 md:w-56 rtl:pl-3 rtl:pr-8"
            />
          </div>
        </div>

        {isLoading ? (
          <div className="p-12 flex justify-center">
            <div className="w-8 h-8 border-4 border-primary/30 border-t-primary rounded-full animate-spin" />
          </div>
        ) : appointments.length === 0 ? (
          <div className="p-12 text-center text-muted-foreground">
            {"No appointments found"}
          </div>
        ) : (
          <>
            <div className="hidden md:block overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="bg-muted/40">
                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.patient}
                    </th>

                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.date}
                    </th>

                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.type}
                    </th>

                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.status}
                    </th>

                    <th className="text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider px-6 py-3 rtl:text-right">
                      {t.id}
                    </th>
                  </tr>
                </thead>

                <tbody className="divide-y divide-border">
                  {appointments.map((appt) => (
                    <tr
                      key={appt.id}
                      className="hover:bg-muted/30 transition-colors"
                    >
                      <td className="px-6 py-4">
                        <span className="text-sm font-medium text-foreground">
                          {appt.patient}
                        </span>
                      </td>

                      <td className="px-6 py-4 text-sm text-muted-foreground">
                        {appt.date}
                      </td>

                      <td className="px-6 py-4 text-sm text-muted-foreground">
                        {appt.type}
                      </td>

                      <td className="px-6 py-4">
                        <StatusBadge status={appt.status} />
                      </td>

                      <td className="px-6 py-4">
                        <span className="text-xs font-mono text-muted-foreground bg-muted px-2 py-1 rounded">
                          {appt.id}
                        </span>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            <div className="md:hidden divide-y divide-border">
              {appointments.map((appt) => (
                <div
                  key={appt.id}
                  className="px-4 py-3"
                >
                  <div className="flex items-center gap-3">
                    <div className="flex-1 min-w-0">
                      <p className="text-sm font-medium text-foreground truncate">
                        {appt.patient}
                      </p>

                      <p className="text-xs text-muted-foreground">
                        {appt.date} · {appt.type}
                      </p>
                    </div>

                    <StatusBadge status={appt.status} />
                  </div>

                  <div className="mt-1.5 ps-12">
                    <span className="text-[10px] font-mono text-muted-foreground bg-muted px-1.5 py-0.5 rounded">
                      {appt.id}
                    </span>
                  </div>
                </div>
              ))}
            </div>
          </>
        )}
      </div>
    </>
  );
}