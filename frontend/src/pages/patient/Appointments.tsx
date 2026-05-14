import React, { useState } from "react";
import "./appointments.css";
import defaultDoctor from "@/assets/patient_images/doctor.jpg";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { cancelPatientAppointment, getPatientAppointments } from "@/api/patient.api";
import { useProfile } from "@/contexts/ProfileContext";
import { Loader2 } from "lucide-react";
import { toast } from "sonner";
import PatientPageHeader from "@/components/patient/PatientPageHeader";
import EmptyState from "@/components/patient/EmptyState";
import { useAuth } from "@/contexts/AuthContext";
import { getPatientDashboard } from "@/api/patient.api";

const toArabicNums = (str: any) =>
  String(str || "").replace(/\d/g, (d) => "٠١٢٣٤٥٦٧٨٩"[+d]);

const monthsAr: Record<string, string> = {
  "01": "يناير","02": "فبراير","03": "مارس","04": "أبريل",
  "05": "مايو","06": "يونيو","07": "يوليو","08": "أغسطس",
  "09": "سبتمبر","10": "أكتوبر","11": "نوفمبر","12": "ديسمبر",
};
const monthsEn: Record<string, string> = {
  "01":"Jan","02":"Feb","03":"Mar","04":"Apr",
  "05":"May","06":"Jun","07":"Jul","08":"Aug",
  "09":"Sep","10":"Oct","11":"Nov","12":"Dec",
};

type Status = "Waiting" | "Scheduled" | "Completed" | "Cancelled" | "In Progress";
interface Appointment {
  appointmentId: number;
  appointmentDateTime: string;
  doctorId: number;
  doctorName: string;
  doctorProfilePicture?: string;
  reasonForVisit?: string;
  status: Status;
}

const STATUS_AR: Record<string, string> = {
  waiting: "انتظار", scheduled: "مجدول", completed: "مكتمل",
  cancelled: "ملغي", "in progress": "جارٍ",
};

export default function Appointments() {
  const { t, lang } = useLang();
  const isAr = lang === "ar";
  const queryClient = useQueryClient();
  const { user } = useAuth();
  const { patientImage } = useProfile();
  const [confirmId, setConfirmId] = useState<number | null>(null);

  // Load header data (reuse dashboard query if already cached)
  const { data: dashboard } = useQuery({
    queryKey: ["patient-dashboard"],
    queryFn: getPatientDashboard,
    staleTime: 60_000,
  });

  const { data: appointments = [], isLoading } = useQuery({
    queryKey: ["patient-appointments"],
    queryFn: async () => {
      const response = await getPatientAppointments();
      return response.items as Appointment[];
    },
  });

  const cancelMutation = useMutation({
    mutationFn: (id: number) => cancelPatientAppointment(id),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["patient-appointments"] });
      toast.success(isAr ? "تم إلغاء الموعد" : "Appointment cancelled");
      setConfirmId(null);
    },
    onError: () => toast.error(isAr ? "فشل الإلغاء" : "Failed to cancel appointment"),
  });

  const colors = [
    "linear-gradient(135deg, #24487D, #4a90e2)",
    "linear-gradient(135deg, #445A82, #a7a5b3)",
    "linear-gradient(135deg, #2966A2, #6885a2)",
    "linear-gradient(135deg, #4b5462, #5e5887)",
  ];

  const formatDateBox = (dateStr: string) => {
    const d = new Date(dateStr);
    const monthNum = String(d.getMonth() + 1).padStart(2, "0");
    const day = String(d.getDate());
    const year = String(d.getFullYear());
    return {
      month: isAr ? monthsAr[monthNum] : monthsEn[monthNum],
      day: isAr ? toArabicNums(day) : day,
      year: isAr ? toArabicNums(year) : year,
    };
  };

  const statusLabel = (s: string) => {
    if (isAr) return STATUS_AR[s.toLowerCase()] ?? s;
    return s;
  };

  if (isLoading) {
    return (
      <div className="flex h-64 items-center justify-center">
        <Loader2 className="w-8 h-8 animate-spin text-primary" />
      </div>
    );
  }

  return (
    <div className="appointments-container" dir={isAr ? "rtl" : "ltr"}>

      <PatientPageHeader
        title={t.appointmentsTitle}
        name={dashboard?.fullName || user?.fullName || ""}
        patientId={dashboard?.patientId ?? ""}
        imageUrl={dashboard?.profilePicture || patientImage}
        isAr={isAr}
        patientIdLabel={t.patientId}
      />

      {appointments.length === 0 ? (
        <EmptyState
          icon="🗓️"
          message={(t as any).noAppointmentsFound || (isAr ? "لا توجد مواعيد." : "No appointments found.")}
        />
      ) : (
        appointments.map((app, index) => {
          const { month, day, year } = formatDateBox(app.appointmentDateTime);
          return (
            <div className="appointment-card" key={app.appointmentId}>
              <div className="date-box" style={{ background: colors[index % colors.length] }}>
                <p>{month}</p>
                <h3>{day}</h3>
                <span>{year}</span>
              </div>
              <img
                src={app.doctorProfilePicture || defaultDoctor}
                alt={app.doctorName}
                className="doctor-img"
                onError={(e) => { (e.target as HTMLImageElement).src = defaultDoctor; }}
              />
              <div className="vertical-line" />
              <div className="doctor-info">
                <h3>{app.doctorName}</h3>
                <p>{app.reasonForVisit || (isAr ? "استشارة" : "Consultation")}</p>
                <div className={`status-tag ${app.status.toLowerCase().replace(" ", "-")}`}>
                  {statusLabel(app.status)}
                </div>
              </div>
              <button
                className="cancel-btn"
                onClick={() => setConfirmId(app.appointmentId)}
                disabled={app.status.toLowerCase() === "cancelled"}
              >
                {t.cancel}
              </button>
            </div>
          );
        })
      )}

      {confirmId !== null && (
        <div className="confirm-overlay">
          <div className="confirm-modal">
            <div className="confirm-icon">🗓️</div>
            <h3>{isAr ? "تأكيد الإلغاء" : "Confirm Cancellation"}</h3>
            <p>{isAr ? "هل أنت متأكد من إلغاء هذا الموعد؟" : "Are you sure you want to cancel this appointment?"}</p>
            <div className="confirm-actions">
              <button
                className="confirm-yes"
                onClick={() => confirmId && cancelMutation.mutate(confirmId)}
                disabled={cancelMutation.isPending}
              >
                {cancelMutation.isPending ? <Loader2 className="w-4 h-4 animate-spin" /> : (isAr ? "نعم، إلغاء" : "Yes, Cancel")}
              </button>
              <button className="confirm-no" onClick={() => setConfirmId(null)} disabled={cancelMutation.isPending}>
                {isAr ? "لا، رجوع" : "No, Go Back"}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
