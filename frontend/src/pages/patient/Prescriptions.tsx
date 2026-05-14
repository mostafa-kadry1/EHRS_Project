import React, { useState } from "react";
import "./prescription.css";
import { FaDownload } from "react-icons/fa";
import defaultDoctor from "@/assets/patient_images/doctor.jpg";
import { useLanguage as useLang } from "@/contexts/LanguageContext";
import { useProfile } from "@/contexts/ProfileContext";
import { useAuth } from "@/contexts/AuthContext";
import { useQuery } from "@tanstack/react-query";
import { downloadPatientPrescription, getPatientPrescriptions } from "@/api/prescriptions.api";
import { getPatientDashboard } from "@/api/patient.api";
import { Loader2 } from "lucide-react";
import PatientPageHeader from "@/components/patient/PatientPageHeader";
import EmptyState from "@/components/patient/EmptyState";
import { toast } from "sonner";

const toArabicNums = (str: any) =>
  String(str || "").replace(/\d/g, (d) => "٠١٢٣٤٥٦٧٨٩"[+d]);

const monthsAr: Record<string, string> = {
  "01":"يناير","02":"فبراير","03":"مارس","04":"أبريل",
  "05":"مايو","06":"يونيو","07":"يوليو","08":"أغسطس",
  "09":"سبتمبر","10":"أكتوبر","11":"نوفمبر","12":"ديسمبر",
};

const formatDateAr = (dateStr: string) => {
  if (!dateStr) return "";
  const d = new Date(dateStr);
  const day = String(d.getDate());
  const month = String(d.getMonth() + 1).padStart(2, "0");
  const year = String(d.getFullYear());
  return `${toArabicNums(day)} ${monthsAr[month]} ${toArabicNums(year)}`;
};

interface PrescriptionItem {
  recordId: number;
  doctorName: string;
  doctorProfilePicture?: string;
  specialization: string;
  recordDateTime: string;
  prescriptionPath: string | null;
  chiefComplaint: string;
  diagnosis: string;
  treatment: string;
}

const Prescriptions = () => {
  const { t, lang } = useLang();
  const isAr = lang === "ar";
  const { user } = useAuth();
  const { patientImage } = useProfile();
  const [activeTab, setActiveTab] = useState<"active" | "past">("active");

  const { data: dashboard } = useQuery({
    queryKey: ["patient-dashboard"],
    queryFn: getPatientDashboard,
    staleTime: 60_000,
  });

  const { data, isLoading } = useQuery({
    queryKey: ["patient-prescriptions", activeTab],
    queryFn: async () => (await getPatientPrescriptions(activeTab)).items as PrescriptionItem[],
  });

  const prescriptions = data ?? [];

  const displayDate = (d: string) =>
    isAr ? formatDateAr(d) : new Date(d).toLocaleDateString();

  const handleDownload = async (recordId: number) => {
    try {
      const blob = await downloadPatientPrescription(recordId);
      const url = window.URL.createObjectURL(blob);
      const link = document.createElement("a");
      link.href = url;
      link.setAttribute("download", `prescription_${recordId}.pdf`);
      document.body.appendChild(link);
      link.click();
      link.remove();
      window.URL.revokeObjectURL(url);
    } catch {
      toast.error(isAr ? "فشل تنزيل الوصفة" : "Failed to download prescription");
    }
  };

  return (
    <div className="prescription-card" dir={isAr ? "rtl" : "ltr"}>

      <PatientPageHeader
        title={isAr ? "الوصفات الطبية" : t.prescriptionsTitle}
        name={dashboard?.fullName || user?.fullName || ""}
        patientId={dashboard?.patientId ?? ""}
        imageUrl={dashboard?.profilePicture || patientImage}
        isAr={isAr}
        patientIdLabel={t.patientId}
      />

      {/* Tabs */}
      <div className="tabs">
        <span className={activeTab === "active" ? "active" : ""} onClick={() => setActiveTab("active")}>
          {t.active}
        </span>
        <span className={activeTab === "past" ? "active" : ""} onClick={() => setActiveTab("past")}>
          {t.past}
        </span>
      </div>
      <div className="title-line" />

      {isLoading ? (
        <div className="flex justify-center py-20">
          <Loader2 className="w-10 h-10 animate-spin text-primary" />
        </div>
      ) : prescriptions.length === 0 ? (
        <EmptyState icon="💊" message={isAr ? "لا توجد وصفات طبية." : "No prescriptions found."} />
      ) : (
        prescriptions.map((item) => (
          <div key={item.recordId} className="prescription-wrapper">
            <div className="prescription-header">
              <div className="doctor-info-rx">
                <img
                  src={item.doctorProfilePicture || defaultDoctor}
                  alt={item.doctorName}
                  onError={(e) => { (e.target as HTMLImageElement).src = defaultDoctor; }}
                />
                <div>
                  <h4>{item.doctorName}</h4>
                  <p>{item.specialization}</p>
                </div>
              </div>
              <span className="date">{displayDate(item.recordDateTime)}</span>
            </div>

            <div className="prescription-box">
              <div className="left-box">
                {!item.prescriptionPath ? (
                  <div className="upload-box disabled-box">
                    {isAr ? "لا يوجد ملف PDF" : "No PDF uploaded"}
                  </div>
                ) : (
                  <div className="pdf-viewer-placeholder flex flex-col items-center justify-center border rounded-lg p-8 bg-muted/20">
                    <p className="text-sm mb-4">{isAr ? "ملف الوصفة متاح" : "Prescription PDF available"}</p>
                    <button onClick={() => handleDownload(item.recordId)} className="download-btn">
                      <FaDownload /> {t.download}
                    </button>
                  </div>
                )}
              </div>

              <div className="right-box">
                <div className="input-group">
                  <label>{isAr ? "الشكوى الرئيسية" : "Chief Complaint"}</label>
                  <input type="text" value={item.chiefComplaint} readOnly />
                </div>
                <div className="input-group">
                  <label>{t.diagnosis}</label>
                  <input type="text" value={item.diagnosis} readOnly />
                </div>
                <div className="input-group">
                  <label>{isAr ? "العلاج" : "Treatment"}</label>
                  <input type="text" value={item.treatment} readOnly />
                </div>
              </div>
            </div>
          </div>
        ))
      )}
    </div>
  );
};

export default Prescriptions;
