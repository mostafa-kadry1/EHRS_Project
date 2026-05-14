import React from "react";
import { useNavigate } from "react-router-dom";
import "./dashboard.css";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useQuery } from "@tanstack/react-query";
import { getPatientDashboard } from "@/api/patient.api";
import { useAuth } from "@/contexts/AuthContext";
import { useProfile } from "@/contexts/ProfileContext";
import { Loader2 } from "lucide-react";
import PatientPageHeader from "@/components/patient/PatientPageHeader";
import EmptyState from "@/components/patient/EmptyState";

const toArabicNums = (str: any) =>
  String(str || "").replace(/\d/g, (d) => "٠١٢٣٤٥٦٧٨٩"[+d]);

const toArabicDate = (dateStr: string) => {
  if (!dateStr) return "";
  const months: Record<string, string> = {
    Jan: "يناير", Feb: "فبراير", Mar: "مارس", Apr: "أبريل",
    May: "مايو",  Jun: "يونيو",  Jul: "يوليو",  Aug: "أغسطس",
    Sep: "سبتمبر",Oct: "أكتوبر", Nov: "نوفمبر", Dec: "ديسمبر",
  };
  return dateStr
    .replace(/\b(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)\b/g, (m) => months[m])
    .replace(/\d+/g, (d) => toArabicNums(d));
};

export default function Dashboard() {
  const navigate = useNavigate();
  const { t, lang } = useLang();
  const { user } = useAuth();
  const { patientImage } = useProfile();
  const isAr = lang === "ar";

  const { data: dashboard, isLoading } = useQuery({
    queryKey: ["patient-dashboard"],
    queryFn: getPatientDashboard,
  });

  const fmt = (val: any) => (isAr ? toArabicNums(val) : String(val ?? ""));
  const fmtDate = (d: string) =>
    isAr
      ? toArabicDate(new Date(d).toLocaleDateString("en-US", { year: "numeric", month: "short", day: "numeric" }))
      : new Date(d).toLocaleDateString();

  if (isLoading) {
    return (
      <div className="flex h-screen items-center justify-center">
        <Loader2 className="w-10 h-10 animate-spin text-primary" />
      </div>
    );
  }

  const visits = dashboard?.recentVisits ?? [];

  return (
    <div className="dashboard-container" dir={isAr ? "rtl" : "ltr"}>

      {/* ── Unified patient header ── */}
      <PatientPageHeader
        title={t.patientDashboard}
        name={dashboard?.fullName || user?.fullName || ""}
        patientId={dashboard?.patientId ?? ""}
        imageUrl={dashboard?.profilePicture || patientImage}
        isAr={isAr}
        patientIdLabel={t.patientId}
      />

      {/* Vital Signs */}
      <div className="vital-section">
        <h3 className="section-title">{t.vitalSigns}</h3>
        <div className="vital-grid">
          <div className="vital-card"><div className="vital-top"><span className="icon">🌡</span><p>{t.temperature}</p></div><h4>{fmt(dashboard?.vitalSigns?.temperature ?? "--")} °C</h4></div>
          {/* Fix: proper Arabic bpm label */}
          <div className="vital-card"><div className="vital-top"><span className="icon">❤️</span><p>{t.heartRate}</p></div><h4>{fmt(dashboard?.vitalSigns?.heartRate ?? "--")} {isAr ? "نبضة/دقيقة" : "bpm"}</h4></div>
          <div className="vital-card"><div className="vital-top"><span className="icon">🩺</span><p>{t.bloodPressure}</p></div>
            <h4>
              {(() => {
                const raw = dashboard?.vitalSigns?.pressureHeart;
                if (raw === null || raw === undefined) return "--";
                const rawStr = String(raw);
                // Case 1: already "120/80" format from DB
                if (rawStr.includes("/")) {
                  const [sys, dia] = rawStr.split("/");
                  return isAr
                    ? `${fmt(sys)} / ${fmt(dia)} ملم زئبق`
                    : `${sys} / ${dia} mmHg`;
                }
                // Case 2: decimal like 120.80 — integer part = systolic, decimal = diastolic
                const num = parseFloat(rawStr);
                if (!isNaN(num)) {
                  const sys = Math.floor(num);
                  // diastolic encoded as fractional * 100 (e.g. 120.80 → 80)
                  const dia = Math.round((num - sys) * 100);
                  if (dia > 0 && dia < sys) {
                    return isAr
                      ? `${fmt(sys)} / ${fmt(dia)} ملم زئبق`
                      : `${sys} / ${dia} mmHg`;
                  }
                  // If only one value (no fractional), show as-is
                  return isAr ? `${fmt(sys)} ملم زئبق` : `${sys} mmHg`;
                }
                return rawStr;
              })()}
            </h4>
          </div>
          <div className="vital-card"><div className="vital-top"><span className="icon">🫁</span><p>{t.oxygenSat}</p></div><h4>{fmt(dashboard?.vitalSigns?.spO2 ?? "--")}%</h4></div>
          <div className="vital-card"><div className="vital-top"><span className="icon">⚖️</span><p>{t.weight}</p></div><h4>{fmt(dashboard?.weightKg ?? "--")} {isAr ? "كجم" : "kg"}</h4></div>
          <div className="vital-card"><div className="vital-top"><span className="icon">📏</span><p>{t.height}</p></div><h4>{fmt(dashboard?.heightCm ?? "--")} {isAr ? "سم" : "cm"}</h4></div>
          <div className="vital-card"><div className="vital-top"><span className="icon">📊</span><p>{t.bmi}</p></div><h4>{fmt(dashboard?.bmi ?? "--")}</h4></div>
          <div className="vital-card"><div className="vital-top"><span className="icon">🩸</span><p>{t.bloodType}</p></div><h4>{dashboard?.bloodType ?? "--"}</h4></div>
        </div>
      </div>

      {/* Upcoming Appointment */}
      <div className="upcoming-section">
        <h3 className="section-title">{t.upcomingAppointment}</h3>
        {dashboard?.upcomingAppointment ? (
          <div className="upcoming-card">
            <div className="appointment-info">
              <p className="appointment-time">
                {new Date(dashboard.upcomingAppointment.appointmentDateTime).toLocaleString(
                  isAr ? "ar-EG" : "en-US"
                )}
              </p>
              <h4 className="doctor-name">{dashboard.upcomingAppointment.doctorName}</h4>
            </div>
            <button className="primary-btn" onClick={() => navigate("/patient/appointments")}>
              {t.viewFullAppointment}
            </button>
          </div>
        ) : (
          <EmptyState icon="📅" message={isAr ? "لا توجد مواعيد قادمة" : "No upcoming appointments"} />
        )}
      </div>

      {/* Recent Visits */}
      <div className="recent-section">
        <h3 className="section-title">{t.recentVisits}</h3>

        {visits.length === 0 ? (
          <EmptyState icon="🗂️" message={(t as any).noRecentVisits || "No recent visits recorded."} />
        ) : (
          <div className="recent-card">
            <table className="visit-table">
              <thead>
                <tr>
                  <th>{t.date}</th>
                  <th>{t.diagnosis}</th>
                  <th>{t.notes}</th>
                </tr>
              </thead>
              <tbody>
                {visits.map((visit: any) => (
                  <tr key={visit.recordId}>
                    <td>{fmtDate(visit.recordDateTime)}</td>
                    <td>{visit.diagnosis}</td>
                    <td>{visit.notes}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}
      </div>
    </div>
  );
}
