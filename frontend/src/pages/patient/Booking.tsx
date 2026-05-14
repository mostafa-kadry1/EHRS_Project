import React, { useState, useEffect } from "react";
import "./booking.css";
import { Loader2, MapPin, Stethoscope, UserRound, FileText, CalendarDays } from "lucide-react";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useProfile } from "@/contexts/ProfileContext";
import { useAuth } from "@/contexts/AuthContext";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import {
  getBookingAreas, getBookingSpecialties,
  getBookingDoctors, createBooking,
  getPatientDashboard, type BookingDoctorDto,
} from "@/api/patient.api";
import { toast } from "sonner";
import PatientPageHeader from "@/components/patient/PatientPageHeader";
import DateInput from "@/components/shared/DateInput";

const TODAY = new Date();
TODAY.setHours(0, 0, 0, 0);
const MAX_DATE = new Date(TODAY);
MAX_DATE.setFullYear(MAX_DATE.getFullYear() + 1);

export default function Booking() {
  const { t, lang } = useLang();
  const isAr = lang === "ar";
  const { user } = useAuth();
  const { patientImage } = useProfile();
  const queryClient = useQueryClient();

  const [area, setArea] = useState("");
  const [specialty, setSpecialty] = useState("");
  const [doctorId, setDoctorId] = useState<number | "">("");
  const [appointmentDate, setAppointmentDate] = useState<Date | undefined>();
  const [reason, setReason] = useState("");

  // Dashboard header data
  const { data: dashboard } = useQuery({
    queryKey: ["patient-dashboard"],
    queryFn: getPatientDashboard,
    staleTime: 60_000,
  });

  // Areas from backend — fall back to the static list in LanguageContext if empty
  const { data: backendAreas = [], isLoading: areasLoading, isError: areasError, refetch: refetchAreas } =
    useQuery<string[]>({
      queryKey: ["booking-areas"],
      queryFn: getBookingAreas,
      staleTime: 300_000,
      retry: 2,
      retryDelay: 1_000,
    });

  // Use backend areas when available; fall back to the LanguageContext static list
  const areas: string[] = backendAreas.length > 0 ? backendAreas : (t.areas as unknown as string[]);

  // Specialties — load from backend when area is chosen; fall back to static list
  const { data: backendSpecialties = [], isFetching: specFetching } = useQuery<string[]>({
    queryKey: ["booking-specialties", area],
    queryFn: () => getBookingSpecialties(area),
    enabled: !!area && backendAreas.length > 0, // only call if backend areas work
    staleTime: 60_000,
  });

  const specialties: string[] =
    backendSpecialties.length > 0 ? backendSpecialties : (t.specialties as unknown as string[]);

  // Doctors — only from backend (requires real data)
  const { data: doctors = [], isFetching: docFetching } = useQuery<BookingDoctorDto[]>({
    queryKey: ["booking-doctors", area, specialty],
    queryFn: () => getBookingDoctors(area, specialty),
    enabled: !!(area && specialty && backendAreas.length > 0),
    staleTime: 60_000,
  });

  useEffect(() => { setSpecialty(""); setDoctorId(""); }, [area]);
  useEffect(() => { setDoctorId(""); }, [specialty]);

  const bookMutation = useMutation({
    mutationFn: () => {
      if (!area || !specialty || !appointmentDate) throw new Error("missing_fields");
      if (backendAreas.length > 0 && !doctorId) throw new Error("missing_fields");
      const selected = new Date(appointmentDate);
      selected.setHours(0, 0, 0, 0);
      if (selected < TODAY) throw new Error("past_date");
      return createBooking({
        doctorId: Number(doctorId) || 0,
        appointmentDate: selected.toISOString().slice(0, 10),
        area,
        specialty,
        reasonForVisit: reason || undefined,
      });
    },
    onSuccess: () => {
      toast.success(t.bookingSuccess || "Appointment booked successfully!");
      setArea(""); setSpecialty(""); setDoctorId("");
      setAppointmentDate(undefined); setReason("");
      queryClient.invalidateQueries({ queryKey: ["patient-appointments"] });
      queryClient.invalidateQueries({ queryKey: ["patient-dashboard"] });
    },
    onError: (err: any) => {
      const msg = err?.message;
      if (msg === "missing_fields") toast.error(t.bookingFillAllFields || "Please fill all required fields.");
      else if (msg === "past_date")  toast.error(t.bookingPastDate    || "Please select today or a future date.");
      else {
        const serverMsg = err?.response?.data?.message ?? err?.data?.message;
        toast.error(serverMsg || "Booking failed. Please try again.");
      }
    },
  });

  // Govern whether doctor dropdown is shown (only when backend has real doctors)
  const showDoctorSelect = backendAreas.length > 0;

  return (
    <div className="booking-container" dir={isAr ? "rtl" : "ltr"}>

      <PatientPageHeader
        title={t.bookingTitle}
        name={dashboard?.fullName || user?.fullName || ""}
        patientId={dashboard?.patientId ?? ""}
        imageUrl={dashboard?.profilePicture || patientImage}
        isAr={isAr}
        patientIdLabel={t.patientId}
      />

      <p className="booking-subtitle">{t.bookingSubtitle}</p>

      <form className="booking-card" onSubmit={(e) => { e.preventDefault(); bookMutation.mutate(); }} noValidate>

        {/* ── Governorate ── */}
        <label className="field-label">
          <MapPin size={15} className="field-label-icon" />
          {isAr ? "المحافظة" : "Governorate"}
        </label>
        <div className="field-wrap">
          <select
            value={area}
            onChange={(e) => setArea(e.target.value)}
            disabled={areasLoading}
            className="field-select"
          >
            <option value="">{isAr ? "اختر المحافظة" : "Select a governorate"}</option>
            {areas.map((a) => <option key={a} value={a}>{a}</option>)}
          </select>
          {areasLoading && <Loader2 size={16} className="field-spinner" />}
          {areasError && (
            <button type="button" onClick={() => refetchAreas()} className="field-retry">
              {isAr ? "إعادة المحاولة" : "Retry"}
            </button>
          )}
        </div>

        {/* ── Specialty ── */}
        <label className="field-label">
          <Stethoscope size={15} className="field-label-icon" />
          {isAr ? "التخصص" : "Specialty"}
        </label>
        <div className="field-wrap">
          <select
            value={specialty}
            onChange={(e) => setSpecialty(e.target.value)}
            disabled={!area || specFetching}
            className="field-select"
            title={!area ? (isAr ? "اختر المحافظة أولاً" : "Select a governorate first") : ""}
          >
            <option value="">{isAr ? "اختر التخصص" : "Select a specialty"}</option>
            {specialties.map((s) => <option key={s} value={s}>{s}</option>)}
          </select>
          {specFetching && <Loader2 size={16} className="field-spinner" />}
        </div>

        {/* ── Doctor (only if backend has real data) ── */}
        {showDoctorSelect && (
          <>
            <label className="field-label">
              <UserRound size={15} className="field-label-icon" />
              {isAr ? "الطبيب" : "Doctor"}
            </label>
            <div className="field-wrap">
              <select
                value={doctorId}
                onChange={(e) => setDoctorId(Number(e.target.value) || "")}
                disabled={!specialty || docFetching}
                className="field-select"
                title={!specialty ? (isAr ? "اختر التخصص أولاً" : "Select a specialty first") : ""}
              >
                <option value="">{isAr ? "اختر الطبيب" : "Select a doctor"}</option>
                {doctors.map((d) => <option key={d.doctorId} value={d.doctorId}>{d.fullName}</option>)}
              </select>
              {docFetching && <Loader2 size={16} className="field-spinner" />}
              {!docFetching && specialty && area && doctors.length === 0 && (
                <span className="field-no-results">
                  {isAr ? "لا يوجد أطباء متاحون" : "No doctors available"}
                </span>
              )}
            </div>
          </>
        )}

        {/* ── Appointment Date ── */}
        <label className="field-label">
          <CalendarDays size={15} className="field-label-icon" />
          {isAr ? "تاريخ الموعد" : "Appointment Date"}
        </label>
        <div className="field-wrap">
          <DateInput
            value={appointmentDate}
            onChange={setAppointmentDate}
            minDate={TODAY}
            maxDate={MAX_DATE}
            fromYear={new Date().getFullYear()}
            toYear={new Date().getFullYear() + 1}
            placeholder={isAr ? "يوم/شهر/سنة" : "DD/MM/YYYY"}
            confirmLabel={isAr ? "تأكيد" : "Confirm"}
            className="date-input-field"
          />
        </div>

        {/* ── Reason for Visit ── */}
        <label className="field-label">
          <FileText size={15} className="field-label-icon" />
          {isAr ? "سبب الزيارة" : "Reason for Visit"}
        </label>
        <div className="field-wrap">
          <select value={reason} onChange={(e) => setReason(e.target.value)} className="field-select">
            <option value="">{isAr ? "اختر سبب الزيارة" : "Select a reason for visit"}</option>
            {t.visitReasons.map((r, i) => <option key={i} value={r}>{r}</option>)}
          </select>
        </div>

        <button type="submit" className="book-btn" disabled={bookMutation.isPending}>
          {bookMutation.isPending
            ? <Loader2 className="w-5 h-5 animate-spin mx-auto" />
            : t.bookAppointment}
        </button>

      </form>
    </div>
  );
}
